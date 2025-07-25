using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace DTOs.Bookings
{
    public class BookingStatusUpdateDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingStatus Status { get; set; }
    }
}
