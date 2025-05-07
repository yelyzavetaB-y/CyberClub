using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace CyberClub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GeneralController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("updateExpiredBookingsSeats/{key}")]
        public async Task<IActionResult> UpdateExpiredBookingsSeats(string key)
        {
            //https://localhost:44358/api/general/updateExpiredBookingsSeats/d123d2378dg287dg23d78g
            if (key.Equals("d123d2378dg287dg23d78g"))
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UpdateExpiredBookingsSeats", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}

/*
Stored procedures:

CREATE PROCEDURE [UpdateExpiredBookingsSeats]
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Seat
    SET IsAvailable = 1
    WHERE SeatID IN (
        SELECT DISTINCT b.SeatID 
        FROM Booking b 
        WHERE b.Status = 'Confirmed' 
        AND DATEADD(MINUTE, b.Duration, b.StartTime) < GETDATE()
    );
END

 */