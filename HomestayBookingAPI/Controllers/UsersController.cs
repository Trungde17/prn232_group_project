using BusinessObjects;
using BusinessObjects.Enums;
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomestayBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // PUT: api/Users
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Gender = (GenderType)request.Gender;
            user.DateOfBirth = request.DateOfBirth;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.AvatarUrl = request.AvatarUrl;
            user.UpdateAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Update failed.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return Ok(new
            {
                message = "User updated successfully.",
                user = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Gender,
                    user.DateOfBirth,
                    user.PhoneNumber,
                    user.Address,
                    user.AvatarUrl,
                    user.UpdateAt
                }
            });
        }
    }
}
