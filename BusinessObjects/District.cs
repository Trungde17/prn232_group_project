using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class District
    {
        [Key]
        public int DistrictId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // VD: "Hải Châu"

        public ICollection<Ward> Wards { get; set; }
    }
}
