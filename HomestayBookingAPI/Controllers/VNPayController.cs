using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace HomestayBookingProject.Controllers
{
    [Route("api/payments/vnpay")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        private readonly string _vnpHashSecret;

        public VNPayController(IConfiguration configuration)
        {
            _vnpHashSecret = configuration["VNPay:HashSecret"] ?? throw new ArgumentNullException("VNPay:HashSecret is not configured");
        }

        [HttpGet("ipn")]
        public IActionResult HandleVNPayIPN([FromQuery] Dictionary<string, string> vnpParams)
        {
            try
            {
                // Lấy secureHash từ query parameters
                if (!vnpParams.TryGetValue("vnp_SecureHash", out var secureHash) || string.IsNullOrEmpty(secureHash))
                {
                    return BadRequest(new { message = "Missing or invalid vnp_SecureHash" });
                }

                // Lấy vnp_ResponseCode và vnp_TxnRef
                vnpParams.TryGetValue("vnp_ResponseCode", out var responseCode);
                vnpParams.TryGetValue("vnp_TxnRef", out var txnRef);

                // Xóa vnp_SecureHash khỏi params để tính toán chữ ký
                vnpParams.Remove("vnp_SecureHash");

                // Tạo chuỗi ký tự để xác minh chữ ký
                var signData = string.Join("&", vnpParams.OrderBy(k => k.Key)
                    .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

                // Tính toán chữ ký HMAC-SHA512
                var computedHash = HmacSha512(signData, _vnpHashSecret);

                // Xác minh chữ ký
                if (!secureHash.Equals(computedHash, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "Invalid signature" });
                }

                // Ghi log kết quả
                var response = new
                {
                    vnp_TxnRef = txnRef,
                    vnp_ResponseCode = responseCode,
                    isSuccess = responseCode == "00",
                    message = GetResponseMessage(responseCode)
                };

                Console.WriteLine($"VNPay IPN received. TxnRef: {txnRef}, ResponseCode: {responseCode}");

                // Redirect về deep link cho ứng dụng Android
                var redirectUrl = responseCode == "00"
                    ? $"homestaybooking://success?txnRef={txnRef}&responseCode={responseCode}"
                    : $"homestaybooking://error?txnRef={txnRef}&responseCode={responseCode}";

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing VNPay IPN: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        private string HmacSha512(string input, string key)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private string GetResponseMessage(string responseCode)
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
}