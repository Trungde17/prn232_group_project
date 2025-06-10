using BusinessObjects.Rooms;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Bookings
{
    public class BookingDetail
    {
        [Key]
        public int BookingDetailId { get; set; }
        [Required]
        public int BookingId { get; set; }

        public Booking Booking { get; set; }
        [Required]
        public int RoomId { get; set; }

        public Room Room { get; set; }
    }
}
