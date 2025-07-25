using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Rooms;

namespace Services
{
    public interface IPriceTypeService
    {
        Task<IEnumerable<PriceType>> GetAllPriceTypesAsync();
    }
}
