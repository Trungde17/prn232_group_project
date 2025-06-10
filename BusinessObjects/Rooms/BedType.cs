using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Rooms
{
    public class BedType
    {
        [Key]
        public int BedTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<RoomBed> RoomBeds { get; set; }
    }
}
