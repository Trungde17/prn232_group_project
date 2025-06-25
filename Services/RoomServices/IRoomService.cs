using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Rooms;
using DTOs.RoomDtos;

namespace Services.RoomServices
{
    public interface IRoomService
    {
        Task<Room> CreateRoomAsync(RoomCreateDto dto);
        Task<Room> UpdateRoomAsync(int id, RoomUpdateDto dto);
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room> GetRoomByIdAsync(int id);
        Task<bool> DeleteRoomAsync(int id);
    }
}
