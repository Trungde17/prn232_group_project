using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Rooms;
using Repositories;
using Repositories.RoomRepository;

namespace Services
{
    public class PriceTypeService : IPriceTypeService
    {
        private readonly IGenericRepository<PriceType> _repository;

        public PriceTypeService(IGenericRepository<PriceType> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PriceType>> GetAllPriceTypesAsync()
        {
            return await _repository.AllAsync();
        }
    }
}
