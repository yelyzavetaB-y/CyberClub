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

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> MyBookings()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;

            await _bookingService.DeleteExpiredBookingsAsync();
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);

            var model = new MyBookingsViewModel
            {
                Bookings = bookings.Select(b => new UserBookingInfo
                {
                    Id = b.BookingID,
                    StartTime = b.StartTime,
                    ZoneName = b.Zone?.Name ?? "N/A",
                    SeatNumber = b.Seat?.SeatNumber ?? "N/A",
                    Duration = b.Duration,
                    Status = b.Status.ToString()
                }).ToList()
            };

            return View("~/Views/Customer/MyBookingsList.cshtml", model);

        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var success = await _bookingService.CancelBookingAsync(id);
            if (success)
                return RedirectToAction("MyBookings");

            ModelState.AddModelError("", "Failed to cancel booking");
            return RedirectToAction("MyBookings");
        }
    }
}
