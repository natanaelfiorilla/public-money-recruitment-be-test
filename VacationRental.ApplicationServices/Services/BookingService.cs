using System;
using System.Threading.Tasks;
using VacationRental.Common;
using VacationRental.Domain.Entities;
using VacationRental.DomainServices;

namespace VacationRental.ApplicationServices.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRep _bookingRep;
        private readonly IRentalRep _rentalRep;
        private readonly IAvailabilityService _availabilityService;

        public BookingService(IBookingRep bookingRep,
                              IRentalRep rentalRep,
                              IAvailabilityService availabilityService)
        {
            _bookingRep = bookingRep;
            _rentalRep = rentalRep;
            _availabilityService = availabilityService;
        }

        public OperationResult<Booking> Create(Booking bookingNew)
        {
            OperationResult<Booking> result = OperationResultHelpers.Ok<Booking>();

            try
            {
                Validate(bookingNew, result);

                if (result)
                {
                    _availabilityService.ValidateAvailability(bookingNew, result);

                    if (result)
                    {
                        var availableUnit = _availabilityService.RetreiveAvailableUnit(bookingNew, result);

                        var rentalPreparationTime = GetRentalPreparationTime(bookingNew.RentalId, result);

                        if (result)
                        {
                            var booking = new Booking()
                            {
                                Id = _bookingRep.Count() + 1,
                                Nights = bookingNew.Nights,
                                RentalId = bookingNew.RentalId,
                                Start = bookingNew.Start,
                                Unit = availableUnit
                            };

                            _bookingRep.Add(booking);

                            var preparationTime = new Booking()
                            {
                                Id = _bookingRep.Count() + 1,
                                Nights = rentalPreparationTime,
                                RentalId = booking.RentalId,
                                Start = booking.Start.AddDays(booking.Nights),
                                Unit = booking.Unit,
                                IsPreparationTime = true
                            };

                            _bookingRep.Add(preparationTime);

                            result.Value = booking;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            return result;
        }

        public OperationResult<Booking> GetBooking(int bookingId)
        {
            OperationResult<Booking> result = OperationResultHelpers.Ok<Booking>();

            try
            {
                result.Value = _bookingRep.GetById(bookingId);
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            return result;
        }

        public void UpdatePreparationTimes(int rentalId, int preparationTimeInDays, OperationResult result)
        {
            try
            {
                _availabilityService.ValidatePreparationTimeChange(rentalId, preparationTimeInDays, result);

                if(result)
                {
                    foreach (var preparationTime in _bookingRep.GetPreparationTimesByRentalFromDate(rentalId, DateTime.Now.Date))
                    {
                        preparationTime.Nights = preparationTimeInDays;
                        _bookingRep.Update(preparationTime);
                    }                   
                }                
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            } 
        }

        private int GetRentalPreparationTime(int rentalId, OperationResult result)
        {
            try
            {
                var rental = _rentalRep.GetById(rentalId);

                if (rental != null)
                {
                    return rental.PreparationTimeInDays;
                }

                result.AddError("Rental not found");
            }
            catch (Exception ex)
            {
                result.AddException(ex);
            }

            return 0;
        }

        private void Validate(Booking bookingNew, OperationResult<Booking> result)
        {
            if (!_rentalRep.Exists(bookingNew.RentalId)) result.AddError("Rental not found");
            if (bookingNew.Nights <= 0) result.AddError("Nights must be positive");
            if (bookingNew.Start.Date <= DateTime.MinValue.Date) result.AddError($"Start must me grater than {DateTime.MinValue}");
            if (bookingNew.Start.Date >= DateTime.MaxValue.Date) result.AddError($"Start must be lower than {DateTime.MaxValue}");
        }
    }
}
