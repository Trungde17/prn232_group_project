using AutoMapper;
using BusinessObjects.Homestays;
using DTOs;

namespace HomestayBookingAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Homestay, HomestayListDTO>()
      .ForMember(dest => dest.HomestayName, opt => opt.MapFrom(src => src.Name))
      .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules))
      .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src =>
          src.StreetAddress + ", " +
          (src.Ward != null ? src.Ward.Name + ", " : "") +
          (src.Ward != null && src.Ward.District != null ? src.Ward.District.Name : "")
      ))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
      .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src =>
          src.HomestayImages != null && src.HomestayImages.Any()
              ? src.HomestayImages.First().ImageUrl
              : null
      ));


            CreateMap<Homestay, HomestayDetailDTO>()
     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.HomestayId))
     .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Ward != null ? src.Ward.Name : null))
     .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.Ward != null && src.Ward.District != null ? src.Ward.District.Name : null))
     .ForMember(dest => dest.HostPhone, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.PhoneNumber : null))
     .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>
         src.HomestayImages != null
             ? src.HomestayImages.Select(img => img.ImageUrl).ToList()
             : new List<string>()));
        }
    }
}
