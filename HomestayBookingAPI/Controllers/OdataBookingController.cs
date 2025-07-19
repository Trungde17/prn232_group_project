using System.Security.Claims;
using AutoMapper;
using BusinessObjects.Bookings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services.BookingServices;
using Services.HomestayServices;
using Services.RoomServices;

namespace HomestayBookingAPI.Controllers
{
   
    public class OdataBookingController : ODataController
    {
        private readonly IBookingService _bookingService;
        private readonly IHomestayService _homestayService;
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public OdataBookingController(IBookingService bookingService, IMapper mapper, IRoomService roomService, IHomestayService homestayService)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _homestayService = homestayService;
            _mapper = mapper;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                var bookings = await _bookingService.GetAllAsync();
                return Ok(bookings ?? new List<Booking>());
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving bookings.");
            }
        }

        [EnableQuery]
        [HttpGet("({key})")]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var booking = await _bookingService.GetByIdAsync(key);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }


        [HttpPut("({key})")]
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] Booking updatedBooking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booking = await _bookingService.UpdateAsync(key, updatedBooking);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        [HttpPatch("({key})")]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<Booking> patch)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booking = await _bookingService.GetByIdAsync(key);
            if (booking == null)
                return NotFound();

            patch.Patch(booking);
            var updated = await _bookingService.UpdateAsync(key, booking);

            return Ok(updated);
        }

        [HttpDelete("({key})")]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var deleted = await _bookingService.DeleteAsync(key);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
