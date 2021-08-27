using System;
using System.Collections.Generic;
using VacationRental.Common;
using VacationRental.Domain.Entities;
using VacationRental.DomainServices;

namespace VacationRental.ApplicationServices.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IBookingRep _bookingRep;
        private readonly IRentalRep _rentalRep;

        public CalendarService(IBookingRep bookingRep,
                              IRentalRep rentalRep)
        {
            _bookingRep = bookingRep;
            _rentalRep = rentalRep;
        }

        public OperationResult<Calendar> GetCalendar(int rentalId, DateTime start, int nights)
        {
            OperationResult<Calendar> result = OperationResultHelpers.Ok<Calendar>();

            try
            {
                Validate(rentalId, start, nights, result);

                if (result)
                {
                    var calendar = new Calendar()
                    {
                        RentalId = rentalId,
                        Dates = new List<CalendarDate>()
                    };

                    for (var i = 0; i < nights; i++)
                    {
                        var date = new CalendarDate()
                        {
                            Date = start.Date.AddDays(i),
                            Bookings = new List<CalendarBooking>(),
                            PreparationTimes = new List<CalendarBooking>()
                        };

                        var bookingsForRentalDate = _bookingRep.GetBookingsByRentalDate(rentalId, date.Date);

                        foreach (var booking in bookingsForRentalDate)
                        {
                            if(!booking.IsPreparationTime)
                            {
                                date.Bookings.Add(new CalendarBooking
                                {
                                    Id = booking.Id,
                                    Unit = booking.Unit
                                });
                            }
                            else
                            {
                                date.PreparationTimes.Add(new CalendarBooking
                                {
                                    Unit = booking.Unit
                                });
                            }

                        }

                        calendar.Dates.Add(date);
                    }

                    result.Value = calendar;
                }
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            return result;
        }

        private void Validate(int rentalId, DateTime start, int nights, OperationResult<Calendar> result)
        {
            if (nights <= 0) result.AddError("Nights must be positive");
            if (start < DateTime.MinValue) result.AddError($"Start must me grater than {DateTime.MinValue}");
            if (start > DateTime.MaxValue) result.AddError($"Start must be lower than {DateTime.MaxValue}");
            if (!_rentalRep.Exists(rentalId)) result.AddError("Rental not found");
        }
    }
}
