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
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            if (booking.UserId <= 0)
                throw new ArgumentException("Invalid UserId");

            if (booking.SeatId <= 0)
                throw new ArgumentException("Invalid SeatId");

            if (booking.StartTime < DateTime.Now)
                throw new ArgumentException("Start time cannot be in the past");

            if (booking.Duration <= 0)
                throw new ArgumentException("Duration must be a positive value");

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

        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId, bool includeCancelled = false)
        {
            List<Booking> bookings = new();

            string query = @"
        SELECT b.BookingID, b.SeatID, b.UserID, b.StartTime, b.Duration, b.Status,
               s.SeatNumber, z.Name as ZoneName
        FROM Booking b
        JOIN Seat s ON s.SeatID = b.SeatID
        JOIN Zone z ON z.ZoneID = s.ZoneID
        WHERE b.UserID = @UserID";
                
            if (!includeCancelled)
            {
                query += " AND b.Status != 'Cancelled'";
            }

            query += " ORDER BY b.StartTime DESC";

            SqlParameter[] parameters = {
        new SqlParameter("@UserID", userId)
    };

            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                while (reader.Read())
                {
                    var seat = new Seat
                    {
                        SeatID = reader.GetInt32(reader.GetOrdinal("SeatID")),
                        SeatNumber = reader.GetString(reader.GetOrdinal("SeatNumber"))
                    };

                    var zone = new Zone
                    {
                        Name = reader.GetString(reader.GetOrdinal("ZoneName"))
                    };

                    bookings.Add(new Booking
                    {
                        BookingID = reader.GetInt32(reader.GetOrdinal("BookingID")),
                        SeatId = seat.SeatID,
                        UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                        StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                        Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                        Status = Enum.Parse<Status>(reader.GetString(reader.GetOrdinal("Status"))),
                        Seat = seat,
                        Zone = zone
                    });
                }
            }, parameters);

            return bookings;
        }


        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID");

            try
            {
                const string getSeatQuery = @"SELECT SeatID FROM Booking WHERE BookingID = @BookingID";
                var getSeatParam = new SqlParameter("@BookingID", bookingId);

                object result = await _queryBuilder.ExecuteScalarAsync<object>(getSeatQuery, getSeatParam);

                if (result == null || result == DBNull.Value)
                {
                    Console.WriteLine($"No booking found with ID: {bookingId}");
                    return false;
                }

                int seatId = Convert.ToInt32(result);

                const string cancelBookingQuery = @"UPDATE Booking SET Status = @Status WHERE BookingID = @BookingID";

                SqlParameter[] cancelParams = {
            new SqlParameter("@Status", Status.Cancelled.ToString()),
            new SqlParameter("@BookingID", bookingId)
        };

                int bookingRowsAffected = await _queryBuilder.ExecuteQueryAsync(cancelBookingQuery, cancelParams);
                if (bookingRowsAffected == 0)
                {
                    Console.WriteLine($"Booking status update failed for BookingID: {bookingId}");
                    return false;
                }

                const string updateSeatQuery = @"UPDATE Seat SET IsAvailable = 1 WHERE SeatID = @SeatID";
                SqlParameter[] seatParams = {
            new SqlParameter("@SeatID", seatId)
        };

                int seatRowsAffected = await _queryBuilder.ExecuteQueryAsync(updateSeatQuery, seatParams);

                return seatRowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while cancelling booking ID {bookingId}: {ex.Message}");
                return false;
            }
        }


        public async Task<List<Booking>> GetBookingsForZoneAndTimeAsync(int zoneId, DateTime startTime, int durationMinutes)
        {
            var endTime = startTime.AddMinutes(durationMinutes);

            const string query = @"
        SELECT b.*
        FROM Booking b
        JOIN Seat s ON s.SeatID = b.SeatID
        WHERE s.ZoneID = @ZoneID
          AND b.Status = 'Confirmed'
          AND NOT (
              DATEADD(MINUTE, b.Duration, b.StartTime) <= @StartTime
              OR b.StartTime >= @EndTime
          )";

            var parameters = new[]
            {
        new SqlParameter("@ZoneID", zoneId),
        new SqlParameter("@StartTime", startTime),
        new SqlParameter("@EndTime", endTime)
    };

            var bookings = new List<Booking>();

            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                while (reader.Read())
                {
                    bookings.Add(new Booking
                    {
                        BookingID = reader.GetInt32(reader.GetOrdinal("BookingID")),
                        SeatId = reader.GetInt32(reader.GetOrdinal("SeatID")),
                        StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                        Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                        Status = Enum.Parse<Status>(reader.GetString(reader.GetOrdinal("Status")))
                    });
                }
            }, parameters);

            return bookings;
        }

        public async Task<int> DeleteExpiredBookingsAsync()
        {
            string query = @"
        DELETE FROM Booking
        WHERE Status = 'Confirmed'
        AND DATEADD(MINUTE, Duration, StartTime) < @Now";

            var parameters = new[] { new SqlParameter("@Now", DateTime.Now) };
            return await _queryBuilder.ExecuteQueryAsync(query, parameters);
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            List<Booking> bookings = new List<Booking>();

            string query = @"
        SELECT b.BookingID, b.UserID, u.Email, b.SeatID, s.SeatNumber, s.ZoneID, z.Name AS ZoneName,
               b.StartTime, b.Duration, b.Status
        FROM Booking b
        JOIN [User] u ON b.UserID = u.UserID
        JOIN Seat s ON b.SeatID = s.SeatID
        JOIN Zone z ON s.ZoneID = z.ZoneID
        ORDER BY b.StartTime DESC";

            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                while (reader.Read())
                {
                    bookings.Add(new Booking
                    {
                        BookingID = reader.GetInt32(reader.GetOrdinal("BookingID")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                        StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                        Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                        Status = Enum.Parse<Status>(reader.GetString(reader.GetOrdinal("Status"))),
                        Seat = new Seat
                        {
                            SeatID = reader.GetInt32(reader.GetOrdinal("SeatID")),
                            SeatNumber = reader.GetString(reader.GetOrdinal("SeatNumber")),
                            ZoneID = reader.GetInt32(reader.GetOrdinal("ZoneID")),
                        },
                        Zone = new Zone
                        {
                            ZoneID = reader.GetInt32(reader.GetOrdinal("ZoneID")),
                            Name = reader.GetString(reader.GetOrdinal("ZoneName"))
                        },
                        User = new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("UserID")),
                            Email = reader.GetString(reader.GetOrdinal("Email"))
                        }
                    });
                }
            });

            return bookings;
        }

    }

}
