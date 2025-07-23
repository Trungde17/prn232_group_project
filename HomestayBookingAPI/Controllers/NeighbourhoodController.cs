using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace HomestayBookingAPI.Controllers
{

    public class NeighbourhoodController : ODataController
    {
        private readonly INeighbourhoodService _service;

        public NeighbourhoodController(INeighbourhoodService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var neighbourhoods = await _service.GetAllNeighbourhoodsAsync();
            return Ok(neighbourhoods);
        }
        [HttpGet("api/neighbourhood/get-all")]
        public async Task<IActionResult> GetAll()
        {
            var neighbourhoods = await _service.GetAllNeighbourhoodsAsync();
            return Ok(neighbourhoods);
        }
    }
}
