using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Rooms
{
    public class RoomAmenity
    {
        [Key] 
        public int RoomId { get; set; }
        [Key]

        public int AmenityId { get; set; }

        public Room? Room { get; set; }
        public Amenity? Amenity { get; set; }
    }
}
