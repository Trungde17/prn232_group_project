using BusinessObjects.Homestays;
using DataAccess;

namespace Repositories.HomeStayRepository
{
    public class FavoriteHomestayRepository : GenericRepository<FavoriteHomestay>
    {
        public FavoriteHomestayRepository(HomestayDbContext context) : base(context) { }
    }
}
