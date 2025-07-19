using DTOs.RoomDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services.RoomServices;

namespace HomestayBookingAPI.Controllers
{
    //[Route("odata/[controller]")]
    public class RoomsController : ODataController
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms.AsQueryable()); // EnableQuery requires IQueryable
        }

        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var room = await _roomService.GetRoomByIdAsync(key);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RoomCreateDto dto)
        {
            var created = await _roomService.CreateRoomAsync(dto);
            return Created(created);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] RoomUpdateDto dto)
        {
            if (key != dto.RoomId)
                return BadRequest("Mismatched Room ID.");

            var updated = await _roomService.UpdateRoomAsync(key, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            var deleted = await _roomService.DeleteRoomAsync(key);
            return deleted ? Ok(deleted) : NotFound();
        }
    }
}
