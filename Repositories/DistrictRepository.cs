using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccess;

namespace Repositories
{
    public class DistrictRepository : GenericRepository<District>
    {
        public DistrictRepository(HomestayDbContext context) : base(context)
        {
        }
    }
}
