using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Domain.Models.Enum;
using CyberClub.Infrastructure.DBService;
using Microsoft.Data.SqlClient;


namespace CyberClub.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly QueryBuilder _queryBuilder;
        private readonly ISeatRepository _seatRepository;
        public BookingRepository(QueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public async Task<bool> AddBookingAsync(Booking booking)
        {
            string query = @"
    INSERT INTO Booking (UserID, SeatID, Status, StartTime, Duration)
    VALUES (@UserID, @SeatID, @Status, @StartTime, @Duration);
    SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
    new SqlParameter("@UserID", booking.UserId),
    new SqlParameter("@SeatID", booking.SeatId),
    new SqlParameter("@Status", booking.Status.ToString()),
    new SqlParameter("@StartTime", booking.StartTime), 
    new SqlParameter("@Duration", booking.Duration)
        };

            decimal bookingIdRaw = await _queryBuilder.ExecuteScalarAsync<decimal>(query, parameters);
            int bookingId = (int)bookingIdRaw;
            return bookingId > 0;
        }

        public async Task<bool> BookAvailableSeatAsync(int userId, DateTime startTime, int zoneId, int duration)
        {
            List<Seat> availableSeats = await _seatRepository.FindAvailableSeatAsync(zoneId, startTime, duration);
            Seat availableSeat = availableSeats.FirstOrDefault();
            if (availableSeat == null)
                return false; 
            Booking booking = new Booking
            {
                UserId = userId,
                SeatId = availableSeat.SeatID,
                Status = Status.Confirmed,
                StartTime = startTime
            };

            return await AddBookingAsync(booking);
        }

        public async Task<List<int>> GetBookedSeatIdsAsync(int zoneId, DateTime start, DateTime end)
        {
            string query = @"
SELECT b.SeatID
FROM Booking b
JOIN Seat s ON b.SeatID = s.SeatID
WHERE s.ZoneID = @ZoneId
  AND b.Status = 'Confirmed'
  AND (
    @StartTime < DATEADD(MINUTE, b.Duration, b.StartTime)
    AND @EndTime > b.StartTime
)";

            var parameters = new[]
            {
        new SqlParameter("@ZoneId", zoneId),
        new SqlParameter("@StartTime", start),
        new SqlParameter("@EndTime", end)
    };

            List<int> bookedSeats = new();
            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                while (reader.Read())
                    bookedSeats.Add(reader.GetInt32(0));
            }, parameters);

            return bookedSeats;
        }

    }

}
