using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace HomestayBookingAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WardsController : ControllerBase
    
    {
        private readonly IWardService _wardService;

        public WardsController(IWardService wardService)
        {
            _wardService = wardService;
        }

        // GET: odata/Wards
        [HttpGet]
        
        public async Task<IActionResult> Get()
        {
            var wards = await _wardService.getAll();
            return Ok(wards);
        }


    }
}
