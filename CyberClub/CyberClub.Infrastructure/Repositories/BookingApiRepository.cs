using CyberClub.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Infrastructure.Repositories
{
    public class BookingApiRepository : IBookingApiRepository
    {
        private readonly IConfiguration _configuration;
        public BookingApiRepository(IConfiguration configuration)
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
