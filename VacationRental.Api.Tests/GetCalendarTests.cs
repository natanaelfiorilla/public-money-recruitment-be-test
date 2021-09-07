using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VacationRental.Api.Controllers;
using VacationRental.Api.Models;
using VacationRental.ApplicationServices;
using VacationRental.Common;
using VacationRental.Domain.Entities;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class GetCalendarTests
    {
        private readonly HttpClient _client;

        public GetCalendarTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenRequestWithNightsNonPositive_WhenGetCalendar_ThenABadRequestReturns()
        {
            var rentalId = 1;
            var startDate = "2000-01-01";
            var negativeNigths = -1;

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={rentalId}&start={startDate}&nights={negativeNigths}"))
            {
                Assert.False(getCalendarResponse.IsSuccessStatusCode);
                Assert.True(getCalendarResponse.StatusCode == HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task GivenRequestWithStartLowerThanMinValue_WhenGetCalendar_ThenABadRequestReturns()
        {
            var rentalId = 1;
            var startDate = DateTime.MinValue;
            var nigths = 5;

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={rentalId}&start={startDate}&nights={nigths}"))
            {
                Assert.False(getCalendarResponse.IsSuccessStatusCode);
                Assert.True(getCalendarResponse.StatusCode == HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task GivenRequestWithStarGratherThanMaxValue_WhenGetCalendar_ThenABadRequestReturns()
        {
            var rentalId = 1;
            var startDate = DateTime.MaxValue;
            var nigths = 5;

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={rentalId}&start={startDate}&nights={nigths}"))
            {
                Assert.False(getCalendarResponse.IsSuccessStatusCode);
                Assert.True(getCalendarResponse.StatusCode == HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public void GivenRequestWithNoExistingRental_WhenGetCalendar_ThenABadRequestReturns()
        {
            var rentalId = 999999;
            var startDate = DateTime.Now;
            var nigths = 5;

            var mockCalendarService = new Mock<ICalendarService>();
            mockCalendarService.Setup(c => c.GetCalendar(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(OperationResultHelpers.Error("Rental not found", new Calendar()));

            var mockMapper = new Mock<IMapper>();

            var controller = new CalendarController(mockCalendarService.Object, mockMapper.Object);

            var getCalendarResponse = controller.Get(rentalId, startDate, nigths);

            Assert.IsType<BadRequestObjectResult>(getCalendarResponse.Result);
        }

        [Fact]
        public void GivenOkRequest_WhenGetCalendar_Then500ErrorReturns()
        {
            var rentalId = 1;
            var startDate = DateTime.Now;
            var nigths = 5;

            var mockCalendarService = new Mock<ICalendarService>();
            mockCalendarService.Setup(c => c.GetCalendar(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(OperationResultHelpers.ExceptionResult<Calendar>(new Exception()));

            var mockMapper = new Mock<IMapper>();

            var controller = new CalendarController(mockCalendarService.Object, mockMapper.Object);

            Assert.Throws<Exception>(() => controller.Get(rentalId, startDate, nigths));

        }

        [Fact]
        public void GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar()
        {
            var rentalId = 1;
            var startDate = DateTime.Now;
            var nigths = 5;

            var calendarFake = new Calendar()
            {
                RentalId = 1,
                Dates = new List<CalendarDate>()
                {
                    new CalendarDate() { Date = DateTime.Now,
                                         Bookings = new List<CalendarBooking>()
                                         {
                                            new CalendarBooking() { Id = 1, Unit = 1 }
                                         },
                                         PreparationTimes = new List<CalendarBooking>()
                                         {
                                            new CalendarBooking() { Unit = 1 }
                                         }
                                        }
                }
            };

            var mockCalendarService = new Mock<ICalendarService>();
            mockCalendarService.Setup(c => c.GetCalendar(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(OperationResultHelpers.Ok(calendarFake));

            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<VacationRentalMappingProfile>()));

            var controller = new CalendarController(mockCalendarService.Object, mapper);

            var getCalendarResult = controller.Get(rentalId, startDate, nigths);

            Assert.Equal(rentalId, getCalendarResult.Value.RentalId);
            Assert.Single(getCalendarResult.Value.Dates);

            Assert.Equal(startDate.Date, getCalendarResult.Value.Dates[0].Date.Date);
            Assert.Single(getCalendarResult.Value.Dates[0].Bookings);
            Assert.Single(getCalendarResult.Value.Dates[0].PreparationTimes);

            Assert.Equal(1, getCalendarResult.Value.Dates[0].Bookings[0].Unit);

            Assert.Contains(getCalendarResult.Value.Dates[0].PreparationTimes, x => x.Unit == getCalendarResult.Value.Dates[0].Bookings[0].Unit);
        }
    }
}
