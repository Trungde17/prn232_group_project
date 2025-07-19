using System.ComponentModel.DataAnnotations;

namespace DTOs.Bookings
{
    public class CreateBookingDTO
    {
        [Required]
        public int HomestayId { get; set; }
        public DateTime DateBooked { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime DateCheckIn { get; set; }

        [Required]
        public DateTime DateCheckOut { get; set; }
        [Required]
        public List<int> RoomIds { get; set; }
    }
}
