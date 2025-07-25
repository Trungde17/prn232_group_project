using BusinessObjects;
using Repositories;

namespace Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IGenericRepository<Policy> _repository;

        public PolicyService(IGenericRepository<Policy> repository)
        {
            _repository = repository;
        }


        public async Task<IEnumerable<Policy>> GetAllPoliciesAsync()
        {
            return await _repository.AllAsync();
        }
    }
}
