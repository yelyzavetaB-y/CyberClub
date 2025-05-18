using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using CyberClub.Domain.Interfaces;
using CyberClub.Controllers;

namespace UnitTests.Controllers.GeneralController
{

    public class GeneralControllerTests
    {
        [Fact]
        public async Task UpdateExpiredBookingsSeatsValidKeyReturnsOk()
        {
            var bookingServiceMock = new Mock<IBookingService>();
            var controller = new CyberClub.Controllers.Api.GeneralController(bookingServiceMock.Object);
            string validKey = "d123d2378dg287dg23d78g";

            var result = await controller.UpdateExpiredBookingsSeats(validKey);

            Assert.IsType<OkResult>(result);
            bookingServiceMock.Verify(s => s.UpdateExpiredBookingsSeatsAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateExpiredBookingsSeatsValidKeyReturnsNotFound()
        {
            var bookingServiceMock = new Mock<IBookingService>();
            var controller = new CyberClub.Controllers.Api.GeneralController(bookingServiceMock.Object);
            string invalidKey = "invalid";

            var result = await controller.UpdateExpiredBookingsSeats(invalidKey);

            Assert.IsType<NotFoundResult>(result);
            bookingServiceMock.Verify(s => s.UpdateExpiredBookingsSeatsAsync(), Times.Never);
        }
    }
}
