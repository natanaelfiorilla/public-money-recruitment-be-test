using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.ApplicationServices;
using VacationRental.Common;
using VacationRental.Domain.Entities;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;
        private readonly IMapper _mapper;

        public CalendarController(ICalendarService calendarService,
                                  IMapper mapper)
        {
            _calendarService = calendarService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<CalendarViewModel> Get(int rentalId, DateTime start, int nights)
        {
            if (nights <= 0) return BadRequest("Nights must be positive");
            if (start < DateTime.MinValue) return BadRequest($"Start must me grater than {DateTime.MinValue}");
            if (start > DateTime.MaxValue) return BadRequest($"Start must be lower than {DateTime.MaxValue}");

            OperationResult<Calendar> result = _calendarService.GetCalendar(rentalId, start, nights);

            if (result == null) throw new Exception("Internal Server Error.");

            if (!result)
            {
                if (result.HasErrors()) return BadRequest(result.Errors);

                if (result.IsException()) throw new Exception("Internal Server Error.");
            }

            if (result.Value == null) return NotFound();

            return _mapper.Map<Calendar, CalendarViewModel>(result.Value);
        }
    }
}
