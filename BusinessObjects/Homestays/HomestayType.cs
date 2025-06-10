using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Homestays
{
    public class HomestayType
    {
        [Key]
        public int HomestayTypeId { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Homestay Type Name")]
        public string TypeName { get; set; }

        public ICollection<Homestay> Homestays { get; set; }
    }
}
