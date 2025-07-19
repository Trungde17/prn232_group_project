using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class UpdateUserRequest
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Range(0, 2, ErrorMessage = "Gender must be 0 (Male), 1 (Female), or 2 (Other)")]
        public int Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(250)]
        public string? AvatarUrl { get; set; }
    }
}
