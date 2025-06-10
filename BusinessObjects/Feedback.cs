using BusinessObjects.Homestays;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int HomestayId { get; set; }
        public Homestay Homestay { get; set; }

        [Required]
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } // 1 đến 5 sao

        [StringLength(1000)]
        public string Comment { get; set; }  // nhận xét văn bản
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; }
    }
}
