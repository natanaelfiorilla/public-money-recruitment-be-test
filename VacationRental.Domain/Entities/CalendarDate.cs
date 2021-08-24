using System;
using System.Collections.Generic;

namespace VacationRental.Domain.Entities
{
    public class CalendarDate
    {
        public DateTime Date { get; set; }
        public List<CalendarBooking> Bookings { get; set; }

        public CalendarDate()
        {
        }
    }
}
