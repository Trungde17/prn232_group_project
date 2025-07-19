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

        [Required]
        [StringLength(255)]
        public string StreetAddress { get; set; }
        [Required]
        public int WardId { get; set; }

        [MaxLength(10000)]
        public string Rules { get; set; }
        [Required]
        public string OwnerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
