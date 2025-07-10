using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class HomestayListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string WardName { get; set; }  
        public bool Status { get; set; }
        public string ThumbnailUrl { get; set; } // Nếu có ảnh đại diện
    }
}

