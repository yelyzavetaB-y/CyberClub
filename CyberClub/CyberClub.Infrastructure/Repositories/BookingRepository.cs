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
    new SqlParameter("@StartTime", booking.ReservedStartTime), 
    new SqlParameter("@Duration", booking.Duration)
        };

            decimal bookingIdRaw = await _queryBuilder.ExecuteScalarAsync<decimal>(query, parameters);
            int bookingId = (int)bookingIdRaw;
            return bookingId > 0;
        }

        public async Task<bool> BookAvailableSeatAsync(int userId, DateTime reservedStartTime)
        {
            Seat availableSeat = await _seatRepository.FindAvailableSeatAsync();
            if (availableSeat == null)
                return false; 
            Booking booking = new Booking
            {
                UserId = userId,
                SeatId = availableSeat.SeatID,
                Status = Status.Confirmed,
                ReservedStartTime = reservedStartTime
            };

            return await AddBookingAsync(booking);
        }


    }

}
