using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Rooms
{
    public class PriceType
    {
        [Key]
        public int PriceTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; }

        public ICollection<RoomPrice> RoomPrices { get; set; }
    }
}
