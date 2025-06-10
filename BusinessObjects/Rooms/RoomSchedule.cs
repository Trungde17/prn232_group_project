using BusinessObjects.Enums;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Rooms
{
    public class RoomSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int RoomId { get; set; }

        public Room Room { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required]
        public ScheduleType ScheduleType { get; set; } = ScheduleType.Booking;
    }
}
