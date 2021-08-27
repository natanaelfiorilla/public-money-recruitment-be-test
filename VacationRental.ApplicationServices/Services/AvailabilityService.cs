using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Common;
using VacationRental.Domain.Entities;
using VacationRental.DomainServices;

namespace VacationRental.ApplicationServices.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IBookingRep _bookingRep;
        private readonly IRentalRep _rentalRep;

        public AvailabilityService(IBookingRep bookingRep,
                              IRentalRep rentalRep)
        {
            _bookingRep = bookingRep;
            _rentalRep = rentalRep;
        }

        public int RetreiveAvailableUnit(Booking bookingNew, OperationResult result)
        {
            try
            {
                var rental = _rentalRep.GetById(bookingNew.RentalId);

                var bookings = _bookingRep.GetBookingsByRentalRange(bookingNew.RentalId, bookingNew.Start.Date, bookingNew.Nights);

                var occupiedUnits = bookings.Select(x => x.Unit).Distinct();

                for (int i = 1; i <= rental.Units; i++)
                {
                    if (occupiedUnits.Contains(i)) continue;

                    return i;
                }
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            result.AddError("No Available units found.");

            return 0;
        }

        public void ValidateAvailability(Booking bookingNew, OperationResult result)
        {
            try
            {
                //This For has no sense to me.
                for (var i = 0; i < bookingNew.Nights; i++)
                {
                    var count = 0;

                    var bookings = _bookingRep.GetBookingsByRentalRange(bookingNew.RentalId, bookingNew.Start.Date, bookingNew.Nights);

                    var rental = _rentalRep.GetById(bookingNew.RentalId);

                    var occupiedUnits = new List<int>();

                    if (rental != null &&
                        bookings != null)
                    {
                        foreach (var booking in bookings)
                        {
                            if (!occupiedUnits.Contains(booking.Unit))
                            {
                                occupiedUnits.Add(booking.Unit);
                                count++;
                            }
                        }

                        if (count >= rental.Units) result.AddError("Not available");
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }
        }
    }
}
