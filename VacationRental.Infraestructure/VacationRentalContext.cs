using System.Collections.Generic;
using VacationRental.Domain.Entities;

namespace VacationRental.Infraestructure
{
    public class VacationRentalContext
    {
        private readonly IDictionary<int, Rental> _rentals;
        private readonly IDictionary<int, Booking> _bookings;

        public IDictionary<int, Rental> Rentals { get { return _rentals; } }
        public IDictionary<int, Booking> Bookings { get { return _bookings; } }

        public VacationRentalContext()
        {
            _rentals = new Dictionary<int, Rental>();
            _bookings = new Dictionary<int, Booking>();
        }        
    }
}
