using BusinessObjects.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class CreateOwnerAccountDTO
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        public GenderType Gender { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }
        [Required]
        [Display(Name = "Avatar Image URL")]
        [StringLength(250)]
        public string? AvatarUrl { get; set; }
    }
}
