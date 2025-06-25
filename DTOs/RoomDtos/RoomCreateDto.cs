using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.RoomDtos
{
    public class RoomCreateDto
    {
        public string Name { get; set; }
        public int HomestayId { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
        public int Capacity { get; set; }
        public double Size { get; set; }

        public List<RoomBedDto> RoomBeds { get; set; }
        public List<RoomPriceDto> RoomPrices { get; set; }
        public List<RoomAmenityDto> RoomAmenities { get; set; }
        public List<RoomScheduleDto> RoomSchedules { get; set; }
    }
}
