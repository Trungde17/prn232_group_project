using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Homestays
{
    public class FavoriteHomestay
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [Key, Column(Order = 1)]
        public int HomestayId { get; set; }
        [ForeignKey("HomestayId")]
        public Homestay Homestay { get; set; }

        public DateTime FavoritedAt { get; set; } = DateTime.UtcNow;
    }
}
