using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace HomestayBookingAPI.Controllers
{
    public class PriceTypeController : ODataController
    {
        private readonly IPriceTypeService _service;

        public PriceTypeController(IPriceTypeService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var priceTypes = await _service.GetAllPriceTypesAsync();
            return Ok(priceTypes);
        }
    }
}
