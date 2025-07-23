namespace DTOs.RoomDtos
{
    public class GetRoomForBookingResponseDTO
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public int HomestayId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public int Capacity { get; set; }
        public double Size { get; set; }
        public List<string> Amenities { get; set; }
        public List<string> RoomBeds { get; set; }
        public bool IsAvailable { get; set; }
    }
}
