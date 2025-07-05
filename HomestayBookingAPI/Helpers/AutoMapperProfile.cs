using AutoMapper;
using BusinessObjects.Bookings;
using BusinessObjects.Homestays;
using DTOs.Bookings;
using DTOs.HomestayDtos;

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


        }
    }
}
