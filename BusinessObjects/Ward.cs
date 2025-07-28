using BusinessObjects.Homestays;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("DistrictId")]
        public District District { get; set; }
        [InverseProperty("Ward")]
        public ICollection<Homestay> Homestays { get; set; }
    }
}
