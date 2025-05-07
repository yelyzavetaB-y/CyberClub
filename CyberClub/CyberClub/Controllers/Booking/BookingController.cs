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
        private readonly BookingService _bookingService;

        public BookingController(ZoneService zoneService, SeatService seatService, BookingService bookingService)
        {
            _zoneService = zoneService;
            _seatService = seatService;
            _bookingService = bookingService;
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
        [HttpGet]
        public async Task<IActionResult> GetAllZonesWithSeats(string startTime, int duration)
        {
            var start = DateTime.Parse(startTime);
            var end = start.AddMinutes(duration);

            var zones = await _zoneService.GetAllZonesAsync(); 
            var allSeats = new List<ZoneWithSeatsViewModel>();

            foreach (var zone in zones)
            {
                var seats = await _seatService.GetSeatsByZoneIdAsync(zone.ZoneID);
                var bookedIds = await _seatService.GetAvailableSeatAsync(zone.ZoneID, start, duration); 

                var seatVMs = seats.Select(seat => new SeatWithStatusViewModel
                {
                    SeatID = seat.SeatID,
                    SeatNumber = seat.SeatNumber,
                    ZoneID = seat.ZoneID,
                    IsAvailable = seat.IsAvailable,
                    IsAvailableForBooking = !bookedIds.Any(b => b.SeatID == seat.SeatID)
                }).ToList();

                allSeats.Add(new ZoneWithSeatsViewModel
                {
                    ZoneID = zone.ZoneID,
                    Name = zone.Name,
                    Capacity = zone.Capacity,
                    Seats = seatVMs
                });
            }

            return Json(allSeats);
        }

        public async Task<IActionResult> MyBookings()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;

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

    }
}
