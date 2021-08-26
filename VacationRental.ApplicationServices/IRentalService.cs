using VacationRental.Common;
using VacationRental.Domain.Entities;

namespace VacationRental.ApplicationServices
{
    public interface IRentalService
    {
        OperationResult<Rental> GetRental(int rentalId);
        OperationResult<Rental> Create(Rental rentalNew);
    }
}
