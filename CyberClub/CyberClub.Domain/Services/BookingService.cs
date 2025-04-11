using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISeatRepository _seatRepository;

        public BookingService(IBookingRepository bookingRepository, ISeatRepository seatRepository)
        {
            _bookingRepository = bookingRepository;
            _seatRepository = seatRepository;
        }

        public async Task<bool> BookAvailableSeatAsync(int userId, DateTime reservedStartTime)
        {
            return await _bookingRepository.BookAvailableSeatAsync(userId, reservedStartTime);
        }

        public async Task<bool> AddBookingAsync(Booking booking)
        {
            return await _bookingRepository.AddBookingAsync(booking);
        }
        public async Task<bool> BookSeatAsync(int seatId, int userId, DateTime reservedStart, int durationMinutes)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            if (seat == null || !seat.IsAvailable)
                return false;

            var booking = new Booking
            {
                UserId = userId,
                SeatId = seatId,
                ReservedStartTime = reservedStart,
                Duration = durationMinutes,
                Status = Status.Confirmed
            };

            bool success = await _bookingRepository.AddBookingAsync(booking);
            if (!success) return false;

            return await _seatRepository.UpdateSeatAvailabilityAsync(seatId, false);
        }




    }

}
