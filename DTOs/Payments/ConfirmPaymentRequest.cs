namespace DTOs.Payments
{
    public class ConfirmPaymentRequest
    {
        public int BookingId { get; set; }
        public Dictionary<string, string> VnpayParams { get; set; }
    }
}
