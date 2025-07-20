using BusinessObjects.Bookings;

namespace Services.BookingServices
{
    public interface IBookingService
    {
        Task<List<Booking>> GetMyBookingAsync(string userId);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<Booking> CreateBookingAsync(Booking booking, List<int> roomIds);
        public Task<List<int>> CheckRoomAvailabilityAsync(List<int> roomIds, DateTime checkIn, DateTime checkOut);
        public Task<bool> CheckRoomAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut);
        public Task<decimal> CalculateTotalAmountAsync(List<int> roomIds, DateTime checkIn, DateTime checkOut);
    }
}
