using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class HomestayListDTO
    {
        public string name { get; set; }
        
        public string Rules { get; set; }        // Tên loại phòng đầu tiên (nếu có)
        public string streetAddress { get; set; }
        public bool Status { get; set; }
        public string ImageUrl { get; set; } // Nếu có ảnh đại diện
    }
}

