using AutoMapper;
using BusinessObjects;
using BusinessObjects.Homestays;
using DTOs.HomestayDtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services.HomestayServices;
using System.Security.Claims;


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
        [EnableQuery(MaxAnyAllExpressionDepth = 5)]
        [HttpGet("")]
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


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner")]
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> MyHomestays()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
            //var userIdClaim = "05a1acb8-31ac-4735-be45-4c12f089eb1c";
            if (userIdClaim == null)
                return StatusCode(500, "Cannot retrieve user ID");

            //var homestays = await _homestayService.GetHomestayByUserIdAsync(userIdClaim.Value);
            var homestays = await _homestayService.GetHomestayByUserIdAsync(userIdClaim);
            return Ok(homestays);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateHomestayDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var homestay = _mapper.Map<Homestay>(dto);
                var createdHomestay = await _homestayService.CreateHomestayAsync(homestay);
                return Ok(new { homestay = homestay });
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
