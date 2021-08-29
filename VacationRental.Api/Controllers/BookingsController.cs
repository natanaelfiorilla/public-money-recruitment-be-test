using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.ApplicationServices;
using VacationRental.Common;
using VacationRental.Domain.Entities;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public BookingsController(IBookingService bookingService,
                                  IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public ActionResult<BookingViewModel> Get(int bookingId)
        {
            OperationResult<Booking> result = _bookingService.GetBooking(bookingId);

            if (result == null) throw new Exception("Internal Server Error.");

            if (!result)
            {
                if (result.HasErrors()) return BadRequest(result.Errors);
                
                if (result.IsException()) throw new Exception("Internal Server Error.");
            }

            if (result.Value == null) return NotFound();
         
            return _mapper.Map<Booking, BookingViewModel>(result.Value);            
        }

        [HttpPost]
        public ActionResult<ResourceIdViewModel> Post(BookingBindingModel model)
        {
            if (model == null) return BadRequest();
            if (model.Nights <= 0) return BadRequest("Nights must be positive");
            if (model.Start.Date <= DateTime.MinValue.Date) return BadRequest($"Start must me grater than {DateTime.MinValue}");
            if (model.Start.Date >= DateTime.MaxValue.Date) return BadRequest($"Start must be lower than {DateTime.MaxValue}");

            var bookingNew = _mapper.Map<BookingBindingModel, Booking>(model);

            OperationResult<Booking> result = _bookingService.Create(bookingNew);

            if (result == null) throw new Exception("Internal Server Error.");

            if (!result)
            {
                if (result.HasErrors()) return BadRequest(result.Errors);

                if (result.IsException()) throw new Exception("Internal Server Error.");
            }

            if (result.Value == null) throw new Exception("Internal Server Error.");

            return _mapper.Map<Booking, ResourceIdViewModel>(result.Value);
        }
    }
}
