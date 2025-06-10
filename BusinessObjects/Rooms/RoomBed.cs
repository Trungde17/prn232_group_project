namespace BusinessObjects.Rooms
{
    public class RoomBed
    {

        public int RoomId { get; set; }


        public int BedTypeId { get; set; }

        public int Quantity { get; set; }

        // Navigation
        public Room Room { get; set; }
        public BedType BedType { get; set; }
    }
}
