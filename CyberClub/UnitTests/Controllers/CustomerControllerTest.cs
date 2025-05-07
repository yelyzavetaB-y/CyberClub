using CyberClub.Controllers.Customer;
using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Helper;

namespace UnitTests.Controllers
{

    public class CustomerControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo = new();
        private readonly Mock<IZoneRepository> _mockZoneRepo = new();
        private readonly Mock<ISeatRepository> _mockSeatRepo = new();
        private readonly Mock<IBookingRepository> _mockBookingRepo = new();

        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            var userService = new UserService(_mockUserRepo.Object);
            var zoneService = new ZoneService(_mockZoneRepo.Object);
            var seatService = new SeatService(_mockSeatRepo.Object, _mockBookingRepo.Object, _mockZoneRepo.Object);
            var bookingService = new BookingService(_mockBookingRepo.Object, _mockSeatRepo.Object);

            _controller = new CustomerController(userService, zoneService, seatService, bookingService);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new DummySession();
            httpContext.Session.SetInt32("UserID", 1);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task FinalizeBooking_Success_RedirectsToCustomer()
        {
            // Arrange
            int testSeatId = 1;
            int testZoneId = 1;
            var model = new BookingViewModel
            {
                SelectedSeatId = testSeatId,
                UserID = 1,
                SelectedZoneId = testZoneId,
                SelectedDate = DateTime.Today,
                SelectedTime = new TimeSpan(12, 0, 0),
                Duration = 60
            };

            var startTime = model.SelectedDate.Date + model.SelectedTime;

            _mockSeatRepo
                .Setup(s => s.GetSeatByIdAsync(testSeatId))
                .ReturnsAsync(new Seat
                {
                    SeatID = testSeatId,
                    ZoneID = testZoneId,
                    SeatNumber = "A1",
                    IsAvailable = true
                });

            _mockSeatRepo
                .Setup(s => s.FindAvailableSeatAsync(testZoneId, startTime, model.Duration))
                .ReturnsAsync(new List<Seat> { new Seat { SeatID = testSeatId } });

            _mockBookingRepo
                .Setup(b => b.AddBookingAsync(It.IsAny<Booking>()))
                .ReturnsAsync(true);

            _mockSeatRepo
                .Setup(s => s.UpdateSeatAvailabilityAsync(testSeatId, false))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.FinalizeBooking(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Customer", redirectResult.ActionName);
        }


        [Fact]
        public async Task FinalizeBooking_BookingFails_ReturnsViewWithError()
        {
            var model = new BookingViewModel
            {
                SelectedSeatId = 1,
                UserID = 1,
                SelectedZoneId = 1,
                SelectedDate = DateTime.Today,
                SelectedTime = new TimeSpan(14, 0, 0),
                Duration = 60
            };

            var startTime = model.SelectedDate.Date + model.SelectedTime;

            _mockSeatRepo.Setup(s => s.GetSeatByIdAsync(model.SelectedSeatId))
                .ReturnsAsync(new Seat { SeatID = 1, ZoneID = 1 });

            _mockSeatRepo.Setup(s => s.FindAvailableSeatAsync(1, startTime, model.Duration))
                .ReturnsAsync(new List<Seat> { new Seat { SeatID = 2 } }); 

            _mockBookingRepo.Setup(b => b.AddBookingAsync(It.IsAny<Booking>()))
                .ReturnsAsync(false);

            _mockZoneRepo.Setup(z => z.GetAllZonesAsync()).ReturnsAsync(new List<Zone>());
            _mockSeatRepo.Setup(s => s.GetSeatsByZoneIdAsync(1)).ReturnsAsync(new List<Seat>());

            var result = await _controller.FinalizeBooking(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CustomerPanel", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task FinalizeBooking_InvalidModel_ReturnsViewWithErrors()
        {
            var model = new BookingViewModel
            {
                SelectedSeatId = 0,
                UserID = 0,
                SelectedZoneId = 1
            };

            _controller.ModelState.AddModelError("error", "missing input");

            _mockZoneRepo.Setup(z => z.GetAllZonesAsync()).ReturnsAsync(new List<Zone>());
            _mockSeatRepo.Setup(s => s.GetSeatsByZoneIdAsync(1)).ReturnsAsync(new List<Seat>());

            var result = await _controller.FinalizeBooking(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("CustomerPanel", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
        }
    }
}

