using System;
using VacationRental.Common;
using VacationRental.Domain.Entities;

namespace VacationRental.ApplicationServices
{
    public interface ICalendarService
    {
        OperationResult<Calendar> GetCalendar(int rentalId, DateTime start, int nights);
    }
}
