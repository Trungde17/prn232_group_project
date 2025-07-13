using BusinessObjects.Homestays;
using System.ComponentModel.DataAnnotations;
namespace BusinessObjects
{
    public class Ward
    {
        [Key]
        public int WardId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public int DistrictId { get; set; }
        public District District { get; set; }

        public ICollection<Homestay> Homestays { get; set; }
    }
}
