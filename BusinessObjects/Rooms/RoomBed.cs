using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BusinessObjects.Rooms
{
    public class RoomBed
    {
        [Key,Column(Order =0)]
        public int RoomId { get; set; }

        [Key,Column(Order = 1)]
        public int BedTypeId { get; set; }

        public int Quantity { get; set; }

        // Navigation
        [ForeignKey("RoomId")]
        public Room Room { get; set; }
        [ForeignKey("BedTypeId")]

        public BedType BedType { get; set; }
    }
}
