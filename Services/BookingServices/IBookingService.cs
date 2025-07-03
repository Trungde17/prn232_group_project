using BusinessObjects.Bookings;

namespace Services.BookingServices
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAsync(string userId);
        Task<Booking> CreateBookingAsync(Booking booking, List<int> roomIds);
        public Task<List<int>> CheckRoomAvailabilityAsync(List<int> roomIds, DateTime checkIn, DateTime checkOut);
        public Task<decimal> CalculateTotalAmountAsync(List<int> roomIds, DateTime checkIn, DateTime checkOut);
    }
}
