namespace DTOs.Payments
{
    public class ConfirmPaymentResponse
    {
        public bool Success { get; set; }
        public int BookingId { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }
}
