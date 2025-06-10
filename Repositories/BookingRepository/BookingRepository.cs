using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Bookings;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Repositories.BookingRepository
{
    public class BookingRepository : GenericRepository<Booking>
    {
        public BookingRepository(HomestayDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Booking>> AllAsync()
        {
            return await context.Bookings
                .Include(b => b.BookingDetails)
                .Include(b => b.Homestay)
                .Include(b => b.Customer)
                .ToListAsync();
        }

        public override async Task<Booking> GetAsync(dynamic id)
        {
            try
            {
                int Id = (int)id;
                return await context.Bookings
                    .Include(b => b.BookingDetails)
                    .Include(b => b.Homestay)
                    .Include(b => b.Customer)
                    .FirstOrDefaultAsync(b => b.BookingId == Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the booking with ID: {id}", ex);
            }
        }
        public virtual async Task<IEnumerable<Booking>> FindAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await context.Bookings.Include(b => b.BookingDetails)
                    .Include(b => b.Homestay)
                    .Include(b => b.Customer).Where(predicate).ToListAsync();
        }

        public virtual async Task<Booking> GetWithConditionAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await context.Bookings.Include(b => b.BookingDetails)
                    .Include(b => b.Homestay)
                    .Include(b => b.Customer).FirstOrDefaultAsync(predicate);
        }



    }
}
