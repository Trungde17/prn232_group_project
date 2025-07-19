using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class HomestayDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string WardName { get; set; }
        public string DistrictName { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrls { get; set; }
        public bool Status { get; set; }
        public double PricePerNight { get; set; }
        public int MaxGuest { get; set; }
        public string HostName { get; set; }
        public string HostPhone { get; set; }
    }
}

