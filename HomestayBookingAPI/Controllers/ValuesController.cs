using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Services.RoomServices;

namespace HomestayBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public ValuesController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms.AsQueryable()); 
        }
    }
}
