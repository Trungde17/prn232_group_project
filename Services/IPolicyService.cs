using BusinessObjects;

namespace Services
{
    public interface IPolicyService
    {
        Task<IEnumerable<Policy>> GetAllPoliciesAsync();
    }
}
