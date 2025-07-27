using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class WardService : IWardService
    {
        private readonly IGenericRepository<Ward> _wardRepository;

        public WardService(IGenericRepository<Ward> wardRepository)
        {
            _wardRepository = wardRepository;
        }

        public async Task<List<Ward>> getAll()
        {
            return (List<Ward>)await _wardRepository.AllAsync();
        }
    }
}
