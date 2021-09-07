using System;
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

        public IEnumerable<Booking> GetBookingsByRentalDate(int rentalId, DateTime date)
        {
            try
            {
                return _context.Bookings.Values.Where(b => b.RentalId == rentalId &&
                                                b.Start <= date &&
                                                b.Start.AddDays(b.Nights) > date).ToList();
            }
            catch
            {
                //Add some logging
            }

            return new List<Booking>();
        }

        public IEnumerable<Booking> GetBookingsByRentalRange(int rentalId, DateTime date, int nights)
        {
            try
            {
                return _context.Bookings.Values.Where(b => b.RentalId == rentalId 
                                                        && ((b.Start <= date && b.Start.AddDays(b.Nights) > date)
                                                        || (b.Start < date.AddDays(nights) && b.Start.AddDays(b.Nights) >= date.AddDays(nights))
                                                        || (b.Start > date && b.Start.AddDays(b.Nights) < date.AddDays(nights)))).ToList();                
            }
            catch
            {
                //Add some logging
            }

            return new List<Booking>();
        }

        public IEnumerable<Booking> GetBookingsByRentalUnitRange(int rentalId, int unit, DateTime date, int nights)
        {
            try
            {
                return _context.Bookings.Values.Where(b => b.RentalId == rentalId
                                                        && b.Unit == unit
                                                        && ((b.Start <= date && b.Start.AddDays(b.Nights) > date)
                                                        || (b.Start < date.AddDays(nights) && b.Start.AddDays(b.Nights) >= date.AddDays(nights))
                                                        || (b.Start > date && b.Start.AddDays(b.Nights) < date.AddDays(nights)))).ToList();
            }
            catch
            {
                //Add some logging
            }

            return new List<Booking>();
        }

        public IEnumerable<Booking> GetBookingsByRentalUnitFromDate(int rentalId, int unit, DateTime date)
        {
            try
            {
                return _context.Bookings.Values.Where(b => b.RentalId == rentalId &&
                                                b.Unit == unit &&
                                                (b.Start <= date &&
                                                b.Start.AddDays(b.Nights) > date ||
                                                b.Start > date)).ToList();
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

        public IEnumerable<Booking> GetPreparationTimesByRentalDate(int rentaId, DateTime date)
        {
            try
            {
                return _context.Bookings.Values.Where(b => b.RentalId == rentaId &&
                                                b.Start <= date &&
                                                b.Start.AddDays(b.Nights) > date
                                                && b.IsPreparationTime == true).ToList();
            }
            catch
            {
                //Add some logging
            }

            return new List<Booking>();
        }

        public IEnumerable<Booking> GetPreparationTimesByRentalFromDate(int rentaId, DateTime date)
        {
            try
            {
                return _context.Bookings.Values.Where(b => b.RentalId == rentaId &&
                                                (b.Start <= date &&
                                                 b.Start.AddDays(b.Nights) > date ||
                                                 b.Start > date )
                                                && b.IsPreparationTime == true).ToList();
            }
            catch
            {
                //Add some logging
            }

            return new List<Booking>();
        }

        public void Update(Booking bookingUpdate)
        {
            try
            {
                _context.Bookings.Remove(bookingUpdate.Id);
                _context.Bookings.Add(bookingUpdate.Id, bookingUpdate);
            }
            catch
            {
                //Add some logging
                throw;
            }
        }
    }
}
