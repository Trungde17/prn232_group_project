namespace DTOs.Payments
{
    public class CreatePaymentRequest
    {
        public int BookingId { get; set; }
        public int HomestayId { get; set; }
        public string HomestayName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public decimal Amount { get; set; }
        public List<int> RoomIds { get; set; }
    }
}
