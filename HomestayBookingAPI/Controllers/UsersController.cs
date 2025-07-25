using BusinessObjects;
using BusinessObjects.Enums;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("test")]
        [Authorize]
        public IActionResult TestAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            
            return Ok(new
            {
                message = "JWT Authentication working!",
                userId = userId,
                email = email,
                roles = roles
            });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                phoneNumber = user.PhoneNumber,
                emailConfirmed = user.EmailConfirmed,
                role = roles.FirstOrDefault()
            });
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Cập nhật thông tin user (chỉ sử dụng các field cần thiết)
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber ?? string.Empty;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new
                {
                    message = "Profile updated successfully.",
                    user = new
                    {
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        phoneNumber = user.PhoneNumber
                    }
                });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Kiểm tra mật khẩu hiện tại
            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);
            if (!checkPasswordResult)
                return BadRequest("Current password is incorrect.");

            // Đổi mật khẩu
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        // PUT: api/Users
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto request)
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
