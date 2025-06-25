using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.RoomDtos
{
    public class RoomPriceDto
    {
        public int PriceTypeId { get; set; }

       
        public decimal AmountPerNight { get; set; }
    }
}
