using System;
using VacationRental.Common;
using VacationRental.Domain.Entities;

namespace VacationRental.ApplicationServices
{
    public interface IRentalService
    {
        //bool Exists(int id);

        //Rental GetById(int id);

        OperationResult<Rental> GetRental(int rentalId);
        OperationResult<Rental> Create(Rental rentalNew);
    }
}
