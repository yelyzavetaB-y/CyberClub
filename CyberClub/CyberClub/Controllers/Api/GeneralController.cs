using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberClub.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public GeneralController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        //https://localhost:44358/api/general/updateExpiredBookingsSeats/d123d2378dg287dg23d78g

        [HttpGet("updateExpiredBookingsSeats/{key}")]
        public async Task<IActionResult> UpdateExpiredBookingsSeats(string key)
        {
            if (key.Equals("d123d2378dg287dg23d78g", StringComparison.OrdinalIgnoreCase))
            {
                await _bookingService.UpdateExpiredBookingsSeatsAsync();
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
