using System;
using VacationRental.Common;
using VacationRental.Domain.Entities;
using VacationRental.DomainServices;

namespace VacationRental.ApplicationServices.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRep _rentalRep;

        public RentalService(IRentalRep rentalRep)
        {
            _rentalRep = rentalRep;
        }

        public OperationResult<Rental> Create(Rental rentalNew)
        {
            OperationResult<Rental> result = OperationResultHelpers.Ok<Rental>();

            try
            {
                Validate(rentalNew, result);

                if (result)
                {
                    var rental = new Rental()
                    {
                        Id = _rentalRep.Count() + 1,
                        Units = rentalNew.Units,
                        PreparationTimeInDays = rentalNew.PreparationTimeInDays
                    };

                    _rentalRep.Add(rental);

                    result.Value = rental;
                }
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            return result;
        }

        public OperationResult<Rental> GetRental(int rentalId)
        {
            OperationResult<Rental> result = OperationResultHelpers.Ok<Rental>();

            try
            {
                result.Value = _rentalRep.GetById(rentalId);
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            return result;
        }

        private void Validate(Rental rentalNew, OperationResult<Rental> result)
        {
            if (rentalNew.Units <= 0) result.AddError("Units be positive");
            if (rentalNew.PreparationTimeInDays < 0) result.AddError("Preparation Time must be positive");
        }
    }
}
