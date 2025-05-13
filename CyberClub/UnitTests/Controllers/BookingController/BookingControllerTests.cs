using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using CyberClub.Controllers.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CyberClub.Api.Interfaces;

namespace UnitTests.Controllers.BookingController
{
    public class BookingControllerTests
    {
        [Fact]
        public async Task CancelBooking_ReturnsRedirectWhenSuccessTrue()
        {
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockSeatRepo = new Mock<ISeatRepository>();
            mockBookingRepo.Setup(r => r.CancelBookingAsync(1)).ReturnsAsync(true);

            var bookingService = new BookingService(mockBookingRepo.Object, mockSeatRepo.Object);
            var controller = new CyberClub.Controllers.Booking.BookingController(bookingService);

            var result = await controller.CancelBooking(1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyBookings", redirect.ActionName);
        }

        [Fact]
        public async Task CancelBooking_Failed_ReturnsRedirectToMyBookings_WithError()
        {
            
            var bookingRepoMock = new Mock<IBookingRepository>();
            var seatRepoMock = new Mock<ISeatRepository>();

            var bookingService = new BookingService(bookingRepoMock.Object, seatRepoMock.Object);

            var controller = new CyberClub.Controllers.Booking.BookingController(bookingService);

            int bookingId = 1;

            bookingRepoMock.Setup(s => s.CancelBookingAsync(bookingId)).ReturnsAsync(false);

            var result = await controller.CancelBooking(bookingId) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("MyBookings", result.ActionName);

            var modelStateErrors = controller.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            Assert.Contains("Failed", modelStateErrors);
        }
    }
}
