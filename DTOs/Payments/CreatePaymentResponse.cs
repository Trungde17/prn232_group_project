namespace DTOs.Payments
{
    public class CreatePaymentResponse
    {
        public bool Success { get; set; }
        public string PaymentUrl { get; set; }
        public int BookingId { get; set; }
        public string Message { get; set; }
    }
}
