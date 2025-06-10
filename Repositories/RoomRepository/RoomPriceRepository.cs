using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Rooms;
using DataAccess;

namespace Repositories.RoomRepository
{
    public class RoomPriceRepository : GenericRepository<RoomPrice>
    {
        public RoomPriceRepository(HomestayDbContext context) : base(context)
        {
        }
    }
}
