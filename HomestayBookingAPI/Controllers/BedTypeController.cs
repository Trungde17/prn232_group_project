using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace HomestayBookingAPI.Controllers
{
    public class BedTypeController : ODataController
    {
        private readonly IBedTypeService _service;

        public BedTypeController(IBedTypeService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var bedTypes = await _service.GetAllBedTypesAsync();
            return Ok(bedTypes);
        }
    }
}
