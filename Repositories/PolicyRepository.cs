using BusinessObjects;
using DataAccess;

namespace Repositories
{
    public class PolicyRepository : GenericRepository<Policy>
    {
        public PolicyRepository(HomestayDbContext context) : base(context)
        {
        }

    }
}
