﻿using System;
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
                Validate(rentalId, result);

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
                            Bookings = new List<CalendarBooking>()
                        };

                        var bookingsForRentalDate = _bookingRep.GetBookingsByRentalDate(rentalId, date.Date);

                        foreach (var booking in bookingsForRentalDate)
                        {
                            date.Bookings.Add(new CalendarBooking { Id = booking.Id });
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

        private void Validate(int rentalId, OperationResult<Calendar> result)
        {
            if (!_rentalRep.Exists(rentalId)) result.AddError("Rental not found");
        }
    }
}
