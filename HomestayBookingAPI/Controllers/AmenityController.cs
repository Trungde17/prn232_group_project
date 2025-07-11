using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace HomestayBookingAPI.Controllers
{
    public class AmenityController : ODataController
    {
        private readonly IAmenityService _service;

        public AmenityController(IAmenityService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var amenities = await _service.GetAllAmenitiesAsync();
            return Ok(amenities);
        }
    }
}
