using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Domain.Entities;
using VacationRental.DomainServices;

namespace VacationRental.Infraestructure
{
    public class RentalRep: IRentalRep
    {
        private readonly VacationRentalContext _context;

        public RentalRep(VacationRentalContext context)
        {
            _context = context;
        }

        public void Add(Rental newObject)
        {
            try
            {
                _context.Rentals.Add(newObject.Id, newObject);
            }
            catch
            {
                //Add some logging
                throw;
            }
        }

        public int Count()
        {
            try
            {
                if (_context != null &&
                    _context.Rentals != null)
                {
                    return _context.Rentals.Keys.Count();
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
            try
            {
                if (_context != null &&
                    _context.Rentals != null)
                {
                    return _context.Rentals.ContainsKey(id);
                }
            }
            catch
            {
                //Add some logging
            }

            return false;
        }

        public IEnumerable<Rental> GetAll()
        {
            throw new NotImplementedException();
        }

        public Rental GetById(int id)
        {
            try
            {
                if (_context != null &&
                    _context.Rentals != null)
                {
                    return _context.Rentals[id];
                }
            }
            catch (KeyNotFoundException)
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

        public void Update(Rental rentalUpdate)
        {
            try
            {
                _context.Rentals.Remove(rentalUpdate.Id);
                _context.Rentals.Add(rentalUpdate.Id, rentalUpdate);
            }
            catch
            {
                //Add some logging
                throw;
            }            
        }
    }
}
