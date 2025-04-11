using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Infrastructure.DBService;
using Microsoft.Data.SqlClient;

namespace CyberClub.Infrastructure.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly QueryBuilder _queryBuilder;

        public SeatRepository(QueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public async Task<bool> AddSeatAsync(Seat seat)
        {
            const string query = @"
            INSERT INTO Seat (SeatNumber, ZoneID, IsAvailable)
            VALUES (@SeatNumber, @ZoneID, @IsAvailable);
            SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
            new SqlParameter("@SeatNumber", seat.SeatNumber),
            new SqlParameter("@ZoneID", seat.ZoneID),
            new SqlParameter("@IsAvailable", seat.IsAvailable)
        };

            int seatId = await _queryBuilder.ExecuteScalarAsync<int>(query, parameters);
            return seatId > 0;
        }

        public async Task<Seat> FindAvailableSeatAsync()
        {
            const string query = @"SELECT TOP 1 SeatID, SeatNumber, ZoneID, IsAvailable, ReservedStartTime
                               FROM Seat WHERE IsAvailable = 1 ORDER BY ZoneID ASC;";

            Seat seat = null;

            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                if (reader.Read())
                {
                    seat = MapSeat(reader);
                }
            });

            return seat;
        }

        public async Task<bool> UpdateSeatAvailabilityAsync(int seatId, bool isAvailable)
        {
            const string query = "UPDATE Seat SET IsAvailable = @IsAvailable WHERE SeatID = @SeatID";

            var parameters = new[]
            {
            new SqlParameter("@IsAvailable", isAvailable),
            new SqlParameter("@SeatID", seatId)
        };

            int rowsAffected = await _queryBuilder.ExecuteQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<Seat> GetSeatByIdAsync(int seatId)
        {
            const string query = @"SELECT SeatID, SeatNumber, ZoneID, IsAvailable, ReservedStartTime
                               FROM Seat WHERE SeatID = @SeatID";

            var parameters = new[]
            {
            new SqlParameter("@SeatID", seatId)
        };

            Seat seat = null;

            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                if (reader.Read())
                {
                    seat = MapSeat(reader);
                }
            }, parameters);

            return seat;
        }

        public async Task<List<Seat>> GetSeatsByZoneIdAsync(int zoneId)
        {
            const string query = @"SELECT SeatID, SeatNumber, ZoneID, IsAvailable, ReservedStartTime
                               FROM Seat WHERE ZoneID = @ZoneID";

            var parameters = new[]
            {
            new SqlParameter("@ZoneID", zoneId)
        };

            var seats = new List<Seat>();

            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                while (reader.Read())
                {
                    seats.Add(MapSeat(reader));
                }
            }, parameters);

            return seats;
        }

        private Seat MapSeat(SqlDataReader reader)
        {
            return new Seat
            {
                SeatID = reader.GetInt32(reader.GetOrdinal("SeatID")),
                ZoneID = reader.GetInt32(reader.GetOrdinal("ZoneID")),
                SeatNumber = reader.GetString(reader.GetOrdinal("SeatNumber")),
                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                ReservedStartTime = reader.IsDBNull(reader.GetOrdinal("ReservedStartTime"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("ReservedStartTime"))
            };
        }
    }

}
