using System.Collections.Generic;
using System.Linq;
using VacationRental.Domain.Entities;
using VacationRental.DomainServices;

namespace VacationRental.Infraestructure
{
    public class BookingRep: IBookingRep
    {
        private readonly VacationRentalContext _context;

        public BookingRep(VacationRentalContext context)
        {
            _context = context;
        }

        public void Add(Booking newObject)
        {
            _context.Bookings.Add(newObject.Id, newObject);
        }

        public int Count()
        {
            try
            {
                if (_context != null &&
                    _context.Bookings != null)
                {                    
                    return _context.Bookings.Keys.Count();
                }
            }
            catch
            {
                //Add some logging
            }

            return 0;
        }

        public bool Exists(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Booking> GetAll()
        {
            try
            {
                if (_context != null &&
                    _context.Bookings != null)
                {
                    return _context.Bookings.Values.ToList();
                }
            }
            catch
            {
                //Add some logging
            }

            return new List<Booking>();
        }

        public Booking GetById(int id)
        {
            try
            {
                if (_context != null &&
                    _context.Bookings != null)
                {
                    return _context.Bookings[id];
                }
            }
            catch(KeyNotFoundException)
            {
                //Not Found
                return null;
            }
            catch
            {
                //Add some logging
            }
            
            return null;
        }
    }
}
