using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Homestays
{
    public class HomestayAmenity
    {
        [Required]
        [Key]
        public int HomestayId { get; set; }
        [Key]
        [Required]
        public int AmenityId { get; set; }

        [ForeignKey("HomestayId")]
        public Homestay Homestay { get; set; }

        [ForeignKey("AmenityId")]
        public Amenity Amenity { get; set; }
    }
}
