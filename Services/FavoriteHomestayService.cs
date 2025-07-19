using BusinessObjects.Homestays;
using Microsoft.EntityFrameworkCore;
using Repositories.HomeStayRepository;

namespace Services
{
    public class FavoriteHomestayService : IFavoriteHomestayService
    {
        private readonly FavoriteHomestayRepository _repository;

        public FavoriteHomestayService(FavoriteHomestayRepository repository)
        {
            _repository = repository;
        }
        public async Task<FavoriteHomestay> CreateAsync(FavoriteHomestay favoriteHomestay)
        {
            if (favoriteHomestay == null)
            {
                throw new ArgumentNullException(nameof(favoriteHomestay), "Favorite homestay cannot be null.");
            }

            // Kiểm tra xem homestay đã được yêu thích hay chưa
            var existing = await GetAsync(favoriteHomestay.HomestayId, favoriteHomestay.UserId);
            if (existing != null)
            {
                throw new InvalidOperationException("Homestay is already in user's favorites.");
            }

            return await _repository.AddAsync(favoriteHomestay);
        }

        public async Task<FavoriteHomestay> DeleteAsync(FavoriteHomestay favoriteHomestay)
        {
            if (favoriteHomestay == null)
            {
                throw new ArgumentNullException(nameof(favoriteHomestay), "Favorite homestay cannot be null.");
            }

            return await _repository.DeleteAsync(favoriteHomestay);
        }

        public async Task<IEnumerable<FavoriteHomestay>> GetAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
            }

            return await _repository.GetQueryable()
                .Where(fh => fh.UserId == userId)
                .Include(fh => fh.Homestay)
                .ToListAsync();
        }

        public async Task<FavoriteHomestay> GetAsync(int homestayId, string userId)
        {
            if (homestayId <= 0)
            {
                throw new ArgumentException("Homestay ID must be greater than zero.", nameof(homestayId));
            }
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
            }

            return await _repository.GetQueryable()
                .FirstOrDefaultAsync(fh => fh.HomestayId == homestayId && fh.UserId == userId);
        }
    }
}
