using AutoMapper;
using BusinessObjects.Bookings;
using BusinessObjects.Enums;
using DTOs.Bookings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services.BookingServices;
using Services.HomestayServices;
using Services.RoomServices;
using System.Security.Claims;
namespace HomestayBookingAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IHomestayService _homestayService;
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        public BookingController(IBookingService bookingService, IMapper mapper, IRoomService roomService, IHomestayService homestayService)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _homestayService = homestayService;
            _mapper = mapper;
        }
        [EnableQuery]
        [HttpGet("/my-booking")]
        public async Task<ActionResult> Get()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var bookings = await _bookingService.GetMyBookingAsync(userId);
                return Ok(bookings ?? new List<Booking>());
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving bookings.");
            }

        }

        


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateBookingDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token.");

                if (!(await _homestayService.CheckValidHomestay(dto.HomestayId)))
                    return BadRequest($"Invalid HomestayId {dto.HomestayId}. Homestay does not exist.");

                var invalidRoomIds = await _roomService.CheckRoomsInHomestayAsync(dto.RoomIds, dto.HomestayId);
                if (invalidRoomIds != null && invalidRoomIds.Count > 0)
                    return BadRequest($"The following RoomIds do not belong to HomestayId {dto.HomestayId} or do not exist: {string.Join(", ", invalidRoomIds)}.");

                var unavailableRoomIds = await _bookingService.CheckRoomAvailabilityAsync(dto.RoomIds, dto.DateCheckIn, dto.DateCheckOut);
                if (unavailableRoomIds != null && unavailableRoomIds.Count > 0)
                    return BadRequest($"The following RoomIds are unavailable for the selected dates: {string.Join(", ", unavailableRoomIds)}.");

                var booking = _mapper.Map<Booking>(dto);
                booking.CustomerId = "71184f16-d47d-4aae-9bf3-f94d49ea91f8";
                booking.TotalAmount = await _bookingService.CalculateTotalAmountAsync(dto.RoomIds, dto.DateCheckIn, dto.DateCheckOut);
                var result = await _bookingService.CreateBookingAsync(booking, dto.RoomIds);
                if (result == null)
                    return StatusCode(500, "Failed to create booking.");
                // Return 201 Created with booking URL
                return CreatedAtAction(nameof(Get), new { bookingId = result.BookingId });

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving bookings.");
            }

        }
        [HttpPut("({key})/status")]
        public async Task<IActionResult> UpdateStatus([FromODataUri] int key, [FromBody] BookingStatus status)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedBooking = await _bookingService.UpdateBookingStatusAsync(key, status);
            if (updatedBooking == null)
                return NotFound();

            return Ok(updatedBooking);
        }
    }
}
