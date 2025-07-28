using BusinessObjects.Enums;
using DTOs.Payments;
using Microsoft.AspNetCore.Mvc;
using Services.BookingServices;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace HomestayBookingProject.Controllers
{
    [Route("api/payments/vnpay")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IBookingService _bookingService;
        private readonly ILogger<VNPayController> _logger;

        private readonly string _vnpTmnCode;
        private readonly string _vnpHashSecret;
        private readonly string _vnpUrl;
        private readonly string _vnpReturnUrl;

        public VNPayController(
            IConfiguration configuration,
            IBookingService bookingService,
            ILogger<VNPayController> logger)
        {
            _configuration = configuration;
            _bookingService = bookingService;
            _logger = logger;

            // VNPay Configuration
            _vnpTmnCode = _configuration["VNPay:TmnCode"] ?? "DEMO";
            _vnpHashSecret = _configuration["VNPay:HashSecret"] ?? "DEMOSECRET";
            _vnpUrl = _configuration["VNPay:Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            _vnpReturnUrl = _configuration["VNPay:ReturnUrl"] ?? "http://localhost:3000/payment-result";
        }

        [HttpPost("create-vnpay-payment")]
        public async Task<IActionResult> CreateVNPayPayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                _logger.LogInformation("Creating VNPay payment for booking: {BookingId}", request.BookingId);

                var existingBooking = await _bookingService.GetBookingByIdAsync(request.BookingId);
                if (existingBooking == null)
                {
                    return BadRequest(new CreatePaymentResponse
                    {
                        Success = false,
                        Message = "Booking not exists"
                    });
                }

                var vnpayUrl = CreateVNPayUrl(request);

                return Ok(new CreatePaymentResponse
                {
                    Success = true,
                    PaymentUrl = vnpayUrl,
                    BookingId = request.BookingId,
                    Message = "Payment URL created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay payment for booking: {BookingId}", request.BookingId);
                return StatusCode(500, new CreatePaymentResponse
                {
                    Success = false,
                    Message = "Internal server error",
                    BookingId = request.BookingId
                });
            }
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            try
            {
                _logger.LogInformation("Confirming payment for booking: {BookingId}", request.BookingId);
                var booking = await _bookingService.GetBookingByIdAsync(request.BookingId);
                if (booking == null)
                {
                    return NotFound(new ConfirmPaymentResponse
                    {
                        Success = false,
                        BookingId = request.BookingId,
                        Message = "Booking not found"
                    });
                }

                if (booking.Status != BookingStatus.Pending)
                {
                    return Ok(new ConfirmPaymentResponse
                    {
                        Success = booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Completed,
                        BookingId = request.BookingId,
                        Status = booking.Status.ToString(),
                        Message = "Payment already processed"
                    });
                }

                var responseCode = request.VnpayParams?.GetValueOrDefault("vnp_ResponseCode") ?? "00";

                if (responseCode == "00")
                {
                    await _bookingService.UpdateBookingStatusAsync(request.BookingId, BookingStatus.Confirmed);

                    _logger.LogInformation("✅ Booking {BookingId} confirmed successfully", request.BookingId);

                    return Ok(new ConfirmPaymentResponse
                    {
                        Success = true,
                        BookingId = request.BookingId,
                        Status = "confirmed",
                        PaymentStatus = "success",
                        Message = "Payment confirmed successfully"
                    });
                }
                else
                {
                    _logger.LogInformation("❌ Booking {BookingId} cancelled due to payment failure", request.BookingId);

                    return Ok(new ConfirmPaymentResponse
                    {
                        Success = false,
                        BookingId = request.BookingId,
                        Status = "cancelled",
                        PaymentStatus = "failed",
                        Message = "Payment failed or cancelled"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment for booking: {BookingId}", request.BookingId);
                return StatusCode(500, new ConfirmPaymentResponse
                {
                    Success = false,
                    BookingId = request.BookingId,
                    Message = "Internal server error"
                });
            }
        }

        [HttpGet("ipn")]
        public async Task<IActionResult> HandleVNPayIPN([FromQuery] Dictionary<string, string> vnpParams)
        {
            try
            {
                if (!vnpParams.TryGetValue("vnp_SecureHash", out var secureHash) || string.IsNullOrEmpty(secureHash))
                {
                    return BadRequest(new { message = "Missing or invalid vnp_SecureHash" });
                }

                if (!vnpParams.TryGetValue("vnp_TxnRef", out var txnRef) || string.IsNullOrEmpty(txnRef))
                {
                    return BadRequest(new { message = "Missing vnp_TxnRef" });
                }

                vnpParams.TryGetValue("vnp_ResponseCode", out var responseCode);
                vnpParams.Remove("vnp_SecureHash");

                if (!ValidateVNPaySignature(vnpParams, secureHash))
                {
                    return BadRequest(new { message = "Invalid signature" });
                }
                if (!int.TryParse(txnRef, out var bookingId))
                {
                    return BadRequest(new { message = "Invalid booking ID" });
                }
                var frontendUrl = _configuration["App:ClientUrl"];
                if (responseCode == "00")
                {
                    await _bookingService.UpdateBookingStatusAsync(bookingId, BookingStatus.Confirmed);
                    var redirectPaymentSuccessUrl = $"{frontendUrl}/payment-success";
                    return Redirect(redirectPaymentSuccessUrl);
                }
                await _bookingService.UpdateBookingStatusAsync(bookingId, BookingStatus.Cancelled);
                var redirectPaymentFailed = $"{frontendUrl}/payment-fail";
                return Redirect(redirectPaymentFailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay IPN");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        private string CreateVNPayUrl(CreatePaymentRequest request)
        {
            // Sử dụng SortedList với VnPayCompare giống như trong VnPayLibrary
            var vnpParams = new SortedList<string, string>(new VnPayCompare())
            {
                {"vnp_Version", "2.1.0"},
                {"vnp_Command", "pay"},
                {"vnp_TmnCode", _vnpTmnCode},
                {"vnp_Amount", (request.Amount * 100).ToString()},
                {"vnp_CurrCode", "VND"},
                {"vnp_TxnRef", request.BookingId.ToString()},
                {"vnp_OrderInfo", $"Thanh toan don hang {request.BookingId}"},
                {"vnp_OrderType", "250000"},
                {"vnp_Locale", "vn"},
                {"vnp_ReturnUrl", _vnpReturnUrl},
                {"vnp_IpAddr", GetClientIpAddress()},
                {"vnp_CreateDate", DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(7)).ToString("yyyyMMddHHmmss")}
            };

            // Tạo signData giống VnPayLibrary - sử dụng WebUtility.UrlEncode
            var signDataBuilder = new StringBuilder();
            foreach (var kvp in vnpParams.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                signDataBuilder.Append($"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}&");
            }

            var signData = signDataBuilder.ToString();
            if (signData.Length > 0)
            {
                signData = signData.Remove(signData.Length - 1, 1); // Bỏ ký tự & cuối
            }

            _logger.LogInformation("SignData for hash: {SignData}", signData);

            // Tính hash giống VnPayLibrary - key trước, data sau
            var secureHash = HmacSHA512(_vnpHashSecret, signData);

            _logger.LogInformation("Generated SecureHash: {SecureHash}", secureHash);

            // Tạo final URL
            var finalUrl = $"{_vnpUrl}?{signData}&vnp_SecureHash={secureHash}";

            _logger.LogInformation("Final VNPay URL: {FinalUrl}", finalUrl);

            return finalUrl;
        }

        // Sử dụng method HmacSHA512 giống VnPayLibrary
        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }

        // Method để validate signature giống VnPayLibrary
        private bool ValidateVNPaySignature(Dictionary<string, string> vnpParams, string inputHash)
        {
            // Chuyển đổi sang SortedList với VnPayCompare
            var sortedParams = new SortedList<string, string>(new VnPayCompare());
            foreach (var kvp in vnpParams)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    sortedParams.Add(kvp.Key, kvp.Value);
                }
            }

            // Tạo signData để verify
            var signDataBuilder = new StringBuilder();
            foreach (var kvp in sortedParams.Where(kv => !string.IsNullOrEmpty(kv.Value)))
            {
                signDataBuilder.Append($"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}&");
            }

            var signData = signDataBuilder.ToString();
            if (signData.Length > 0)
            {
                signData = signData.Remove(signData.Length - 1, 1);
            }

            _logger.LogInformation("SignData for verification: {SignData}", signData);

            var calculatedHash = HmacSHA512(_vnpHashSecret, signData);

            _logger.LogInformation("Received hash: {ReceivedHash}", inputHash);
            _logger.LogInformation("Calculated hash: {CalculatedHash}", calculatedHash);

            return calculatedHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
                return "127.0.0.1";
            return ipAddress;
        }

        private string GetVNPayErrorMessage(string responseCode)
        {
            return responseCode switch
            {
                "00" => "Giao dịch thành công",
                "07" => "Giao dịch bị nghi ngờ gian lận",
                "09" => "Thẻ/Tài khoản chưa đăng ký dịch vụ",
                "10" => "Không xác minh được thông tin thẻ/tài khoản",
                "11" => "Chưa qua kiểm tra rủi ro",
                "12" => "Thẻ/Tài khoản bị khóa",
                "13" => "Quá hạn mức giao dịch",
                "24" => "Giao dịch bị hủy bởi người dùng",
                "51" => "Tài khoản không đủ số dư",
                "65" => "Quá hạn mức giao dịch trong ngày",
                "75" => "Ngân hàng từ chối giao dịch",
                "79" => "Nhập sai mật khẩu quá số lần cho phép",
                "99" => "Lỗi không xác định từ VNPay",
                _ => $"Lỗi không xác định: {responseCode}"
            };
        }
    }

    // Thêm class VnPayCompare giống như trong VnPayLibrary
    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}