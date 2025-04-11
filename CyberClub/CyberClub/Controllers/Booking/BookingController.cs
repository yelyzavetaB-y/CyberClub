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
        public BookingController(ZoneService zoneService)
        {
            _zoneService = zoneService;
        }


        public async Task<IActionResult> Panel()
        {
            var zones = await _zoneService.GetAllZonesAsync(); 

            return View(zones); 
        }



    }
}
