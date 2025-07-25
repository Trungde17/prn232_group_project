namespace DTOs
{
    public class HomestaySearchDTO
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }

        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }
}
