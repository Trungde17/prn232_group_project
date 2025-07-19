using BusinessObjects.Enums;
using BusinessObjects.Homestays;
using BusinessObjects.Rooms;
using System.ComponentModel.DataAnnotations;
namespace BusinessObjects
{
    public class Amenity
    {
        [Key]
        public int AmenityId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public AmenityType? Type { get; set; }

        // Navigation
        public ICollection<HomestayAmenity> HomestayAmenities { get; set; }
        public ICollection<RoomAmenity> RoomAmenities { get; set; }
    }
}
