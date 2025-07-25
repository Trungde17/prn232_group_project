using BusinessObjects.Homestays;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Repositories.HomeStayRepository
{
    public class HomestayTypeRepository : GenericRepository<HomestayType>
    {
        public HomestayTypeRepository(HomestayDbContext context) : base(context)
        {
        }
        public override async Task<HomestayType> GetAsync(dynamic id)
        {
            try
            {
                int Id = (int)id;
                return await context.HomestayTypes
                    .FirstOrDefaultAsync(ht => ht.HomestayTypeId == Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the booking with ID: {id}", ex);
            }
        }
    }
}
