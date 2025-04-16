using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using CyberClub.Models;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CyberClub.Controllers.Booking
{
    public class BookingController : Controller
    {
        private readonly ZoneService _zoneService;
        private readonly SeatService _seatService;

        public BookingController(ZoneService zoneService, SeatService seatService)
        {
            _zoneService = zoneService;
            _seatService = seatService;
        }

        public async Task<IActionResult> Panel()
        {
            var zones = await _zoneService.GetAllZonesAsync(); 

            return View(zones); 
        }

        [HttpGet]
        public async Task<IActionResult> GetSeatMap(int zoneId, string startTime, int duration)
        {
            var start = DateTime.Parse(startTime);
            var seats = await _seatService.GetSeatsWithAvailability(zoneId, start, duration);
            return Json(seats);
        }


    }
}
