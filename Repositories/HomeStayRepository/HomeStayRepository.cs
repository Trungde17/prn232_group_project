using BusinessObjects.Homestays;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using DTOs;
namespace Repositories.HomeStayRepository
{
    public class HomeStayRepository : GenericRepository<Homestay>, IHomeStayRepository
    {
        public HomeStayRepository(HomestayDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Homestay>> AllAsync()
        {
            return await context.Homestays
                .Include(h => h.HomestayType)
                .Include(b => b.Feedbacks)
                .Include(b => b.HomestayAmenities)
                .Include(b => b.HomestayPolicies)
                .Include(b => b.HomestayNeighbourhoods)
                .Include(b => b.Bookings).ThenInclude(b => b.BookingDetails)
                .Include(b => b.Rooms)
                .ToListAsync();
        }

        public override async Task<Homestay> GetAsync(dynamic id)
        {
            try
            {
                int Id = (int)id;
                return await context.Homestays
                    .Include(h => h.HomestayType)
                    .Include(b => b.Feedbacks)
                    .Include(b => b.HomestayAmenities).ThenInclude(ha => ha.Amenity)
                    .Include(b => b.HomestayPolicies).ThenInclude(hp => hp.Policy)
                    .Include(b => b.HomestayNeighbourhoods).ThenInclude(hn => hn.Neighbourhood)
                    .Include(b => b.Rooms)
                    .Include(b => b.HomestayImages)
                    .Include(b => b.Ward).ThenInclude(w => w.District)
                    .Include(b=>b.Bookings).ThenInclude(b=>b.BookingDetails)
                    .FirstOrDefaultAsync(b => b.HomestayId == Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the booking with ID: {id}", ex);
            }
        }
        public override async Task<IEnumerable<Homestay>> FindAsync(Expression<Func<Homestay, bool>> predicate)
        {
            return await context.Homestays
                    .Include(h => h.HomestayType)
                    .Include(b => b.Feedbacks)
                    .Include(b => b.HomestayAmenities)
                    .Include(b => b.HomestayPolicies)
                    .Include(b => b.HomestayNeighbourhoods)
                    .Include(b => b.Bookings).ThenInclude(b => b.BookingDetails)
                    .Include(b => b.Rooms).Where(predicate).ToListAsync();
        }

        public override async Task<Homestay> GetWithConditionAsync(Expression<Func<Homestay, bool>> predicate)
        {
            return await context.Homestays
                    .Include(h => h.HomestayType)
                    .Include(b => b.Feedbacks)
                    .Include(b => b.HomestayAmenities)
                    .Include(b => b.HomestayPolicies)
                    .Include(b => b.HomestayNeighbourhoods)
                    .Include(b => b.Bookings).ThenInclude(b => b.BookingDetails)
                    .Include(b => b.Rooms).FirstOrDefaultAsync(predicate);
        }
        public async Task<Homestay> GetDetailByIdAsync(int id)
        {
            return await context.Homestays
                .Include(h => h.Ward)
                    .ThenInclude(w => w.District)
                .Include(h => h.Owner)
                .Include(h => h.HomestayImages)
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.HomestayId == id);
        }
        public async Task<List<HomestayListDTO>> SearchWithInfoAsync(Expression<Func<Homestay, bool>> predicate)
        {
            return await context.Homestays
                .Include(h => h.Ward).ThenInclude(w => w.District)
                .Include(h => h.HomestayImages)
                .Where(predicate)
                .Select(h => new HomestayListDTO
                {
                    name = h.Name,
                    Rules = h.Rules,
                    streetAddress = h.StreetAddress + ", " + h.Ward.Name + ", " + h.Ward.District.Name,
                    Status = h.Status,
                    ImageUrl = h.HomestayImages.Select(i => i.ImageUrl).FirstOrDefault()
                })
                .ToListAsync();
        }



    }
}
