using AutoMapper;
using BusinessObjects;
using BusinessObjects.Bookings;
using BusinessObjects.Homestays;
using BusinessObjects.Rooms;
using DTOs;
using DTOs.Bookings;
using DTOs.FavoriteHomestay;
using DTOs.HomestayDtos;
using DTOs.RoomDtos;

namespace HomestayBookingAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateBookingDTO, Booking>()
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.BookingDetails, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Homestay, opt => opt.Ignore());

            CreateMap<CreateHomestayDTO, Homestay>()
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.HomestayType, opt => opt.Ignore())
            .ForMember(dest => dest.Ward, opt => opt.Ignore())
            .ForMember(dest => dest.Owner, opt => opt.Ignore());

            CreateMap<Homestay, GetHomestayDetailDTO>()
            .ForMember(dest => dest.HomestayTypeName, opt => opt.MapFrom(src => src.HomestayType.TypeName))
            .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Ward.Name))
            .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.Ward.District.Name))
            .ForMember(dest => dest.Policies, opt => opt.MapFrom(src =>
                src.HomestayPolicies.Select(p => new HomestayPolicyDTO
                {
                    PolicyName = p.Policy.Name,
                    IsAllowed = p.IsAllowed,
                }).ToList()))
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src =>
                src.HomestayAmenities.Select(a => a.Amenity.Name).ToList()))
            .ForMember(dest => dest.Neighbourhoods, opt => opt.MapFrom(src =>
                src.HomestayNeighbourhoods.Select(n => n.Neighbourhood.Name).ToList()))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>

                src.HomestayImages.Select(n => n.ImageUrl).ToList()
            ));
            CreateMap<CreateFavoriteHomestayDTO, FavoriteHomestay>();
            CreateMap<Homestay, HomestayListDTO>()
      .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
      .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules))
      .ForMember(dest => dest.StreetAddress, opt => opt.MapFrom(src =>
          src.StreetAddress + ", " +
          (src.Ward != null ? src.Ward.Name + ", " : "") +
          (src.Ward != null && src.Ward.District != null ? src.Ward.District.Name : "")
      ))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
      .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
          src.HomestayImages != null && src.HomestayImages.Any()
              ? src.HomestayImages.First().ImageUrl
              : null
      ));



            CreateMap<Homestay, HomestayListDTO>()
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules))
              .ForMember(dest => dest.StreetAddress, opt => opt.MapFrom(src =>
                  src.StreetAddress + ", " +
                  (src.Ward != null ? src.Ward.Name + ", " : "") +
                  (src.Ward != null && src.Ward.District != null ? src.Ward.District.Name : "")
              ))
              .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
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


            CreateMap<Room, GetRoomForBookingResponseDTO>()
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src =>
                src.RoomAmenities.Select(ra => ra.Amenity.Name).ToList()))
            .ForMember(dest => dest.RoomBeds, opt => opt.MapFrom(src =>
                src.RoomBeds.Select(rb => $"{rb.Quantity} {rb.BedType.Name}").ToList()))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                src.RoomPrices.FirstOrDefault().AmountPerNight));

            CreateMap<CreateOwnerAccountDTO, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CreatAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdateAt, opt => opt.Ignore())
            .ForMember(dest => dest.Homestays, opt => opt.Ignore())
            .ForMember(dest => dest.Feedbacks, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.FavoriteHomestays, opt => opt.Ignore());

        }

    }
}
