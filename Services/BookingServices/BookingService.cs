using System.Collections.Generic;
using BusinessObjects.Bookings;
using BusinessObjects.Enums;
using BusinessObjects.Homestays;
using BusinessObjects.Rooms;
using DataAccess;
using Repositories;

namespace Services.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly IGenericRepository<Booking> _bookingRepo;
        private readonly IGenericRepository<Homestay> _homestayRepo;
        private readonly IGenericRepository<BookingDetail> _bookingDetailRepo;
        private readonly IGenericRepository<Room> _roomRepo;
        private readonly HomestayDbContext _context;
        public BookingService(IGenericRepository<Booking> bookingRepo, IGenericRepository<BookingDetail> bookingDetailRepo, IGenericRepository<Room> roomRepo, HomestayDbContext context, IGenericRepository<Homestay> homestayRepo)
        {
            _bookingRepo = bookingRepo;
            _bookingDetailRepo = bookingDetailRepo;
            _roomRepo = roomRepo;
            _homestayRepo = homestayRepo;
            _context = context;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking, List<int> roomIds)
        {
            if (booking == null || roomIds == null || !roomIds.Any())
            {
                throw new ArgumentException("Booking or booking details cannot be null or empty.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bookingCreated = await _bookingRepo.AddAsync(booking);
                if (bookingCreated == null)
                {
                    throw new InvalidOperationException("Failed to create booking.");
                }
                var bookingDetails = roomIds.Select(id => new BookingDetail
                {
                    BookingId = bookingCreated.BookingId,
                    RoomId = id
                })
                .ToList();
                var addedCount = await _bookingDetailRepo.AddRangesAsync(bookingDetails);
                if (addedCount != bookingDetails.Count)
                {
                    throw new InvalidOperationException("Failed to add all booking details.");
                }

                await transaction.CommitAsync();
                return bookingCreated;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Booking>> GetAllAsync()
        {

            return (List<Booking>)await _bookingRepo.AllAsync();

        }

        public async Task<List<Booking>> GetAllByOwnerIdAsync(string ownerId)
        {
           
            var bookings = await _bookingRepo.FindAsync(h => h.Homestay.OwnerId == ownerId) ?? new List<Booking>();

           
            

            return (List<Booking>) bookings;

        }

        public async Task<List<Booking>> GetAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<Booking>();
            }

            return (await _bookingRepo.AllAsync())
                .Where(b => b.CustomerId == userId)
                .ToList();
        }


        public async Task<List<int>> CheckRoomAvailabilityAsync(List<int> roomIds, DateTime checkIn, DateTime checkOut)
        {
            if (roomIds == null || !roomIds.Any() || checkIn >= checkOut)
            {
                throw new ArgumentException("Invalid room IDs or date range.");
            }
            var bookings = await _bookingRepo.AllAsync();
            if (bookings != null)
            {
                var bookingDetails = await _bookingDetailRepo.AllAsync();
                if (bookingDetails != null)
                {
                    var unavailableRoomIds = bookings
                        .Join(bookingDetails,
                            b => b.BookingId,
                            bd => bd.BookingId,
                            (b, bd) => new { b, bd })
                        .Where(x => roomIds.Contains(x.bd.RoomId)
                            && x.b.DateCheckIn < checkOut
                            && x.b.DateCheckOut > checkIn)
                        .Select(x => x.bd.RoomId)
                        .Distinct()
                        .ToList();
                    return unavailableRoomIds;
                }
            }
            return new List<int>();
        }
        public async Task<decimal> CalculateTotalAmountAsync(List<int> roomIds, DateTime checkIn, DateTime checkOut)
        {
            if (roomIds == null || !roomIds.Any())
            {
                throw new ArgumentException("Room IDs cannot be null or empty.");
            }
            if (checkIn >= checkOut)
            {
                throw new ArgumentException("Check-in date must be before check-out date.");
            }

            double totalDays = (checkOut - checkIn).TotalDays;
            int days = (int)Math.Ceiling(totalDays);
            var rooms = await _roomRepo.FindAsync(r => roomIds.Contains(r.RoomId));
            decimal totalAmount = 0;

            foreach (var room in rooms)
            {
                var price = room.RoomPrices?.FirstOrDefault()?.AmountPerNight ?? 0;
                totalAmount += price * (decimal)days;
            }

            return totalAmount;
        }


        public async Task<Booking> GetByIdAsync(int bookingId)
        {
            return await _bookingRepo.GetAsync(bookingId);
        }

        public async Task<Booking> UpdateAsync(int bookingId, Booking updatedBooking)
        {
            var existing = await _bookingRepo.GetAsync(bookingId);
            if (existing == null)
                return null;

            // Cập nhật các trường cần thiết
            existing.DateCheckIn = updatedBooking.DateCheckIn;
            existing.DateCheckOut = updatedBooking.DateCheckOut;
            existing.CustomerId = updatedBooking.CustomerId;
            existing.Status = updatedBooking.Status;
            existing.TotalAmount = updatedBooking.TotalAmount;
            // ... thêm các property khác nếu cần

            await _bookingRepo.UpdateAsync(existing);
            return existing;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            var existing = await _bookingRepo.GetAsync(bookingId);
            if (existing == null)
                return false;

            existing.Status = status;

            await _bookingRepo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int bookingId)
        {
            var booking = await _bookingRepo.GetAsync(bookingId);
            if (booking == null)
                return false;

            await _bookingRepo.DeleteAsync(booking);
            return true;
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepo.GetAsync(bookingId);
            return booking;
        }

        public Task<List<Booking>> GetMyBookingAsync(string userId)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> CheckRoomAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut)
            {
                throw new ArgumentException("Invalid date range.");
            }
            // Check if room exists
            var roomExists = await _roomRepo.GetAsync(roomId);
            if (roomExists == null)
            {
                throw new ArgumentException($"Room with ID {roomId} does not exist.");
            }
            var bookings = await _bookingRepo.AllAsync();
            if (bookings != null)
            {
                var bookingDetails = await _bookingDetailRepo.AllAsync();
                if (bookingDetails != null)
                {
                    return !bookings
                        .Join(bookingDetails,
                            b => b.BookingId,
                            bd => bd.BookingId,
                            (b, bd) => new { b, bd })
                        .Any(x => roomId == x.bd.RoomId
                            && x.b.DateCheckIn < checkOut
                            && x.b.DateCheckOut > checkIn);
                }
            }
            return true;
        }
    }
}
