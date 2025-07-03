using DTOs.HomestayDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services.HomestayServices;


namespace HomestayBookingAPI.Controllers
{

    public class HomestaysController : ODataController
    {
        private readonly IHomestayService _homestayService;

        public HomestaysController(IHomestayService homestayService)
        {
            _homestayService = homestayService;
        }

        // GET: odata/Homestays
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var homestays = await _homestayService.GetAllHomestaysAsync();
            return Ok(homestays);
        }

        // GET: odata/Homestays(5)
        [EnableQuery]
        [HttpGet("({key})")]

        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var homestay = await _homestayService.GetHomestayByIdAsync(key);
            if (homestay == null)
                return NotFound();

            return Ok(homestay);
        }

        [HttpGet("{key}/bookings")]

        public async Task<IActionResult> GetListBooking([FromODataUri] int key)
        {
            var homestayBooking = await _homestayService.GetHomestayByIdAsync(key);
            if (homestayBooking == null)
                return NotFound();

            return Ok(homestayBooking);
        }
        // PUT: odata/Homestays(5)
        [HttpPut("({key})")]
        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] HomestayUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _homestayService.UpdateHomestayAsync(key, dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
    }
}
