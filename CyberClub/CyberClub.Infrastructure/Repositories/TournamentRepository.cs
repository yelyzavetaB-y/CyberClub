using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Infrastructure.DBService;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Infrastructure.Repositories
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly QueryBuilder _queryBuilder;

        public TournamentRepository(QueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public async Task<List<Tournament>> GetAllAsync()
        {
            var tournaments = new List<Tournament>();
            string query = "SELECT  t.*,  (SELECT COUNT(*) FROM TournamentRegistration r WHERE r.TournamentID = t.Id) AS CurrentParticipantCount FROM Tournament t ORDER BY t.StartDateTime DESC;";

            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                while (reader.Read())
                {
                    tournaments.Add(new Tournament
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Game = reader.GetString(reader.GetOrdinal("Game")),
                        StartDateTime = reader.GetDateTime(reader.GetOrdinal("StartDateTime")),
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        MaxParticipants = reader.IsDBNull(reader.GetOrdinal("MaxParticipants")) ? null : reader.GetInt32(reader.GetOrdinal("MaxParticipants")),
                        ThemeColor = reader.IsDBNull(reader.GetOrdinal("ThemeColor")) ? null : reader.GetString(reader.GetOrdinal("ThemeColor")),
                        CurrentParticipantCount = reader.GetInt32(reader.GetOrdinal("CurrentParticipantCount"))


                    });
                }
            });

            return tournaments;
        }

        public async Task<Tournament?> GetByIdAsync(int id)
        {
            Tournament? tournament = null;
            string query = "SELECT * FROM Tournament WHERE Id = @Id";
            var parameters = new[] { new SqlParameter("@Id", id) };

            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                if (reader.Read())
                {
                    tournament = new Tournament
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Game = reader.GetString(reader.GetOrdinal("Game")),
                        StartDateTime = reader.GetDateTime(reader.GetOrdinal("StartDateTime")),
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        MaxParticipants = reader.IsDBNull(reader.GetOrdinal("MaxParticipants")) ? null : reader.GetInt32(reader.GetOrdinal("MaxParticipants")),
                        ThemeColor = reader.IsDBNull(reader.GetOrdinal("ThemeColor")) ? null : reader.GetString(reader.GetOrdinal("ThemeColor"))

                    };
                }
            }, parameters);

            return tournament;
        }

        public async Task<bool> AddTournamentAsync(Tournament tournament)
        {
            string query = @"
                INSERT INTO Tournament (Name, Game, StartDateTime, Status, Description, MaxParticipants, ThemeColor)
VALUES (@Name, @Game, @StartDateTime, @Status, @Description, @MaxParticipants, @ThemeColor)
";

            var parameters = new[]
            {
                new SqlParameter("@Name", tournament.Name),
                new SqlParameter("@Game", tournament.Game),
                new SqlParameter("@StartDateTime", tournament.StartDateTime),
                new SqlParameter("@Status", tournament.Status),
                new SqlParameter("@Description", (object?)tournament.Description ?? DBNull.Value),
                new SqlParameter("@MaxParticipants", (object?)tournament.MaxParticipants ?? DBNull.Value),
                new SqlParameter("@ThemeColor", (object?)tournament.ThemeColor ?? DBNull.Value)

            };

            int rowsAffected = await _queryBuilder.ExecuteQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAsync(Tournament tournament)
        {
            string query = @"
                UPDATE Tournament
SET Name = @Name, Game = @Game, StartDateTime = @StartDateTime,
    Status = @Status, Description = @Description, MaxParticipants = @MaxParticipants, ThemeColor = @ThemeColor
WHERE Id = @Id
";

            var parameters = new[]
            {
                new SqlParameter("@Id", tournament.Id),
                new SqlParameter("@Name", tournament.Name),
                new SqlParameter("@Game", tournament.Game),
                new SqlParameter("@StartDateTime", tournament.StartDateTime),
                new SqlParameter("@Status", tournament.Status),
                new SqlParameter("@Description", (object?)tournament.Description ?? DBNull.Value),
                new SqlParameter("@MaxParticipants", (object?)tournament.MaxParticipants ?? DBNull.Value),
                new SqlParameter("@ThemeColor", (object?)tournament.ThemeColor ?? DBNull.Value)

            };

            int rowsAffected = await _queryBuilder.ExecuteQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            string query = "DELETE FROM Tournament WHERE Id = @Id";
            var parameters = new[] { new SqlParameter("@Id", id) };

            int rowsAffected = await _queryBuilder.ExecuteQueryAsync(query, parameters);
            return rowsAffected > 0;
        }



        public async Task<bool> RegisterUserAsync(int tournamentId, int userId)
        {
            const string checkQuery = "SELECT COUNT(*) FROM TournamentRegistration WHERE TournamentID = @TournamentID AND UserID = @UserID";
            var checkParams = new[]
            {
        new SqlParameter("@TournamentID", tournamentId),
        new SqlParameter("@UserID", userId)
    };

            int alreadyRegistered = await _queryBuilder.ExecuteScalarAsync<int>(checkQuery, checkParams);
            if (alreadyRegistered > 0)
                return false;

            const string insertQuery = @"
        INSERT INTO TournamentRegistration (TournamentID, UserID)
        VALUES (@TournamentID, @UserID)";

            // ✅ Use a new SqlParameter array for insert
            var insertParams = new[]
            {
        new SqlParameter("@TournamentID", tournamentId),
        new SqlParameter("@UserID", userId)
    };

            int rowsAffected = await _queryBuilder.ExecuteQueryAsync(insertQuery, insertParams);
            return rowsAffected > 0;
        }

        public async Task<List<Tournament>> GetTournamentsByUserIdAsync(int userId)
        {
            const string query = @"
        SELECT t.*
        FROM Tournament t
        JOIN TournamentRegistration r ON t.Id = r.TournamentID
        WHERE r.UserID = @UserID
        ORDER BY t.StartDateTime DESC";

            var parameters = new[] { new SqlParameter("@UserID", userId) };

            var tournaments = new List<Tournament>();
            await _queryBuilder.ExecuteQueryAsync(query, reader =>
            {
                while (reader.Read())
                {
                    tournaments.Add(new Tournament
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Game = reader.GetString(reader.GetOrdinal("Game")),
                        StartDateTime = reader.GetDateTime(reader.GetOrdinal("StartDateTime")),
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        MaxParticipants = reader.IsDBNull(reader.GetOrdinal("MaxParticipants")) ? null : reader.GetInt32(reader.GetOrdinal("MaxParticipants")),
                        ThemeColor = reader.IsDBNull(reader.GetOrdinal("ThemeColor")) ? null : reader.GetString(reader.GetOrdinal("ThemeColor")),
                      
                    });
                }
            }, parameters);

            return tournaments;
        }

        public async Task<bool> CancelUserBookingAsync(int tournamentId, int userId)
        {
            const string query = "DELETE FROM TournamentRegistration WHERE TournamentID = @TournamentID AND UserID = @UserID";

            var parameters = new[]
            {
        new SqlParameter("@TournamentID", tournamentId),
        new SqlParameter("@UserID", userId)
    };

            int rowsAffected = await _queryBuilder.ExecuteQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<int> GetRegisteredCountAsync(int tournamentId)
        {
            const string query = "SELECT COUNT(*) FROM TournamentRegistration WHERE TournamentID = @TournamentID";
            var parameters = new[] {
        new SqlParameter("@TournamentID", tournamentId)
    };

            return await _queryBuilder.ExecuteScalarAsync<int>(query, parameters);
        }

    }
}
