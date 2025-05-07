using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;


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



    }
}
