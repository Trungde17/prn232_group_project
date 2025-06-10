using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Homestays
{
    public class HomestayImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public int HomestayId { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }

        // Navigation property (tuỳ chọn)
        public Homestay Homestay { get; set; }
    }
}
