﻿using System;
using System.Collections.Generic;
using VacationRental.Domain.Entities;

namespace VacationRental.DomainServices
{
    public interface IBookingRep : IRep<Booking>
    {
        IEnumerable<Booking> GetBookingsByRentalDate(int rentaId, DateTime date);
        IEnumerable<Booking> GetBookingsByRentalRange(int rentaId, DateTime date, int nights);
    }
}
