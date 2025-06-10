using BusinessObjects.Homestays;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // Ví dụ: "Cho phép hút thuốc", "Mang theo thú cưng"

        public ICollection<HomestayPolicy> HomestayPolicies { get; set; }
    }
}
