using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
