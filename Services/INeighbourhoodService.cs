using BusinessObjects.Homestays;

namespace Services
{
    public interface INeighbourhoodService
    {
        Task<IEnumerable<Neighbourhood>> GetAllNeighbourhoodsAsync();
    }
}
