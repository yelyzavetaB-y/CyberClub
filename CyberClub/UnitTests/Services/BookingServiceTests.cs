using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Domain.Models.Enum;
using CyberClub.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services
{
    public class BookingServiceTests
    {
        [Fact] 
        public async Task BookSeatAsync_ReturnsTrue_WhenSeatIsFree()
        {
            var seatRepo = new Mock<ISeatRepository>();
            var bookingRepo = new Mock<IBookingRepository>();
            var seatId = 1;
            var zoneId = 5;
            var userId = 42;
            var startTime = new DateTime(2025, 1, 1, 12, 0, 0); 
            var duration = 60;
            var mockValidator = new Mock<ICustomValidator<Booking>>();
            var testSeat = new Seat
            {
                SeatID = seatId,
                ZoneID = zoneId
            };

            seatRepo.Setup(x => x.GetSeatByIdAsync(seatId)).Returns(Task.FromResult(testSeat));
            bookingRepo.Setup(x => x.AddBookingAsync(It.Is<Booking>(b => b.SeatId == seatId && b.UserId == userId))).Returns(Task.FromResult(true));
            seatRepo.Setup(x => x.UpdateSeatAvailabilityAsync(seatId, false)).Returns(Task.FromResult(true));

            var service = new BookingService(bookingRepo.Object, seatRepo.Object, mockValidator.Object);

            var result = await service.BookSeatAsync(seatId, userId, startTime, duration);

            Assert.True(result);
        }


        [Fact]
        public async Task BookSeatAsync_ReturnsFalse_WhenSeatIsNotAvailable()
        {
            
            var seatRepo = new Mock<ISeatRepository>();
            var bookingRepo = new Mock<IBookingRepository>();
            var mockValidator = new Mock<ICustomValidator<Booking>>();
            Seat testSeat = new Seat();
            testSeat.SeatID = 1;
            testSeat.ZoneID = 5;

            seatRepo.Setup(x => x.GetSeatByIdAsync(1))
                    .Returns(Task.FromResult(testSeat));

            List<Seat> availableSeats = new List<Seat>(); 

            seatRepo.Setup(x => x.FindAvailableSeatAsync(5, It.IsAny<DateTime>(), 60))
                    .Returns(Task.FromResult(availableSeats));

            BookingService service = new BookingService(bookingRepo.Object, seatRepo.Object, mockValidator.Object);

            bool result = await service.BookSeatAsync(1, 42, DateTime.Now.AddHours(1), 60);

            Assert.False(result);
        }


        [Fact]
        public async Task BookSeatAsync_ReturnsFalse_WhenSeatDoesNotExist()
        {
            var seatRepo = new Mock<ISeatRepository>();
            var bookingRepo = new Mock<IBookingRepository>();
            var mockValidator = new Mock<ICustomValidator<Booking>>();
            seatRepo.Setup(r => r.GetSeatByIdAsync(1)).ReturnsAsync((Seat)null);

            var service = new BookingService(bookingRepo.Object, seatRepo.Object, mockValidator.Object);
            var result = await service.BookSeatAsync(1, 42, DateTime.Now, 60);

            Assert.False(result);
        }

        [Fact]
        public async Task BookSeatAsync_ReturnsFalse_WhenAddBookingFails()
        {
            var seat = new Seat { SeatID = 1, ZoneID = 5 };

            var seatRepo = new Mock<ISeatRepository>();
            var bookingRepo = new Mock<IBookingRepository>();
            var mockValidator = new Mock<ICustomValidator<Booking>>();
            seatRepo.Setup(r => r.GetSeatByIdAsync(1)).ReturnsAsync(seat);
            seatRepo.Setup(r => r.FindAvailableSeatAsync(5, It.IsAny<DateTime>(), 60))
                    .ReturnsAsync(new List<Seat> { seat });
            bookingRepo.Setup(r => r.AddBookingAsync(It.IsAny<Booking>())).ReturnsAsync(false);

            var service = new BookingService(bookingRepo.Object, seatRepo.Object, mockValidator.Object);
            var result = await service.BookSeatAsync(1, 42, DateTime.Now, 60);

            Assert.False(result);
        }

        [Fact]
        public async Task BookSeatAsync_ReturnsFalse_WhenSeatUpdateFails()
        {
            var seat = new Seat { SeatID = 1, ZoneID = 5 };

            var seatRepo = new Mock<ISeatRepository>();
            var bookingRepo = new Mock<IBookingRepository>();
            var mockValidator = new Mock<ICustomValidator<Booking>>();
            seatRepo.Setup(r => r.GetSeatByIdAsync(1)).ReturnsAsync(seat);
            seatRepo.Setup(r => r.FindAvailableSeatAsync(5, It.IsAny<DateTime>(), 60))
                    .ReturnsAsync(new List<Seat> { seat });
            bookingRepo.Setup(r => r.AddBookingAsync(It.IsAny<Booking>())).ReturnsAsync(true);
            seatRepo.Setup(r => r.UpdateSeatAvailabilityAsync(1, false)).ReturnsAsync(false);

            var service = new BookingService(bookingRepo.Object, seatRepo.Object, mockValidator.Object);
            var result = await service.BookSeatAsync(1, 42, DateTime.Now, 60);

            Assert.False(result);
        }

        [Fact]
        public async Task BookSeatAsync_CreatesBookingWithCorrectFields()
        {
            var seat = new Seat { SeatID = 1, ZoneID = 5 };
            Booking capturedBooking = null;

            var seatRepo = new Mock<ISeatRepository>();
            var bookingRepo = new Mock<IBookingRepository>();
            var mockValidator = new Mock<ICustomValidator<Booking>>();
            seatRepo.Setup(r => r.GetSeatByIdAsync(1)).ReturnsAsync(seat);
            seatRepo.Setup(r => r.FindAvailableSeatAsync(5, It.IsAny<DateTime>(), 60))
                    .ReturnsAsync(new List<Seat> { seat });
            bookingRepo.Setup(r => r.AddBookingAsync(It.IsAny<Booking>()))
                       .Callback<Booking>(b => capturedBooking = b)
                       .ReturnsAsync(true);
            seatRepo.Setup(r => r.UpdateSeatAvailabilityAsync(1, false)).ReturnsAsync(true);

            var service = new BookingService(bookingRepo.Object, seatRepo.Object, mockValidator.Object);
            var now = DateTime.Now;

            await service.BookSeatAsync(1, 99, now, 60);

            Assert.NotNull(capturedBooking);
            Assert.Equal(99, capturedBooking.UserId);
            Assert.Equal(1, capturedBooking.SeatId);
            Assert.Equal(now, capturedBooking.StartTime);
            Assert.Equal(60, capturedBooking.Duration);
            Assert.Equal(Status.Confirmed, capturedBooking.Status);
        }


        [Fact]
        public async Task BookSeatAsync_ShouldReturnFalse_WhenSeatDoesNotExist()
        {
           
            var mockSeatRepo = new Mock<ISeatRepository>();
            var mockBookingRepo = new Mock<IBookingRepository>();
            mockSeatRepo.Setup(r => r.GetSeatByIdAsync(It.IsAny<int>())).ReturnsAsync((Seat)null);
            var mockValidator = new Mock<ICustomValidator<Booking>>();
            var service = new BookingService(mockBookingRepo.Object, mockSeatRepo.Object, mockValidator.Object);
            var result = await service.BookSeatAsync(seatId: 1, userId: 10, startTime: DateTime.Now, durationMinutes: 60);
            Assert.False(result); 
        }


        [Fact]
        public async Task BookSeatAsync_ShouldReturnFalse_WhenSeatNotAvailable()
        {
            var seat = new Seat { SeatID = 1, ZoneID = 10 };
            var mockSeatRepo = new Mock<ISeatRepository>();
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockValidator = new Mock<ICustomValidator<Booking>>();
            mockSeatRepo.Setup(r => r.GetSeatByIdAsync(1)).ReturnsAsync(seat);
            mockSeatRepo.Setup(r => r.FindAvailableSeatAsync(10, It.IsAny<DateTime>(), It.IsAny<int>())).ReturnsAsync(new List<Seat>()); 
            var service = new BookingService(mockBookingRepo.Object, mockSeatRepo.Object, mockValidator.Object);
            var result = await service.BookSeatAsync(1, 1, DateTime.Now, 60);

            Assert.False(result);

        }









    }
}
