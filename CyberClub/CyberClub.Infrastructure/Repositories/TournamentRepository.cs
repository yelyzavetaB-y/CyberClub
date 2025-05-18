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
            string query = "SELECT * FROM Tournament ORDER BY StartDateTime DESC";

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
                        IsOnline = reader.GetBoolean(reader.GetOrdinal("IsOnline")),
                        MaxParticipants = reader.IsDBNull(reader.GetOrdinal("MaxParticipants")) ? null : reader.GetInt32(reader.GetOrdinal("MaxParticipants"))
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
                        IsOnline = reader.GetBoolean(reader.GetOrdinal("IsOnline")),
                        MaxParticipants = reader.IsDBNull(reader.GetOrdinal("MaxParticipants")) ? null : reader.GetInt32(reader.GetOrdinal("MaxParticipants"))
                    };
                }
            }, parameters);

            return tournament;
        }

        public async Task<bool> AddAsync(Tournament tournament)
        {
            string query = @"
                INSERT INTO Tournament (Name, Game, StartDateTime, Status, Description, IsOnline, MaxParticipants)
                VALUES (@Name, @Game, @StartDateTime, @Status, @Description, @IsOnline, @MaxParticipants)";

            var parameters = new[]
            {
                new SqlParameter("@Name", tournament.Name),
                new SqlParameter("@Game", tournament.Game),
                new SqlParameter("@StartDateTime", tournament.StartDateTime),
                new SqlParameter("@Status", tournament.Status),
                new SqlParameter("@Description", (object?)tournament.Description ?? DBNull.Value),
                new SqlParameter("@IsOnline", tournament.IsOnline),
                new SqlParameter("@MaxParticipants", (object?)tournament.MaxParticipants ?? DBNull.Value)
            };

            int rowsAffected = await _queryBuilder.ExecuteQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAsync(Tournament tournament)
        {
            string query = @"
                UPDATE Tournament
                SET Name = @Name, Game = @Game, StartDateTime = @StartDateTime,
                    Status = @Status, Description = @Description, IsOnline = @IsOnline, MaxParticipants = @MaxParticipants
                WHERE Id = @Id";

            var parameters = new[]
            {
                new SqlParameter("@Id", tournament.Id),
                new SqlParameter("@Name", tournament.Name),
                new SqlParameter("@Game", tournament.Game),
                new SqlParameter("@StartDateTime", tournament.StartDateTime),
                new SqlParameter("@Status", tournament.Status),
                new SqlParameter("@Description", (object?)tournament.Description ?? DBNull.Value),
                new SqlParameter("@IsOnline", tournament.IsOnline),
                new SqlParameter("@MaxParticipants", (object?)tournament.MaxParticipants ?? DBNull.Value)
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
    }
}
