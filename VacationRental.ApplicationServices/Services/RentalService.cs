using System;
using VacationRental.Common;
using VacationRental.Domain.Entities;
using VacationRental.DomainServices;

namespace VacationRental.ApplicationServices.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRep _rentalRep;
        private readonly IAvailabilityService _availabilityService;
        private readonly IBookingService _bookingService;

        public RentalService(IRentalRep rentalRep,
                             IAvailabilityService availabilityService,
                             IBookingService bookingService)
        {
            _rentalRep = rentalRep;
            _availabilityService = availabilityService;
            _bookingService = bookingService;
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

        public OperationResult Update(int rentalId, Rental rentalUpdate)
        {
            OperationResult result = OperationResultHelpers.Ok();

            try
            {
                Validate(rentalUpdate, result);

                if (result)
                {
                    var rentalDb = _rentalRep.GetById(rentalId);

                    if (rentalDb != null)
                    {
                        ValidateBusiness(rentalDb, rentalUpdate, result);

                        if (result)
                        {
                            if (rentalUpdate.PreparationTimeInDays != rentalDb.PreparationTimeInDays)
                            {
                                _bookingService.UpdatePreparationTimes(rentalId, rentalUpdate.PreparationTimeInDays, result);
                            }

                            if (result)
                            {
                                rentalUpdate.Id = rentalId;
                                _rentalRep.Update(rentalUpdate);
                            }
                        }
                    }
                    else
                    {
                        result.AddError("Rental not found");
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            return result;
        }

        private void Validate(Rental rentalNew, OperationResult result)
        {
            if (rentalNew.Units <= 0) result.AddError("Units be positive");
            if (rentalNew.PreparationTimeInDays < 0) result.AddError("Preparation Time must be positive");
        }

        private void ValidateBusiness(Rental currentRental, Rental newRental, OperationResult result)
        {
            if (newRental.Units < currentRental.Units)
            {
                //Check if no more existing units has active bookings.
                for (int unit = newRental.Units + 1; unit <= currentRental.Units; unit++)
                {
                    _availabilityService.ValidateActiveBookingsForDeletedUnit(currentRental.Id, unit, result);
                }
            }
        }
    }
}
