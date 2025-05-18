using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Infrastructure.DBService;
using CyberClub.Infrastructure.Interfaces;
using Microsoft.Data.SqlClient;
using System.Threading.Channels;


namespace CyberClub.Infrastructure.Repositories
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly QueryBuilder _queryBuilder;
        private readonly ISeatRepository _seatRepository;

        public ZoneRepository(QueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public async Task<bool> AddZoneAsync(Zone zone)
        {
            string query = @"
        INSERT INTO Zone (Name, Capacity)
        VALUES (@Name, @Capacity);
        SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
            new SqlParameter("@Name", zone.Name),
            new SqlParameter("@Capacity", zone.Capacity)
        };

            int zoneId = await _queryBuilder.ExecuteScalarAsync<int>(query, parameters);
            return zoneId > 0;
        }
        public async Task<List<Zone>> GetAllZonesAsync()
        {
            List<Zone> zones = new List<Zone>();
            string query = "SELECT ZoneID, Name, Capacity FROM Zone";

            Action<SqlDataReader> action = reader =>
            {
                while (reader.Read())
                {
                    zones.Add(new Zone
                    {
                        ZoneID = reader.IsDBNull(reader.GetOrdinal("ZoneID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ZoneID")),
                        Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                        Capacity = reader.IsDBNull(reader.GetOrdinal("Capacity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Capacity")),
                        //Seats = new List<Seat>()
                    });

                }
            };

            try
            {
                await _queryBuilder.ExecuteQueryAsync(query, action);
            }
            catch
            {
               
                return new List<Zone>(); 
            }

            return zones;
        }





    }

}
