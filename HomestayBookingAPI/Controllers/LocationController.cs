using BusinessObjects;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomestayBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly HomestayDbContext _context;

        public LocationController(HomestayDbContext context)
        {
            _context = context;
        }

        // GET: api/location/districts
        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts()
        {
            var districts = await _context.Districts
                .Select(d => new
                {
                    d.DistrictId,
                    d.Name
                })
                .ToListAsync();

            return Ok(districts);
        }

        // GET: api/location/wards?districtId=1
        [HttpGet("wards")]
        public async Task<IActionResult> GetWards(int districtId)
        {
            var wards = await _context.Wards
                .Where(w => w.DistrictId == districtId)
                .Select(w => new
                {
                    w.WardId,
                    w.Name,
                    w.DistrictId
                })
                .ToListAsync();

            return Ok(wards);
        }
    }
}
