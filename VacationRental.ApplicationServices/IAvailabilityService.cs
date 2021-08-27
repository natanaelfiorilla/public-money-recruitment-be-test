using VacationRental.Common;
using VacationRental.Domain.Entities;

namespace VacationRental.ApplicationServices.Services
{
    public interface IAvailabilityService
    {
        void ValidateAvailability(Booking bookingNew, OperationResult result);
        int RetreiveAvailableUnit(Booking bookingNew, OperationResult result);
    }
}
