using BusinessObjects.Bookings;
using BusinessObjects.Rooms;
using System.ComponentModel.DataAnnotations;
namespace BusinessObjects.Homestays
{
    public class Homestay
    {
        [Key]
        public int HomestayId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public int HomestayTypeId { get; set; }
        public HomestayType HomestayType { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(255)]
        public string StreetAddress { get; set; }  // Số nhà, tên đường
        [Required]
        public int WardId { get; set; }

        public Ward Ward { get; set; }

        [MaxLength(10000)]
        public string Rules { get; set; }
        [Required]
        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool Status { get; set; } = true;
        // Navigation Properties
        public ICollection<HomestayAmenity> HomestayAmenities { get; set; }
        public ICollection<HomestayImage> HomestayImages { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<HomestayPolicy> HomestayPolicies { get; set; }
        public ICollection<HomestayNeighbourhood> HomestayNeighbourhoods { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
