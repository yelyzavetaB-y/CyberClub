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

        public async Task<bool> BookAvailableSeatAsync(int userId, DateTime startTime, int zoneId, int durarion)
        {
            return await _bookingRepository.BookAvailableSeatAsync(userId, startTime, zoneId, durarion);
        }

        public async Task<bool> AddBookingAsync(Booking booking)
        {
            return await _bookingRepository.AddBookingAsync(booking);
        }
        public async Task<bool> BookSeatAsync(int seatId, int userId, DateTime startTime, int durationMinutes)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            var availableSeats = await _seatRepository.FindAvailableSeatAsync(seat.ZoneID, startTime, durationMinutes);
            bool stillAvailable = availableSeats.Any(s => s.SeatID == seatId);
            if (!stillAvailable)
                return false;

            var booking = new Booking
            {
                UserId = userId,
                SeatId = seatId,
                StartTime = startTime,
                Duration = durationMinutes,
                Status = Status.Confirmed
            };

            bool success = await _bookingRepository.AddBookingAsync(booking);
            if (!success) return false;

            bool updated = await _seatRepository.UpdateSeatAvailabilityAsync(seatId, false);
            Console.WriteLine("🛠 Seat availability update success: " + updated);
            return updated;
        }


        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _bookingRepository.GetBookingsByUserIdAsync(userId);
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            return await _bookingRepository.CancelBookingAsync(bookingId);
        }

    }

}
