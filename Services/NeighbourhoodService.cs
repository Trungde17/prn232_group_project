using BusinessObjects.Homestays;
using Repositories;

namespace Services
{
    public class NeighbourhoodService : INeighbourhoodService
    {
        private readonly IGenericRepository<Neighbourhood> _repository;

        public NeighbourhoodService(IGenericRepository<Neighbourhood> repository)
        {
            _repository = repository;
        }


        public async Task<IEnumerable<Neighbourhood>> GetAllNeighbourhoodsAsync()
        {
            return await _repository.AllAsync();
        }
    }
}
