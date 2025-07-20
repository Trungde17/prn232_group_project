using Microsoft.AspNetCore.Mvc;

namespace HomestayBookingAPI.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        [HttpPost("vnpay/ipn")]
        public IActionResult VNPayIPN([FromForm] VNPayIPNModel model)
        {
            // Log để debug
            Console.WriteLine("VNPay IPN received:");
            Console.WriteLine($"Response Code: {model.vnp_ResponseCode}");
            Console.WriteLine($"Transaction Ref: {model.vnp_TxnRef}");
            Console.WriteLine($"Amount: {model.vnp_Amount}");

            // Tạm thời return success để VNPay không retry
            return Ok(new { RspCode = "00", Message = "Confirm Success" });
        }
    }

    [Route("payment")]
    public class PaymentReturnController : ControllerBase
    {
        [HttpGet("return")]
        public IActionResult PaymentReturn([FromQuery] VNPayReturnModel model)
        {
            // Log để debug
            Console.WriteLine("VNPay Return received:");
            Console.WriteLine($"Response Code: {model.vnp_ResponseCode}");

            // Return simple HTML page
            return Content(@"
            <html>
                <body>
                    <h1>Payment Completed</h1>
                    <p>You can close this page</p>
                    <script>
                        setTimeout(function() {
                            window.close();
                        }, 3000);
                    </script>
                </body>
            </html>", "text/html");
        }
    }

    // Models
    public class VNPayIPNModel
    {
        public string vnp_TmnCode { get; set; }
        public string vnp_Amount { get; set; }
        public string vnp_BankCode { get; set; }
        public string vnp_ResponseCode { get; set; }
        public string vnp_TxnRef { get; set; }
        public string vnp_TransactionNo { get; set; }
        public string vnp_SecureHash { get; set; }
    }

    public class VNPayReturnModel
    {
        public string vnp_ResponseCode { get; set; }
        public string vnp_TxnRef { get; set; }
        public string vnp_Amount { get; set; }
    }
}
