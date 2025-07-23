using AutoMapper;
using DTOs.RoomDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services.BookingServices;
using Services.HomestayServices;
using Services.RoomServices;

namespace HomestayBookingAPI.Controllers
{
    //[Route("odata/[controller]")]
    public class RoomsController : ODataController
    {
        private readonly IRoomService _roomService;
        private readonly IHomestayService _homestayService;
        private readonly IMapper _mapper;
        private readonly IBookingService _bookingService;

        public RoomsController(IRoomService roomService, IHomestayService homestayService, IMapper mapper, IBookingService bookingService)
        {
            _roomService = roomService;
            _homestayService = homestayService;
            _mapper = mapper;
            _bookingService = bookingService;

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
        [HttpGet("api/rooms/get-rooms-by-homestayId/{homestayId}")]
        public async Task<IActionResult> GetRoomsByHomestayId(int homestayId,
            [FromQuery] DateTime? checkIn,
            [FromQuery] DateTime? checkOut)
        {
            if (homestayId <= 0)
            {
                return NotFound($"HomestayId {homestayId} is not found.");
            }

            var homestay = await _homestayService.GetHomestayByIdAsync(homestayId);
            if (homestay == null) return NotFound($"HomestayId {homestayId} is not found.");
            if (checkIn.HasValue && checkOut.HasValue)
            {
                if (checkIn >= checkOut)
                {
                    return BadRequest("Check-in date must be before check-out date.");
                }
            }
            else if (checkIn.HasValue ^ checkOut.HasValue)
            {
                return BadRequest("Both CheckIn and CheckOut must be provided together.");
            }
            try
            {


                var rooms = await _roomService.GetRoomByHomestayIdAsync(homestayId);
                var roomResponses = new List<GetRoomForBookingResponseDTO>();

                if (rooms != null)
                {
                    roomResponses = _mapper.Map<List<GetRoomForBookingResponseDTO>>(rooms).ToList();
                    foreach (var roomResponse in roomResponses)
                    {
                        roomResponse.IsAvailable = await _bookingService.CheckRoomAvailabilityAsync(roomResponse.RoomId, checkIn.Value, checkOut.Value);
                    }
                }
                return Ok(roomResponses);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "An error occurred while retrieving bookings.");
            }
        }

    }
}
