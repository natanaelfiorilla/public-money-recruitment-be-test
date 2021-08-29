using System.Threading.Tasks;
using VacationRental.Common;
using VacationRental.Domain.Entities;

namespace VacationRental.ApplicationServices
{
    public interface IBookingService
    {
        OperationResult<Booking> GetBooking(int bookingId);
        OperationResult<Booking> Create(Booking bookingNew);
        void UpdatePreparationTimes(int rentalId, int preparationTimeInDays, OperationResult result);
    }
}
