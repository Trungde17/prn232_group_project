using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Homestays;
using BusinessObjects.Rooms;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Repositories.RoomRepository
{
    public class RoomRepository : GenericRepository<Room>
    {
        public RoomRepository(HomestayDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Room>> AllAsync()
        {
            return await context.Rooms
                .Include(h => h.Homestay)
                .Include(b => b.RoomBeds)
                .Include(b => b.RoomPrices)
                .Include(b => b.RoomAmenities)
                .Include(b => b.RoomSchedules)
                .Include(b => b.BookingDetails)
                .ToListAsync();
        }

        public override async Task<Room> GetAsync(dynamic id)
        {
            try
            {
                int Id = (int)id;
                return await context.Rooms
                    .Include(h => h.Homestay)
                    .Include(b => b.RoomBeds)
                    .Include(b => b.RoomPrices)
                    .Include(b => b.RoomAmenities)
                    .Include(b => b.RoomSchedules)
                    .Include(b => b.BookingDetails)
                    .FirstOrDefaultAsync(b => b.HomestayId == Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the booking with ID: {id}", ex);
            }
        }
        public virtual async Task<IEnumerable<Room>> FindAsync(Expression<Func<Room, bool>> predicate)
        {
            return await context.Rooms
                    .Include(h => h.Homestay)
                    .Include(b => b.RoomBeds)
                    .Include(b => b.RoomPrices)
                    .Include(b => b.RoomAmenities)
                    .Include(b => b.RoomSchedules)
                    .Include(b => b.BookingDetails).Where(predicate).ToListAsync();
            }

        public virtual async Task<Room> GetWithConditionAsync(Expression<Func<Room, bool>> predicate)
        {
            return await context.Rooms
                    .Include(h => h.Homestay)
                    .Include(b => b.RoomBeds)
                    .Include(b => b.RoomPrices)
                    .Include(b => b.RoomAmenities)
                    .Include(b => b.RoomSchedules)
                    .Include(b => b.BookingDetails).FirstOrDefaultAsync(predicate);
        }
    }
}
