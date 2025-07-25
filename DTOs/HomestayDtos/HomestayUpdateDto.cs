using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.HomestayDtos
{
    public class HomestayUpdateDto
    {
        public string Name { get; set; }
        public int HomestayTypeId { get; set; }
        public string Description { get; set; }
        public string StreetAddress { get; set; }
        public int WardId { get; set; }
        public string Rules { get; set; }
        public string OwnerId { get; set; }
        public bool Status { get; set; }

        public List<HomestayAmenityDto> Amenities { get; set; }
        public List<HomestayImageDto> Images { get; set; }
        public List<PolicyDto> Policies { get; set; }
        public List<HomestayNeighbourhoodDto> Neighbourhoods { get; set; }
    }
}
