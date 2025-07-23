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


                // Update Room

                // Update Room
                room.Name = dto.Name;
                room.Description = dto.Description;
                room.ImgUrl = dto.ImgUrl;
                room.Capacity = dto.Capacity;
                room.Size = dto.Size;

                await _roomRepo.UpdateAsync(room);


                #region RoomBeds


                var oldBeds = (await _roomBedRepo.FindAsync(rb => rb.RoomId == id)).ToList();
                var dtoBeds = dto.RoomBeds ?? new List<RoomBedDto>();

                // XÓA beds không còn
                var dtoBedTypeIds = dtoBeds.Select(b => b.BedTypeId).ToList();
                var bedsToDelete = oldBeds.Where(ob => !dtoBedTypeIds.Contains(ob.BedTypeId)).ToList();
                if (bedsToDelete.Any())
                    await _roomBedRepo.DeleteRangeAsync(bedsToDelete);

                // ADD or UPDATE
                foreach (var bedDto in dtoBeds)
                {
                    var existingBed = oldBeds.FirstOrDefault(x => x.BedTypeId == bedDto.BedTypeId);
                    if (existingBed != null)
                    {
                        existingBed.Quantity = bedDto.Quantity;
                        await _roomBedRepo.UpdateAsync(existingBed);
                    }
                    else
                    {
                        var newBed = new RoomBed
                        {
                            RoomId = id,
                            BedTypeId = bedDto.BedTypeId,
                            Quantity = bedDto.Quantity
                        };
                        await _roomBedRepo.AddAsync(newBed);
                    }
                }


                #endregion


                #region RoomPrices

                var oldPrices = (await _roomPriceRepo.FindAsync(rp => rp.RoomId == id)).ToList();
                var dtoPrices = dto.RoomPrices ?? new List<RoomPriceDto>();

                var dtoPriceTypeIds = dtoPrices.Select(p => p.PriceTypeId).ToList();
                var pricesToDelete = oldPrices.Where(op => !dtoPriceTypeIds.Contains(op.PriceTypeId)).ToList();
                if (pricesToDelete.Any())
                    await _roomPriceRepo.DeleteRangeAsync(pricesToDelete);

                foreach (var priceDto in dtoPrices)
                {
                    var existingPrice = oldPrices.FirstOrDefault(x => x.PriceTypeId == priceDto.PriceTypeId);
                    if (existingPrice != null)
                    {
                        existingPrice.AmountPerNight = priceDto.AmountPerNight;
                        await _roomPriceRepo.UpdateAsync(existingPrice);
                    }
                    else
                    {
                        var newPrice = new RoomPrice
                        {
                            RoomId = id,
                            PriceTypeId = priceDto.PriceTypeId,
                            AmountPerNight = priceDto.AmountPerNight
                        };
                        await _roomPriceRepo.AddAsync(newPrice);
                    }
                }

                #endregion


                #region RoomAmenities

                var oldAmenities = (await _roomAmenityRepo.FindAsync(ra => ra.RoomId == id)).ToList();
                var dtoAmenities = dto.RoomAmenities ?? new List<RoomAmenityDto>();

                var dtoAmenityIds = dtoAmenities.Select(a => a.AmenityId).ToList();
                var amenitiesToDelete = oldAmenities.Where(oa => !dtoAmenityIds.Contains(oa.AmenityId)).ToList();
                if (amenitiesToDelete.Any())
                    await _roomAmenityRepo.DeleteRangeAsync(amenitiesToDelete);

                foreach (var amenityDto in dtoAmenities)
                {
                    var existingAmenity = oldAmenities.FirstOrDefault(x => x.AmenityId == amenityDto.AmenityId);
                    if (existingAmenity == null)
                    {
                        var newAmenity = new RoomAmenity
                        {
                            RoomId = id,
                            AmenityId = amenityDto.AmenityId
                        };
                        await _roomAmenityRepo.AddAsync(newAmenity);
                    }
                }


                #endregion


                #region RoomSchedules

                var oldSchedules = (await _roomScheduleRepo.FindAsync(rs => rs.RoomId == id)).ToList();
                var dtoSchedules = dto.RoomSchedules ?? new List<RoomScheduleDto>();

                // XÓA schedules không còn
                var schedulesToDelete = oldSchedules
                    .Where(os => !dtoSchedules.Any(ds =>
                        ds.StartDate == os.StartDate &&
                        ds.EndDate == os.EndDate &&
                        ds.ScheduleType == os.ScheduleType))
                    .ToList();

                if (schedulesToDelete.Any())
                    await _roomScheduleRepo.DeleteRangeAsync(schedulesToDelete);

                // ADD mới
                foreach (var scheduleDto in dtoSchedules)
                {
                    var existingSchedule = oldSchedules.FirstOrDefault(os =>
                        os.StartDate == scheduleDto.StartDate &&
                        os.EndDate == scheduleDto.EndDate &&
                        os.ScheduleType == scheduleDto.ScheduleType);

                    if (existingSchedule == null)
                    {
                        var newSchedule = new RoomSchedule
                        {
                            RoomId = id,
                            StartDate = scheduleDto.StartDate,
                            EndDate = scheduleDto.EndDate,
                            ScheduleType = scheduleDto.ScheduleType
                        };
                        await _roomScheduleRepo.AddAsync(newSchedule);
                    }
                }

                #endregion

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
        public async Task<List<Room>> GetRoomByHomestayIdAsync(int homestayId)
        {
            return (await _roomRepo.FindAsync(r => r.HomestayId == homestayId)).ToList();
        }
    }

}
