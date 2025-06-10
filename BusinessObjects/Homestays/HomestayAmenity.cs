namespace BusinessObjects.Homestays
{
    public class HomestayAmenity
    {
        public int HomestayId { get; set; }

        public int AmenityId { get; set; }
        public Homestay Homestay { get; set; }
        public Amenity Amenity { get; set; }
    }
}
