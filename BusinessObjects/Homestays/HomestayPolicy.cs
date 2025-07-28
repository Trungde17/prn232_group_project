using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Homestays
{
    public class HomestayPolicy
    {
        [Key, Column(Order = 0)]
        public int HomestayId { get; set; }

        [Key, Column(Order = 1)]
        public int PolicyId { get; set; }

        public bool IsAllowed { get; set; }
        [ForeignKey("HomestayId")]
        public Homestay Homestay { get; set; }
        [ForeignKey("PolicyId")]

        public Policy Policy { get; set; }
    }

}
