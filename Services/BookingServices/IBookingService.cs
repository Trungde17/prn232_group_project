using BusinessObjects.Bookings;
using BusinessObjects.Enums;

namespace Services.BookingServices
{
    public interface IBookingService
    {
        Task<List<Booking>> GetMyBookingAsync(string userId);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<Booking> CreateBookingAsync(Booking booking, List<int> roomIds);
        public Task<List<int>> CheckRoomAvailabilityAsync(List<int> roomIds, DateTime checkIn, DateTime checkOut);
        public Task<decimal> CalculateTotalAmountAsync(List<int> roomIds, DateTime checkIn, DateTime checkOut);
        Task<List<Booking>> GetAllByOwnerIdAsync(string ownerId);
        Task<List<Booking>> GetAllAsync();
        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status);
        Task<Booking> GetByIdAsync(int bookingId);
        Task<Booking> UpdateAsync(int bookingId, Booking updatedBooking);
        Task<bool> DeleteAsync(int bookingId);
        public Task<bool> CheckRoomAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut);
    }
}
