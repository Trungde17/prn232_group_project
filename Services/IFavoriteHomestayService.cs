using BusinessObjects.Homestays;

namespace Services
{
    public interface IFavoriteHomestayService
    {
        Task<IEnumerable<FavoriteHomestay>> GetAsync(string userId);
        Task<FavoriteHomestay> CreateAsync(FavoriteHomestay favoriteHomestay);
        Task<FavoriteHomestay> DeleteAsync(FavoriteHomestay favoriteHomestay);
        Task<FavoriteHomestay> GetAsync(int homestayId, string userId);
    }
}
