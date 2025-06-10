using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Homestays;
using DataAccess;

namespace Repositories.HomeStayRepository
{
    public class HomestayNeighbourhoodRepository : GenericRepository<HomestayNeighbourhood>
    {
        public HomestayNeighbourhoodRepository(HomestayDbContext context) : base(context)
        {
        }
    }
}
