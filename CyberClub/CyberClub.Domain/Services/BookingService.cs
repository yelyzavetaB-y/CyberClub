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

        public async Task<bool> BookSeatAsync(int seatId, int userId, DateTime startTime, int durationMinutes)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            if(seat == null)
            {
                return false;
            }
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
           
            return updated;
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId, bool includeCancelled = false)
        {
            return await _bookingRepository.GetBookingsByUserIdAsync(userId, includeCancelled);
        }


        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            return await _bookingRepository.CancelBookingAsync(bookingId);
        }
        public async Task<int> DeleteExpiredBookingsAsync()
        {
            return await _bookingRepository.DeleteExpiredBookingsAsync();
        }

    }

}
