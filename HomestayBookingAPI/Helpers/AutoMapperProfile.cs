using AutoMapper;
using BusinessObjects.Homestays;
using DTOs;

namespace HomestayBookingAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Homestay, HomestayListDTO>();
            CreateMap<Homestay, HomestayDetailDTO>();
        }
    }
}
