using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Rooms
{
    public class RoomPrice
    {

        public int RoomId { get; set; }


        public int PriceTypeId { get; set; }

        [Required]
        public decimal AmountPerNight { get; set; }

        public Room Room { get; set; }
        public PriceType PriceType { get; set; }
    }
}
