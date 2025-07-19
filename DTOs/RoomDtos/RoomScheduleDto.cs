using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace DTOs.RoomDtos
{
    public class RoomScheduleDto
    {
        public int? ScheduleId { get; set; }

       
        public int? RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ScheduleType ScheduleType { get; set; } = ScheduleType.Booking;
    }
}
