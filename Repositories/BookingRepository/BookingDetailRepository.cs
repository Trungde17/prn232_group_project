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
    public class BookingDetailRepository : GenericRepository<BookingDetail>
    {
        public BookingDetailRepository(HomestayDbContext context) : base(context)
        {
        }


        public override async Task<IEnumerable<BookingDetail>> AllAsync()
        {
            return await context.BookingDetails.Include(b => b.Room).Include(b => b.Booking).ToListAsync();
        }
        public async override Task<BookingDetail> GetAsync(dynamic id)
        {
            try
            {
                int Id = (int)id;
                return await context.BookingDetails
                    .Include(b => b.Room).Include(b => b.Booking)
                    .FirstOrDefaultAsync(b => b.BookingId == Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the booking with ID: {id}", ex);
            }
        }

        public override Task<BookingDetail> GetWithConditionAsync(Expression<Func<BookingDetail, bool>> predicate)
        {
            return context.BookingDetails.Include(b => b.Room).Include(b => b.Booking).FirstOrDefaultAsync(predicate);
        }

        public override async Task<IEnumerable<BookingDetail>> FindAsync(Expression<Func<BookingDetail, bool>> predicate)
        {
            return await context.BookingDetails.Include(b => b.Room).Include(b => b.Booking).Where(predicate).ToListAsync();
        }

    }
}
