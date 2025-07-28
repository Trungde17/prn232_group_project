using BusinessObjects;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class WardRepository : GenericRepository<Ward>
    {
        public WardRepository(HomestayDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Ward>> AllAsync()
        {
            return await context.Wards.Include(w=>w.District).ToListAsync();
            ;
        }
        public override async Task<Ward> GetAsync(dynamic id)
        {
            try
            {
                int Id = (int)id;
                return await context.Wards
                    .FirstOrDefaultAsync(w => w.WardId == Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the booking with ID: {id}", ex);
            }
        }
    }
}
