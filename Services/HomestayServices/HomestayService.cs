﻿using BusinessObjects;
using BusinessObjects.Bookings;
using BusinessObjects.Homestays;
using DataAccess;
using DTOs.HomestayDtos;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services.HomestayServices
{
    public class HomestayService : IHomestayService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IGenericRepository<Homestay> _homestayRepo;
        private readonly IGenericRepository<HomestayAmenity> _amenityRepo;
        private readonly IGenericRepository<HomestayPolicy> _policyRepo;
        private readonly IGenericRepository<HomestayNeighbourhood> _neighbourhoodRepo;
        private readonly IGenericRepository<HomestayType> _homestayTypeRepo;
        private readonly IGenericRepository<HomestayImage> _imageRepo;
        private readonly IGenericRepository<Ward> _wardRepo;
        private readonly HomestayDbContext _context;

        public HomestayService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IGenericRepository<Homestay> homestayRepo,
            IGenericRepository<HomestayAmenity> amenityRepo,
            IGenericRepository<HomestayPolicy> policyRepo,
            IGenericRepository<HomestayNeighbourhood> neighbourhoodRepo,
            IGenericRepository<HomestayType> homestayTypeRepo,
            IGenericRepository<HomestayImage> imageRepo,
            IGenericRepository<Ward> wardRepo,

            HomestayDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _homestayRepo = homestayRepo;
            _amenityRepo = amenityRepo;
            _policyRepo = policyRepo;
            _neighbourhoodRepo = neighbourhoodRepo;
            _homestayTypeRepo = homestayTypeRepo;
            _imageRepo = imageRepo;
            _wardRepo = wardRepo;
            _context = context;
        }
        public async Task<IEnumerable<Homestay>> GetAllHomestaysAsync()
        {
            return await _homestayRepo.AllAsync();
        }

        public async Task<List<Booking>> GetBookingList(int homeStayId)
        {
            var homestay = await _homestayRepo
                .GetWithConditionAsync(h => h.HomestayId == homeStayId);
            var listBooking = homestay?.Bookings?.ToList() ?? new List<Booking>();

            return listBooking;
        }

        public async Task<Homestay> GetHomestayByIdAsync(int id)
        {
            return await _homestayRepo.GetAsync(id);
        }

        public async Task<IEnumerable<Homestay>> GetHomestayByUserIdAsync(string userId)
        {
            // Lấy danh sách homestay mà OwnerId bằng userId
            var homestays = await _homestayRepo.FindAsync(h => h.OwnerId.Equals(userId));
            return homestays;
        }

        public async Task<Homestay> UpdateHomestayAsync(int id, HomestayUpdateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var homestay = await _homestayRepo.GetAsync(id);
                if (homestay == null) return null;

                homestay.Name = dto.Name;
                homestay.Description = dto.Description;
                homestay.StreetAddress = dto.StreetAddress;
                homestay.WardId = dto.WardId;
                homestay.Rules = dto.Rules;
                homestay.OwnerId = dto.OwnerId;
                homestay.HomestayTypeId = dto.HomestayTypeId;
                homestay.UpdatedAt = DateTime.UtcNow;
                homestay.Status = dto.Status;

                await _homestayRepo.UpdateAsync(homestay);

                // Amenities
                var oldAmenities = await _amenityRepo.FindAsync(x => x.HomestayId == id);
                await _amenityRepo.DeleteRangeAsync(oldAmenities);
                if (dto.Amenities?.Any() == true)
                {
                    var newAmenities = dto.Amenities.Select(a => new HomestayAmenity
                    {
                        HomestayId = id,
                        AmenityId = a.AmenityId
                    });
                    await _amenityRepo.AddRangeAsync(newAmenities);
                }

                // Policies
                var oldPolicies = await _policyRepo.FindAsync(x => x.HomestayId == id);
                await _policyRepo.DeleteRangeAsync(oldPolicies);
                if (dto.Policies?.Any() == true)
                {
                    var newPolicies = dto.Policies.Select(p => new HomestayPolicy
                    {
                        HomestayId = id,
                        PolicyId = p.PolicyId,
                        IsAllowed = p.IsAllowed
                    });
                    await _policyRepo.AddRangeAsync(newPolicies);
                }

                // Neighbourhoods
                var oldNeighbourhoods = await _neighbourhoodRepo.FindAsync(x => x.HomestayId == id);
                await _neighbourhoodRepo.DeleteRangeAsync(oldNeighbourhoods);
                if (dto.Neighbourhoods?.Any() == true)
                {
                    var newNeighbourhoods = dto.Neighbourhoods.Select(n => new HomestayNeighbourhood
                    {
                        HomestayId = id,
                        NeighbourhoodId = n.NeighbourhoodId
                    });
                    await _neighbourhoodRepo.AddRangeAsync(newNeighbourhoods);
                }

                // Images
                var oldImages = await _imageRepo.FindAsync(x => x.HomestayId == id);
                await _imageRepo.DeleteRangeAsync(oldImages);
                if (dto.Images?.Any() == true)
                {
                    var newImages = dto.Images.Select(i => new HomestayImage
                    {
                        HomestayId = id,
                        ImageUrl = i.ImageUrl,
                        SortOrder = i.SortOrder
                    });
                    await _imageRepo.AddRangeAsync(newImages);
                }

                await _homestayRepo.SaveChangesAsync();
                await transaction.CommitAsync();

                return homestay;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> CheckValidHomestay(int id)
        {
            var homestay = await _homestayRepo.GetWithConditionAsync(h => h.HomestayId == id);
            return homestay != null;
        }

        public async Task<Homestay> CreateHomestayAsync(Homestay homestay)
        {
            if (homestay == null)
            {
                throw new ArgumentNullException(nameof(homestay), "Homestay cannot be null.");
            }
            var owner = await _userManager.FindByIdAsync(homestay.OwnerId);
            if (owner == null)
            {
                throw new ArgumentException($"Owner with ID {homestay.OwnerId} does not exist.", nameof(homestay.OwnerId));
            }
            if (!await _userManager.IsInRoleAsync(owner, "Owner"))
            {
                throw new ArgumentException($"User with ID {homestay.OwnerId} is not authorized to create a homestay. Must have 'Owner' role.", nameof(homestay.OwnerId));
            }
            var homestayType = await _homestayTypeRepo.GetAsync(homestay.HomestayTypeId);
            if (homestayType == null)
            {
                throw new ArgumentException($"HomestayType with ID {homestay.HomestayTypeId} does not exist.", nameof(homestay.HomestayTypeId));
            }
            var ward = await _wardRepo.GetAsync(homestay.WardId);
            if (ward == null)
            {
                throw new ArgumentException($"Ward with ID {homestay.WardId} does not exist.", nameof(homestay.WardId));
            }

            _homestayRepo.AddAsync(homestay);
            return homestay;
        }
    }

}
