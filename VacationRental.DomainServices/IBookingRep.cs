using System;
using System.Collections.Generic;
using VacationRental.Domain.Entities;

namespace VacationRental.DomainServices
{
    public interface IBookingRep : IRep<Booking>
    {
        IEnumerable<Booking> GetBookingsByRentalDate(int rentaId, DateTime date);

        IEnumerable<Booking> GetPreparationTimesByRentalDate(int rentalId, DateTime date);

        IEnumerable<Booking> GetBookingsByRentalRange(int rentalId, DateTime date, int nights);

        IEnumerable<Booking> GetBookingsByRentalUnitRange(int rentalId, int unit, DateTime date, int nights);

        IEnumerable<Booking> GetBookingsByRentalUnitFromDate(int rentalId, int unit, DateTime date);

        IEnumerable<Booking> GetPreparationTimesByRentalFromDate(int rentalId, DateTime date);
    }
}
