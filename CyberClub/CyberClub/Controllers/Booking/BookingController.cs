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
        private readonly BookingService _bookingService;
        private readonly TournamentService _tournamentService;
        public BookingController(BookingService bookingService, TournamentService tournamentService)
        {
            _bookingService = bookingService;
            _tournamentService = tournamentService;
        }
        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var success = await _bookingService.CancelBookingAsync(id);
            if (success)
                return RedirectToAction("MyBookings", "Customer");


            ModelState.AddModelError("", "Failed to cancel booking");
            return RedirectToAction("MyBookings", "Customer");

        }


    }
}
