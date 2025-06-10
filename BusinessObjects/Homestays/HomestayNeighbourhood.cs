namespace BusinessObjects.Homestays
{
    public class HomestayNeighbourhood
    {
        public int HomestayId { get; set; }

        public int NeighbourhoodId { get; set; }

        // Navigation properties
        public Homestay Homestay { get; set; }
        public Neighbourhood Neighbourhood { get; set; }
    }
}

