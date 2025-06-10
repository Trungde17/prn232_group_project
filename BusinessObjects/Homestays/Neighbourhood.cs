using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Homestays
{
    public class Neighbourhood
    {
        [Key]
        [Column("neighbourhood_id")]
        public int NeighbourhoodId { get; set; }

        [Required]
        [Column("neighbourhood_name")]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation property
        public ICollection<HomestayNeighbourhood> HomestayNeighbourhoods { get; set; }
    }
}
