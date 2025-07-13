using AutoMapper;
using BusinessObjects;
using BusinessObjects.Homestays;
using DTOs.HomestayDtos;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public HomestaysController(IHomestayService homestayService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _homestayService = homestayService;
            _userManager = userManager;
            _mapper = mapper;
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

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var homestay = await _homestayService.GetHomestayByIdAsync(id);
            if (homestay == null)
                return NotFound();
            var response = _mapper.Map<GetHomestayDetailDTO>(homestay);
            return Ok(response);
        }

        [HttpGet("{key}/bookings")]

        public async Task<IActionResult> GetListBooking([FromODataUri] int key)
        {
            var homestayBooking = await _homestayService.GetHomestayByIdAsync(key);
            if (homestayBooking == null)
                return NotFound();

            return Ok(homestayBooking);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateHomestayDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //if (string.IsNullOrEmpty(userId))
                //{
                //    return Unauthorized("User ID not found in token.");
                //}

                //var user = await _userManager.FindByIdAsync(userId);
                //if (user == null)
                //{
                //    return Unauthorized("User not found.");
                //}
                //if (userId != dto.OwnerId && !await _userManager.IsInRoleAsync(user, "Admin"))
                //{
                //    return Forbid("You are not authorized to create a homestay for this owner.");
                //}

                var homestay = _mapper.Map<Homestay>(dto);


                var createdHomestay = await _homestayService.CreateHomestayAsync(homestay);

                //var homestayResponse = _mapper.Map<HomestayResponseDTO>(createdHomestay);
                return CreatedAtAction(nameof(Get), new { id = createdHomestay.HomestayId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the homestay.");
            }
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
