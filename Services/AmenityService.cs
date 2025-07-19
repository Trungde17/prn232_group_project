using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class AmenityService : IAmenityService
    {
        private readonly IGenericRepository<Amenity> _repository;

        public AmenityService(IGenericRepository<Amenity> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Amenity>> GetAllAmenitiesAsync()
        {
            return await _repository.AllAsync();
        }
    }
}
