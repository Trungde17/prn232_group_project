using BusinessObjects.Enums;
using BusinessObjects.Homestays;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BusinessObjects.Bookings
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
       
        [Required]
        public string CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; }

        [Required]
        public int HomestayId { get; set; }
        [ForeignKey("HomestayId")]
        public Homestay Homestay { get; set; }

        [Required]
        public DateTime DateBooked { get; set; }

        [Required]
        public DateTime DateCheckIn { get; set; }

        [Required]
        public DateTime DateCheckOut { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        public ICollection<BookingDetail> BookingDetails { get; set; }

    }
}
