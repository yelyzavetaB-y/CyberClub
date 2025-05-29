using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Domain.Models.DTOs;
using CyberClub.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly ZoneService _zoneService;
        private readonly SeatService _seatService;
        private readonly ICustomValidator<Booking> _bookSeatValidator;

        public BookingService(IBookingRepository bookingRepository, ISeatRepository seatRepository, ICustomValidator<Booking> bookSeatValidator, ZoneService zoneService, SeatService seatService)
        {
            _bookingRepository = bookingRepository;
            _seatRepository = seatRepository;
            _bookSeatValidator = bookSeatValidator;
            _zoneService = zoneService;
            _seatService = seatService;
        }

        public async Task<List<ZoneSeatMap>> GetZoneSeatMapAsync(DateTime date, TimeSpan time, int duration)
        {
            var requestedTime = date + time;
            var zones = await _zoneService.GetAllZonesAsync();

            var result = new List<ZoneSeatMap>();

            foreach (var zone in zones)
            {
                var seats = await _seatService.GetSeatsWithAvailabilityAsync(zone.ZoneID, requestedTime, duration);

                result.Add(new ZoneSeatMap
                {
                    ZoneID = zone.ZoneID,
                    Name = zone.Name,
                    Capacity = zone.Capacity,
                    Seats = seats.Select(s => new SeatStatus
                    {
                        SeatID = s.SeatID,
                        SeatNumber = s.SeatNumber,
                        IsAvailable = s.IsAvailable
                    }).ToList()
                });
            }

            return result;
        }


        public async Task<bool> BookSeatAsync(int seatId, int userId, DateTime startTime, int durationMinutes)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            if (seat == null) return false;

            // Get all bookings for the seat's zone during the requested time
            var overlappingBookings = await _bookingRepository.GetBookingsForZoneAndTimeAsync(seat.ZoneID, startTime, durationMinutes);

            // If any booking conflicts with this seat, it's not available
            bool isSeatBooked = overlappingBookings.Any(b => b.SeatId == seatId);
            if (isSeatBooked) return false;

            var booking = new Booking
            {
                UserId = userId,
                SeatId = seatId,
                StartTime = startTime,
                Duration = durationMinutes,
                Status = Status.Confirmed
            };

            var result = _bookSeatValidator.Validate(booking);
            if (result != ValidationResult.Success)
                throw new Exception(result.ErrorMessage);

            return await _bookingRepository.AddBookingAsync(booking);
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

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllBookingsAsync();
        }

        public async Task<bool> UpdateBookingStatusAsync(Booking booking)
        {
            return await _bookingRepository.UpdateBookingStatusAsync(booking.BookingID, booking.Status);
        }


       

    }

}
