using AutoMapper;
using BusinessObjects.Homestays;
using DTOs.FavoriteHomestay;

namespace HomestayBookingAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateFavoriteHomestayDTO, FavoriteHomestay>();
        }
    }
}
