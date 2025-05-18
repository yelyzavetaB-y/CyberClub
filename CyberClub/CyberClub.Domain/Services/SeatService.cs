using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Domain.Models.Enum;


namespace CyberClub.Domain.Services
{
    public class SeatService
    {
        private readonly ISeatRepository seatRepository;
        private readonly IBookingRepository bookingRepository;
        private readonly IZoneRepository zoneRepository;
        public SeatService(ISeatRepository seatRepository, IBookingRepository bookingRepository, IZoneRepository zoneRepository)
        {
            this.seatRepository = seatRepository;
            this.bookingRepository = bookingRepository;
            this.zoneRepository = zoneRepository;
        }
        public async Task<List<Seat>> GetSeatsByZoneIdAsync(int zoneId)
        {
            return await seatRepository.GetSeatsByZoneIdAsync(zoneId);
        }
        public async Task<List<Seat>> GetAvailableSeatAsync(int zoneId, DateTime startTime, int duration)
        {
            return await seatRepository.FindAvailableSeatAsync(zoneId, startTime, duration);
        }

        public async Task<List<SeatWithStatus>> GetSeatsWithAvailability(int zoneId, DateTime start, int duration)
        {
            var allSeats = await seatRepository.GetSeatsByZoneIdAsync(zoneId);
            var end = start.AddMinutes(duration);
            var bookedSeatIds = await bookingRepository.GetBookedSeatIdsAsync(zoneId, start, end);

            return allSeats.Select(seat => new SeatWithStatus
            {
                SeatID = seat.SeatID,
                SeatNumber = seat.SeatNumber,
                ZoneID = seat.ZoneID,
                IsAvailable = seat.IsAvailable,
                IsAvailableForBooking = !bookedSeatIds.Contains(seat.SeatID)
            }).ToList();
        }
        //public async Task ReleaseSeatsWithEndedBookingsAsync()
        //{
        //    await seatRepository.ReleaseSeatsWithEndedBookingsAsync();
        //}

        public async Task<List<Seat>> GetSeatsWithAvailabilityAsync(int zoneId, DateTime startTime, int durationMinutes)
        {
            
            var allSeats = await seatRepository.GetSeatsByZoneIdAsync(zoneId);
            var activeBookings = await bookingRepository.GetBookingsForZoneAndTimeAsync(zoneId, startTime, durationMinutes);

            var occupiedSeatIds = activeBookings
                .Where(b => b.Status == Status.Confirmed)
                .Select(b => b.SeatId)
                .ToHashSet();

            foreach (var seat in allSeats)
            {
                seat.IsAvailable = !occupiedSeatIds.Contains(seat.SeatID);
            }

            return allSeats;
        }


    }
}
