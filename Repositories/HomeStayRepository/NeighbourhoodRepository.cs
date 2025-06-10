using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Homestays;
using DataAccess;

namespace Repositories.HomeStayRepository
{
    public class NeighbourhoodRepository : GenericRepository<Neighbourhood>
    {
        public NeighbourhoodRepository(HomestayDbContext context) : base(context)
        {
        }
    }
}
