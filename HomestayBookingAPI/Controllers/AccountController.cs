using BusinessObjects;
using BusinessObjects.Enums;
using DTOs;
using DTOs.UserDtos;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace HomestayBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            RoleManager<IdentityRole> roleManager,
                            IConfiguration config,
                            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _emailSender = emailSender;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized("Email does not exist.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)

            {
                return Unauthorized("Password is incorrect.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.LastName +" "+ user.FirstName),

                };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            claims.AddRange(userClaims);
            // Thêm role, claims khác...

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                role = roles[0]
            });
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            // 1. Gán role mặc định nếu không truyền
            var requestedRole = string.IsNullOrWhiteSpace(dto.Role) ? "Customer" : dto.Role;
            // 2. Kiểm tra role có tồn tại trong hệ thống hay chưa
            var roleExists = await _roleManager.RoleExistsAsync(requestedRole);
            if (!roleExists)
            {
                return BadRequest($"Role '{requestedRole}' does not exist in the system.");
            }
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                EmailConfirmed = false

            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            // 5. Gán vai trò
            var addRoleResult = await _userManager.AddToRoleAsync(user, requestedRole);
            if (!addRoleResult.Succeeded)
            {
                return BadRequest(addRoleResult.Errors);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var callbackUrl = $"{_config["App:ClientUrl"]}/confirm-email?userId={user.Id}&token={encodedToken}";
            // Gửi email
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>Confirm Email</a>");

            return Ok(new
            {
                message = "Registration successful. Please check your email to confirm your account.",
                userId = user.Id,
                token = encodedToken,
            });
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("Missing userId or token.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var decodedToken = WebUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
                return Ok("Email confirmed successfully. You can now log in.");
            else
                return BadRequest("Invalid or expired confirmation link.");
        }

        private static Dictionary<string, string> _resetCodes = new(); // Key: Email, Value: Code

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Ok("If your email is registered and confirmed, a reset code has been sent.");

            // Tạo mã ngẫu nhiên 6 chữ số
            var code = new Random().Next(100000, 999999).ToString();
            _resetCodes[user.Email] = code;

            // Gửi email
            await _emailSender.SendEmailAsync(user.Email, "Reset Code",
                $"Your password reset code is: <b>{code}</b>. This code will expire in a few minutes.");

            return Ok("Reset code sent to your email.");
        }
        [HttpPost("reset-password-with-code")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordWithCode([FromBody] ResetPasswordWithCodeDto dto)
        {
            if (!_resetCodes.TryGetValue(dto.Email, out var storedCode) || storedCode != dto.Code)
            {
                return BadRequest("Invalid or expired reset code.");
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            _resetCodes.Remove(dto.Email); // Xoá mã sau khi sử dụng
            return Ok("Password has been reset successfully.");
        }

        //[HttpPost("reset-password")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        //{
        //    var user = await _userManager.FindByIdAsync(dto.UserId);
        //    if (user == null)
        //        return NotFound("User not found.");

        //    var decodedToken = WebUtility.UrlDecode(dto.Token);
        //    var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
        //    if (!result.Succeeded)
        //        return BadRequest(result.Errors);

        //    return Ok("Password has been reset successfully.");
        //}
        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var payload = await VerifyGoogleToken(dto);
            if (payload == null)
            {
                return BadRequest("Invalid Google token");
            }

            // Kiểm tra người dùng có tồn tại chưa
            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    EmailConfirmed = true // Google đã xác minh email
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                // Gán role mặc định
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            // Tạo JWT
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName)
    };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        //https://developers.google.com/oauthplayground
        private async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(GoogleLoginDto dto)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _config["Authentication:Google:ClientId"] }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logged out successfully.");
        }

        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetUsers([FromQuery] string? email, [FromQuery] string? name)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => u.Email.Contains(email));

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(u => u.FirstName.Contains(name) || u.LastName.Contains(name) || u.UserName.Contains(name));

            var userList = await query.ToListAsync();

            var users = new List<UserDto>();

            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);

                users.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    Role = roles?.FirstOrDefault()
                });
            }

            return Ok(users);
        }

        // ... existing code ...

        [HttpPost("ban/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")] // Restrict to admins
        public async Task<IActionResult> BanUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Check if user has Customer role
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("Admin"))
            {
                return BadRequest("Only users withouth admin role can be banned.");
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            if (result.Succeeded)
            {
                return Ok(new { message = $"Customer {user.Email} has been banned." });
            }
            else
            {
                return BadRequest("Failed to ban user.");
            }
        }

        [HttpPost("unban/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")] // Restrict to admins
        public async Task<IActionResult> UnbanUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Check if user has Customer role
            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains("Customer"))
            {
                return BadRequest("Only users with Customer role can be unbanned.");
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (result.Succeeded)
            {
                return Ok(new { message = $"Customer {user.Email} has been unbanned." });
            }
            else
            {
                return BadRequest("Failed to unban user.");
            }
        }



        [HttpPost("create-owner")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> CreateOwner([FromBody] CreateOwnerRequestDto dto)
        {

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Gender = dto.Gender ?? GenderType.Other,
                DateOfBirth = dto.DateOfBirth,
                //AvatarUrl = dto.AvatarUrl,
                EmailConfirmed = true,
                CreatAt = DateTime.UtcNow
            };
            var password = GeneratePassword();
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, "Owner");
            if (!addRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(addRoleResult.Errors);
            }

            try
            {
                //await _emailSender.SendEmailAsync(user.Email, "Welcome to Homestay Platform",
                //    $"Hello {user.FirstName} {user.LastName},<br/>" +
                //    $"Your Owner account has been created successfully.<br/>" +
                //    $"Email: {user.Email}<br/>" +
                //    $"You can now log in to manage your homestays.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Email error: {ex}");
            }

            return Ok(new
            {

                userId = user.Id,

            });
        }
        private string GeneratePassword(int length = 12)
        {
            string LowerCase = "abcdefghijklmnopqrstuvwxyz";
            string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string Numbers = "0123456789";
            string Symbols = "!@#$%^&*";
            if (length < 8)
                length = 8; // Minimum password length

            var allChars = LowerCase + UpperCase + Numbers + Symbols;
            var password = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                // Ensure at least one character from each category
                password.Append(GetRandomChar(LowerCase, rng));
                password.Append(GetRandomChar(UpperCase, rng));
                password.Append(GetRandomChar(Numbers, rng));
                password.Append(GetRandomChar(Symbols, rng));

                // Fill the rest with random characters
                for (int i = 4; i < length; i++)
                {
                    password.Append(GetRandomChar(allChars, rng));
                }
            }

            // Shuffle the password to avoid predictable patterns
            return ShuffleString(password.ToString());
        }
        private static char GetRandomChar(string chars, RandomNumberGenerator rng)
        {
            byte[] randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            int randomValue = Math.Abs(BitConverter.ToInt32(randomBytes, 0));
            return chars[randomValue % chars.Length];
        }

        private static string ShuffleString(string input)
        {
            char[] array = input.ToCharArray();
            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = array.Length - 1; i > 0; i--)
                {
                    byte[] randomBytes = new byte[4];
                    rng.GetBytes(randomBytes);
                    int randomIndex = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % (i + 1);

                    // Swap elements
                    (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
                }
            }
            return new string(array);
        }
    }
}
