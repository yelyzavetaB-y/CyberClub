using CyberClub.Api.Interfaces;
using Microsoft.Data.SqlClient;

namespace CyberClub.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly IConfiguration _configuration;

        public BookingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task UpdateExpiredBookingsSeatsAsync()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            using var command = new SqlCommand("UpdateExpiredBookingsSeats", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            await command.ExecuteNonQueryAsync();
        }
    }
}
