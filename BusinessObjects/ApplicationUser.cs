using BusinessObjects.Bookings;
using BusinessObjects.Enums;
using BusinessObjects.Homestays;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace BusinessObjects
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public GenderType Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public override string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Create Date")]
        public DateTime? CreatAt { get; set; }

        [Display(Name = "Update Date")]
        public DateTime? UpdateAt { get; set; }

        [Display(Name = "Avatar Image URL")]
        [StringLength(250)]
        public string? AvatarUrl { get; set; }

        public ICollection<Homestay> Homestays { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Homestay> FavoriteHomestays { get; set; }
    }
}
/*
 * customer 1:
   - gmail: hungtran0508@gmail.com
   - mk: Hungtran01@
 * owner 1:
    - gmail: trungnguyenowner01@gmail.com
    - mk: Trungnguyen01@
 
 */