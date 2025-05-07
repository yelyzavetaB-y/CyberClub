using CyberClub.ContextUtilities;
using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace CyberClub.Controllers.Customer
{
    [SessionAuthorize]
    public class CustomerController : Controller
    {
        private readonly UserService _userService;
        private readonly ZoneService _zoneService;
        private readonly SeatService _seatService;
        private readonly BookingService _bookingService;

        public CustomerController(UserService userService, ZoneService zoneService, SeatService seatService, BookingService bookingService)
        {
            _userService = userService;
            _zoneService = zoneService;
            _seatService = seatService;
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBooking(BookingViewModel model)     /*C-UC-2*/
        {
            Console.WriteLine($"[UpdateBooking] Incoming SelectedTimeRaw = {model.SelectedTimeRaw}");

            if (!string.IsNullOrEmpty(model.SelectedTimeRaw) &&
                TimeSpan.TryParse(model.SelectedTimeRaw, out var parsedTime))
            {
                model.SelectedTime = parsedTime;
                Console.WriteLine($"[UpdateBooking] Parsed = {model.SelectedTime}");
            }
            else
            {
                Console.WriteLine("❌ Time parse failed. Fallback to 12:00");
                model.SelectedTime = new TimeSpan(12, 0, 0);
            }

            model.Zones = await _zoneService.GetAllZonesAsync();
            model.Seats = await _seatService.GetAvailableSeatAsync(
                model.SelectedZoneId,
                model.SelectedDate.Date + model.SelectedTime,
                model.Duration >= 60 ? model.Duration : 60
            );

            return View("CustomerPanel", model);
        }
        [HttpPost]
        public async Task<IActionResult> FinalizeBooking(BookingViewModel model)     /*C-UC-3*/
        {
            if (!ModelState.IsValid || model.SelectedSeatId == 0 || model.UserID == 0)
            {

                ModelState.AddModelError("", "Fill data.");
                model.Zones = await _zoneService.GetAllZonesAsync();
                model.Seats = await _seatService.GetSeatsByZoneIdAsync(model.SelectedZoneId);
                Console.WriteLine("Seat: " + model.SelectedSeatId);
                Console.WriteLine("User: " + model.UserID);
                return View("CustomerPanel", model);
            }
            var startDateTime = model.SelectedDate + TimeSpan.Parse(model.SelectedTimeRaw);

            var success = await _bookingService.BookSeatAsync(
                model.SelectedSeatId,
                model.UserID,
                startDateTime,
                model.Duration
            );

            if (!success)
            {
                ModelState.AddModelError("", "error.");
                model.Zones = await _zoneService.GetAllZonesAsync();
                model.Seats = await _seatService.GetSeatsByZoneIdAsync(model.SelectedZoneId);
                return View(model);
            }

            return RedirectToAction("Panel");
        }
        public IActionResult Confirmation()
        {
            return View();
        }
        public IActionResult Settings()
        {
            var dobStr = HttpContext.Session.GetString("DOB");
            DateTime dob;
            if (!string.IsNullOrEmpty(dobStr) && DateTime.TryParse(dobStr, out dob))
            {

            }
            else
            {
                dob = DateTime.Now;
            }

            var model = new SettingsViewModel
            {
                FullName = HttpContext.Session.GetString("FullName"),
                Email = HttpContext.Session.GetString("Email"),
                PhoneNumber = HttpContext.Session.GetString("PhoneNumber"),
                DOB = dob
            };

            return View(model);
        }
        /* C-UC-2 */
        [HttpGet]
        public async Task<IActionResult> Panel()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            var now = DateTime.Now;
            int roundedMinutes = ((now.Minute + 14) / 15) * 15;
            int hour = now.Hour + (roundedMinutes == 60 ? 1 : 0);
            int minutes = roundedMinutes == 60 ? 0 : roundedMinutes;

            if (hour >= 24)
            {
                hour = 23;
                minutes = 45;
            }

            var rounded = new TimeSpan(hour, minutes, 0);

            //await _seatService.ReleaseSeatsWithEndedBookingsAsync();

            var viewModel = new BookingViewModel
            {
                UserID = userId ?? 0,
                Zones = await _zoneService.GetAllZonesAsync(),
                SelectedDate = now.Date,
                SelectedTime = rounded,
                Duration = 60
            };

            viewModel.SelectedTimeRaw = viewModel.SelectedTime.ToString(@"hh\:mm");

            return View("CustomerPanel", viewModel);

        }
        [HttpGet]
        public async Task<IActionResult> GetSeatsByZone(int zoneId, string startTime, int duration)
        {
            if (!DateTime.TryParse(startTime, out var parsedTime))
            {
                Console.WriteLine($"❌ Failed to parse: {startTime}");
                return BadRequest("Invalid startTime");
            }

            var seats = await _seatService.GetAvailableSeatAsync(zoneId, parsedTime, duration);

            return Json(seats.Select(s => new {
                seatID = s.SeatID,
                seatNumber = s.SeatNumber,
                isAvailable = true
            }));
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var success = await _bookingService.CancelBookingAsync(id);
            if (success)
                return RedirectToAction("MyBookingsList");

            ModelState.AddModelError("", "Failed to cancel booking");
            return RedirectToAction("MyBookingsList");
        }

        [HttpGet]
        public async Task<IActionResult> MyBookingsList()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null || userId == 0)
            {
                return RedirectToAction("Panel");
            }

            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId.Value);

            var model = new MyBookingsViewModel
            {
                Bookings = bookings.Select(b => new UserBookingInfo
                {
                    Id = b.BookingID,
                    StartTime = b.StartTime,
                    Duration = b.Duration,
                    SeatNumber = b.Seat?.SeatNumber ?? "N/A",
                    ZoneName = b.Zone?.Name ?? "N/A",
                    Status = b.Status.ToString()
                }).ToList()
            };

            return View(model);
        }


    }
}
