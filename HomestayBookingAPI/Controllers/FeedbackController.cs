using BusinessObjects;
using DataAccess;
using DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomestayBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly HomestayDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(HomestayDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Lấy tất cả feedback
        [HttpGet]
        public IActionResult GetAll()
        {
            var feedbacks = _context.Feedbacks
                .Select(f => new FeedbackResponseDto
                {
                    FeedbackId = f.FeedbackId,
                    HomestayId = f.HomestayId,
                    CustomerId = f.CustomerId,
                    CustomerName = f.Customer != null ? f.Customer.UserName : null,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                }).ToList();

            return Ok(feedbacks);
        }

        // Lấy 1 feedback theo id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var f = await _context.Feedbacks.FindAsync(id);
            if (f == null) return NotFound("Feedback not found.");

            var customer = await _userManager.FindByIdAsync(f.CustomerId);
            var dto = new FeedbackResponseDto
            {
                FeedbackId = f.FeedbackId,
                HomestayId = f.HomestayId,
                CustomerId = f.CustomerId,
                CustomerName = customer?.UserName,
                Rating = f.Rating,
                Comment = f.Comment,
                CreatedAt = f.CreatedAt
            };

            return Ok(dto);
        }

        // Tạo mới feedback
        [HttpPost]
        //[Authorize]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromBody] FeedbackCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var feedback = new Feedback
            {
                HomestayId = dto.HomestayId,
                CustomerId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Feedback submitted successfully.",
                feedbackId = feedback.FeedbackId
            });
        }

        // Cập nhật feedback
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] FeedbackUpdateDto dto)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound("Feedback not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (feedback.CustomerId != userId) return Forbid("You can only update your own feedback.");

            feedback.Rating = dto.Rating;
            feedback.Comment = dto.Comment;
            feedback.UpdateAt = DateTime.UtcNow;

            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();

            return Ok("Feedback updated successfully.");
        }

        // Xóa feedback
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound("Feedback not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (feedback.CustomerId != userId) return Forbid("You can only delete your own feedback.");

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return Ok("Feedback deleted successfully.");
        }
    }
}
