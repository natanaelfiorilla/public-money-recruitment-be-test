using AutoMapper;
using VacationRental.Domain.Entities;

namespace VacationRental.Api.Models
{
    public class VacationRentalMappingProfile : Profile
    {
        public VacationRentalMappingProfile()
        {
            CreateMap<Booking, BookingViewModel>();
            CreateMap<BookingBindingModel, Booking>();
            CreateMap<Booking, ResourceIdViewModel>();
            CreateMap<Rental, RentalViewModel>();
            CreateMap<RentalBindingModel, Rental>();
            CreateMap<Rental,ResourceIdViewModel>();
        }
    }
}
