using System;
using System.Threading.Tasks;
using VacationRental.Common;
using VacationRental.Domain.Entities;
using VacationRental.DomainServices;

namespace VacationRental.ApplicationServices.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRep _bookingRep;
        private readonly IRentalRep _rentalRep;

        public BookingService(IBookingRep bookingRep,
                              IRentalRep rentalRep)
        {
            _bookingRep = bookingRep;
            _rentalRep = rentalRep;
        }

        public OperationResult<Booking> Create(Booking bookingNew)
        {
            OperationResult<Booking> result = OperationResultHelpers.Ok<Booking>();

            try
            {
                Validate(bookingNew, result);

                if (result)
                {
                    ValidateAvailability(bookingNew, result);

                    if (result)
                    {
                        var booking = new Booking()
                        {
                            Id = _bookingRep.Count() + 1,
                            Nights = bookingNew.Nights,
                            RentalId = bookingNew.RentalId,
                            Start = bookingNew.Start
                        };

                        _bookingRep.Add(booking);

                        result.Value = booking;
                    }

                }
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            return result;
        }

        public OperationResult<Booking> GetBooking(int bookingId)
        {
            OperationResult<Booking> result = OperationResultHelpers.Ok<Booking>();

            try
            {
                result.Value = _bookingRep.GetById(bookingId);
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            return result;
        }

        private void Validate(Booking bookingNew, OperationResult<Booking> result)
        {
            if (!_rentalRep.Exists(bookingNew.RentalId)) result.AddError("Rental not found");
        }

        private void ValidateAvailability(Booking bookingNew, OperationResult<Booking> result)
        {
            //This For has no sense to me.
            for (var i = 0; i < bookingNew.Nights; i++)
            {
                var count = 0;

                var bookings = _bookingRep.GetAll();

                var rental = _rentalRep.GetById(bookingNew.RentalId);

                if (rental != null &&
                    bookings != null)
                {
                    foreach (var booking in bookings)
                    {
                        if (booking.RentalId == bookingNew.RentalId
                            && (booking.Start <= bookingNew.Start.Date && booking.Start.AddDays(booking.Nights) > bookingNew.Start.Date)
                            || (booking.Start < bookingNew.Start.AddDays(bookingNew.Nights) && booking.Start.AddDays(booking.Nights) >= bookingNew.Start.AddDays(bookingNew.Nights))
                            || (booking.Start > bookingNew.Start && booking.Start.AddDays(booking.Nights) < bookingNew.Start.AddDays(bookingNew.Nights)))
                        {
                            count++;
                        }
                    }

                    if (count >= rental.Units) result.AddError("Not available");
                }
            }
        }
    }
}
