using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Homestays
{
    public class HomestayNeighbourhood
    {
        [Key]
        public int HomestayId { get; set; }
        [Key]

        public int NeighbourhoodId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(HomestayId))]
        public Homestay Homestay { get; set; }
        [ForeignKey("NeighbourhoodId")]
        public Neighbourhood Neighbourhood { get; set; }
    }
}

