using BusinessObjects.Rooms;
using DataAccess;
using DTOs.RoomDtos;
using Repositories;
using System.Linq.Expressions;

namespace Services.RoomServices
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Room> _roomRepo;
        private readonly IGenericRepository<RoomBed> _roomBedRepo;
        private readonly IGenericRepository<RoomAmenity> _roomAmenityRepo;
        private readonly IGenericRepository<RoomPrice> _roomPriceRepo;
        private readonly IGenericRepository<RoomSchedule> _roomScheduleRepo;
        private readonly HomestayDbContext _dbContext;
        public RoomService(HomestayDbContext dbContext,
            IGenericRepository<Room> roomRepo,
            IGenericRepository<RoomBed> roomBedRepo,
            IGenericRepository<RoomAmenity> roomAmenityRepo,
            IGenericRepository<RoomPrice> roomPriceRepo,
            IGenericRepository<RoomSchedule> roomScheduleRepo)
        {
            _roomRepo = roomRepo;
            _roomBedRepo = roomBedRepo;
            _roomAmenityRepo = roomAmenityRepo;
            _roomPriceRepo = roomPriceRepo;
            _roomScheduleRepo = roomScheduleRepo;
            _dbContext = dbContext;
        }

        public async Task<Room> CreateRoomAsync(RoomCreateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var room = new Room
                {
                    Name = dto.Name,
                    HomestayId = dto.HomestayId,
                    Description = dto.Description,
                    ImgUrl = dto.ImgUrl,
                    Capacity = dto.Capacity,
                    Size = dto.Size
                };

                await _roomRepo.AddAsync(room);

                if (dto.RoomBeds?.Any() == true)
                {
                    var beds = dto.RoomBeds.Select(b => new RoomBed
                    {
                        RoomId = room.RoomId,
                        BedTypeId = b.BedTypeId,
                        Quantity = b.Quantity
                    });
                    await _roomBedRepo.AddRangeAsync(beds);
                }

                if (dto.RoomAmenities?.Any() == true)
                {
                    var amenities = dto.RoomAmenities.Select(a => new RoomAmenity
                    {
                        RoomId = room.RoomId,
                        AmenityId = a.AmenityId
                    });
                    await _roomAmenityRepo.AddRangeAsync(amenities);
                }

                if (dto.RoomPrices?.Any() == true)
                {
                    var prices = dto.RoomPrices.Select(p => new RoomPrice
                    {
                        RoomId = room.RoomId,
                        PriceTypeId = p.PriceTypeId,
                        AmountPerNight = p.AmountPerNight
                    });
                    await _roomPriceRepo.AddRangeAsync(prices);
                }

                if (dto.RoomSchedules?.Any() == true)
                {
                    var schedules = dto.RoomSchedules.Select(s => new RoomSchedule
                    {
                        RoomId = room.RoomId,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        ScheduleType = s.ScheduleType // Đừng quên dòng này nếu dùng enum!
                    });
                    await _roomScheduleRepo.AddRangeAsync(schedules);
                }

                await transaction.CommitAsync();
                return room;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<Room> UpdateRoomAsync(int id, RoomUpdateDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var room = await _roomRepo.GetAsync(id);
                if (room == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }


                room.Name = dto.Name;
                room.Description = dto.Description;
                room.ImgUrl = dto.ImgUrl;
                room.Capacity = dto.Capacity;
                room.Size = dto.Size;

                await _roomRepo.UpdateAsync(room);


                var oldBeds = await _roomBedRepo.FindAsync(rb => rb.RoomId == id);
                if (oldBeds != null && oldBeds.Any())
                    await _roomBedRepo.DeleteRangeAsync(oldBeds);

                if (dto.RoomBeds?.Any() == true)
                {
                    var newBeds = dto.RoomBeds.Select(b => new RoomBed
                    {
                        RoomId = id,
                        BedTypeId = b.BedTypeId,
                        Quantity = b.Quantity
                    });
                    await _roomBedRepo.AddRangeAsync(newBeds);
                }


                var oldPrices = await _roomPriceRepo.FindAsync(rb => rb.RoomId == id);
                if (oldPrices != null && oldPrices.Any())
                    await _roomPriceRepo.DeleteRangeAsync(oldPrices);

                if (dto.RoomPrices?.Any() == true)
                {
                    var newPrices = dto.RoomPrices.Select(p => new RoomPrice
                    {
                        RoomId = id,
                        PriceTypeId = p.PriceTypeId,
                        AmountPerNight = p.AmountPerNight
                    });
                    await _roomPriceRepo.AddRangeAsync(newPrices);
                }


                var oldAmenities = await _roomAmenityRepo.FindAsync(rb => rb.RoomId == id);
                if (oldAmenities != null && oldAmenities.Any())
                    await _roomAmenityRepo.DeleteRangeAsync(oldAmenities);

                if (dto.RoomAmenities?.Any() == true)
                {
                    var newAmenities = dto.RoomAmenities.Select(a => new RoomAmenity
                    {
                        RoomId = id,
                        AmenityId = a.AmenityId
                    });
                    await _roomAmenityRepo.AddRangeAsync(newAmenities);
                }


                var oldSchedules = await _roomScheduleRepo.FindAsync(rb => rb.RoomId == id);
                if (oldSchedules != null && oldSchedules.Any())
                    await _roomScheduleRepo.DeleteRangeAsync(oldSchedules);

                if (dto.RoomSchedules?.Any() == true)
                {
                    var newSchedules = dto.RoomSchedules.Select(s => new RoomSchedule
                    {
                        RoomId = id,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        ScheduleType = s.ScheduleType
                    });
                    await _roomScheduleRepo.AddRangeAsync(newSchedules);
                }


                await transaction.CommitAsync();

                return room;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _roomRepo.AllAsync();
        }

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            return await _roomRepo.GetAsync(id);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _roomRepo.GetAsync(id);
            if (room == null) return false;

            return await _roomRepo.DeleteAsync(room) != null;
        }
        public async Task<List<int>> CheckRoomsInHomestayAsync(List<int> roomIds, int homestayId)
        {
            if (roomIds == null || !roomIds.Any())
            {
                throw new ArgumentException("Room IDs cannot be null or empty.");
            }
            if (homestayId <= 0)
            {
                throw new ArgumentException("Invalid HomestayId.");
            }
            Expression<Func<Room, bool>> predicate = r => roomIds.Contains(r.RoomId);
            var rooms = await _roomRepo.FindAsync(predicate);

            var invalidRoomIds = roomIds
                .Where(id => !rooms.Any(r => r.RoomId == id && r.HomestayId == homestayId))
                .ToList();

            return invalidRoomIds;
        }
    }

}
