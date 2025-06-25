using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace DTOs.RoomDtos
{
    public class RoomScheduleDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ScheduleType ScheduleType { get; set; } = ScheduleType.Booking;
    }
}
