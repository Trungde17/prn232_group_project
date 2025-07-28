using System.ComponentModel.DataAnnotations;

namespace DTOs.HomestayDtos
{
    public class CreateHomestayDTO
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [Required]
        public int HomestayTypeId { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Required]
        [StringLength(255)]
        public string StreetAddress { get; set; }
        [Required]
        public int WardId { get; set; }

        [MaxLength(10000)]
        public string Rules { get; set; }
        [Required]
        public string OwnerId { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<int> NeighbourhoodIds { get; set; }
        public List<int> AmenityIds { get; set; }
        public List<HomestayPolicyDto> HomestayPolicies { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class HomestayPolicyDto
    {
        public int PolicyId { get; set; }
        public bool IsAllow
        {
            get; set;
        }
    }
}
