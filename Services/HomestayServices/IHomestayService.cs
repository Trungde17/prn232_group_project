using BusinessObjects.Bookings;
using BusinessObjects.Homestays;
using DTOs.HomestayDtos;

namespace Services.HomestayServices
{
    public interface IHomestayService
    {
        Task<IEnumerable<Homestay>> GetAllHomestaysAsync();
        Task<Homestay> GetHomestayByIdAsync(int id);
        Task<List<Booking>> GetBookingList(int homeStayId);
        Task<Homestay> UpdateHomestayAsync(int id, HomestayUpdateDto dto);
        Task<bool> CheckValidHomestay(int id);
    }
}
