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
    public class BedTypeService : IBedTypeService
    {
        private readonly IGenericRepository<BedType> _repository;

        public BedTypeService(IGenericRepository<BedType> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BedType>> GetAllBedTypesAsync()
        {
            return await _repository.AllAsync();
        }
    }
}
