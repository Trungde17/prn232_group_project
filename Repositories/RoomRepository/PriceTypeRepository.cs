using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Rooms;
using DataAccess;

namespace Repositories.RoomRepository
{
    public class PriceTypeRepository : GenericRepository<PriceType>
    {
        public PriceTypeRepository(HomestayDbContext context) : base(context)
        {
        }
    }
}
