using AutoMapper;
using BusinessObjects.Homestays;
using DTOs.FavoriteHomestay;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;

namespace HomestayBookingAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FavoriteHomestayController : ControllerBase
    {
        private readonly IFavoriteHomestayService _favoriteHomestayService;
        private readonly IMapper _mapper;
        public FavoriteHomestayController(IFavoriteHomestayService favoriteHomestayService, IMapper mapper)
        {
            _favoriteHomestayService = favoriteHomestayService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }
                var favoriteHomestays = (await _favoriteHomestayService.GetAsync(userId)) ?? new List<FavoriteHomestay>();
                return Ok(favoriteHomestays);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateFavoriteHomestayDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }
            try
            {
                var favoriteHomestay = new FavoriteHomestay
                {
                    HomestayId = dto.HomestayId,
                    UserId = userId
                };
                var result = await _favoriteHomestayService.CreateAsync(favoriteHomestay);
                return CreatedAtAction(nameof(Get), new { userId = result.UserId }, dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{homestayId}")]
        public async Task<IActionResult> Delete(int homestayId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }
            if (homestayId <= 0)
            {
                return BadRequest("Invalid homestay ID or user ID.");
            }

            try
            {
                var favoriteHomestay = await _favoriteHomestayService.GetAsync(homestayId, userId);
                if (favoriteHomestay == null)
                {
                    return NotFound("Favorite homestay not found.");
                }

                await _favoriteHomestayService.DeleteAsync(favoriteHomestay);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
