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
        public Task<List<int>> CheckRoomsInHomestayAsync(List<int> roomIds, int homestayId);
        public Task<List<Room>> GetRoomByHomestayIdAsync(int homestayId);
    }
}
