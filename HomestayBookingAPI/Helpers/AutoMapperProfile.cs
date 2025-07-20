using AutoMapper;
using BusinessObjects.Bookings;
using BusinessObjects.Homestays;
using BusinessObjects.Rooms;
using DTOs.Bookings;
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

            CreateMap<Room, GetRoomForBookingResponseDTO>()
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src =>
                src.RoomAmenities.Select(ra => ra.Amenity.Name).ToList()))
            .ForMember(dest => dest.RoomBeds, opt => opt.MapFrom(src =>
                src.RoomBeds.Select(rb => $"{rb.Quantity} {rb.BedType.Name}").ToList()))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                src.RoomPrices.FirstOrDefault().AmountPerNight));

        }
    }
}
