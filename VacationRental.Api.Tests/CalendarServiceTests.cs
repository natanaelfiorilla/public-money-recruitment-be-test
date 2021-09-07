using System;
using System.Collections.Generic;
using Moq;
using VacationRental.ApplicationServices.Services;
using VacationRental.Domain.Entities;
using VacationRental.DomainServices;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class CalendarServiceTests
    {
        public CalendarServiceTests(IntegrationFixture fixture)
        {
            
        }

        [Fact]
        public void GivenNightsNonPositive_WhenGetCalendar_ThenErrorReturns()
        {
            var rentalId = 1;
            var startDate = DateTime.Now;
            var negativeNigths = -1;

            var mockBookingRep = new Mock<IBookingRep>();
            var mockRentalRep = new Mock<IRentalRep>();

            var calendarService = new CalendarService(mockBookingRep.Object, mockRentalRep.Object);

            var getCalendarResult = calendarService.GetCalendar(rentalId, startDate, negativeNigths);

            Assert.True(getCalendarResult.HasErrors());
            Assert.Contains("Nights must be positive", getCalendarResult.Errors);
        }

        [Fact]
        public void GivenStartLowerThanMinValue_WhenGetCalendar_ThenErrorReturns()
        {
            var rentalId = 1;
            var startDate = DateTime.MinValue;
            var nights = 1;

            var mockBookingRep = new Mock<IBookingRep>();
            var mockRentalRep = new Mock<IRentalRep>();

            var calendarService = new CalendarService(mockBookingRep.Object, mockRentalRep.Object);

            var getCalendarResult = calendarService.GetCalendar(rentalId, startDate, nights);

            Assert.True(getCalendarResult.HasErrors());
            Assert.Contains($"Start must me grater than {DateTime.MinValue}", getCalendarResult.Errors);
        }

        [Fact]
        public void GivenStartGratherThanMinValue_WhenGetCalendar_ThenErrorReturns()
        {
            var rentalId = 1;
            var startDate = DateTime.MaxValue;
            var nights = 1;

            var mockBookingRep = new Mock<IBookingRep>();
            var mockRentalRep = new Mock<IRentalRep>();

            var calendarService = new CalendarService(mockBookingRep.Object, mockRentalRep.Object);

            var getCalendarResult = calendarService.GetCalendar(rentalId, startDate, nights);

            Assert.True(getCalendarResult.HasErrors());
            Assert.Contains($"Start must be lower than {DateTime.MaxValue}", getCalendarResult.Errors);
        }

        [Fact]
        public void GivenNoExistingRental_WhenGetCalendar_ThenErrorReturns()
        {
            var rentalId = 9999;
            var startDate = DateTime.Now;
            var nights = 1;

            var mockBookingRep = new Mock<IBookingRep>();
            var mockRentalRep = new Mock<IRentalRep>();
            mockRentalRep.Setup(r => r.Exists(It.IsAny<int>())).Returns(false);

            var calendarService = new CalendarService(mockBookingRep.Object, mockRentalRep.Object);

            var getCalendarResult = calendarService.GetCalendar(rentalId, startDate, nights);

            Assert.True(getCalendarResult.HasErrors());
            Assert.Contains("Rental not found", getCalendarResult.Errors);
        }

        [Fact]
        public void GivenADateWithNoBookings_WhenGetCalendar_ThenCalendarBookingEmptyReturns()
        {
            var rentalId = 1;
            var startDate = DateTime.Now;
            var nights = 1;

            var mockBookingRep = new Mock<IBookingRep>();
            mockBookingRep.Setup(b => b.GetBookingsByRentalDate(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new List<Booking>());

            var mockRentalRep = new Mock<IRentalRep>();
            mockRentalRep.Setup(r => r.Exists(It.IsAny<int>())).Returns(true);

            var calendarService = new CalendarService(mockBookingRep.Object, mockRentalRep.Object);

            var getCalendarResult = calendarService.GetCalendar(rentalId, startDate, nights);

            Assert.NotNull(getCalendarResult.Value);
            Assert.Equal(rentalId, getCalendarResult.Value.RentalId);
            Assert.NotNull(getCalendarResult.Value.Dates);
            Assert.NotEmpty(getCalendarResult.Value.Dates);
            Assert.Single(getCalendarResult.Value.Dates);
            Assert.Equal(startDate.Date, getCalendarResult.Value.Dates[0].Date);
            Assert.Empty(getCalendarResult.Value.Dates[0].Bookings);
            Assert.Empty(getCalendarResult.Value.Dates[0].PreparationTimes);
        }

        [Fact]
        public void GivenADate_WhenGetCalendar_ThenCalendarBookingReturns()
        {
            var rentalId = 1;
            var startDate = DateTime.Now;
            var nights = 1;

            var fakeBookings = new List<Booking>()
            {
                new Booking(){Id = 1, RentalId = 1, Start = DateTime.Now,            Nights = 1, Unit = 1},
                new Booking(){Id = 2, RentalId = 1, Start = DateTime.Now.AddDays(1), Nights = 1, Unit = 1, IsPreparationTime = true},
                new Booking(){Id = 3, RentalId = 1, Start = DateTime.Now.AddDays(2), Nights = 1, Unit = 2},
                new Booking(){Id = 4, RentalId = 1, Start = DateTime.Now.AddDays(3), Nights = 1, Unit = 2, IsPreparationTime = true},
                new Booking(){Id = 5, RentalId = 1, Start = DateTime.Now.AddDays(4), Nights = 1, Unit = 3}
            };


            var mockBookingRep = new Mock<IBookingRep>();
            mockBookingRep.Setup(b => b.GetBookingsByRentalDate(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(fakeBookings );

            var mockRentalRep = new Mock<IRentalRep>();
            mockRentalRep.Setup(r => r.Exists(It.IsAny<int>())).Returns(true);

            var calendarService = new CalendarService(mockBookingRep.Object, mockRentalRep.Object);

            var getCalendarResult = calendarService.GetCalendar(rentalId, startDate, nights);

            Assert.NotNull(getCalendarResult.Value);
            Assert.Equal(rentalId, getCalendarResult.Value.RentalId);
            Assert.NotNull(getCalendarResult.Value.Dates);
            Assert.NotEmpty(getCalendarResult.Value.Dates);
            Assert.Single(getCalendarResult.Value.Dates);
            Assert.Equal(startDate.Date, getCalendarResult.Value.Dates[0].Date);
            Assert.Equal(3, getCalendarResult.Value.Dates[0].Bookings.Count);
            Assert.Equal(2, getCalendarResult.Value.Dates[0].PreparationTimes.Count);
            Assert.Contains(getCalendarResult.Value.Dates[0].PreparationTimes,
                            p => p.Unit == getCalendarResult.Value.Dates[0].Bookings[0].Unit);
        }
    }
}
