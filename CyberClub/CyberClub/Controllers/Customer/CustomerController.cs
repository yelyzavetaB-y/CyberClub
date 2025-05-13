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

        [HttpPost]    /*С-UC-3: Book a Session*/
        public async Task<IActionResult> FinalizeBooking(BookingViewModel model)
        {
            if (!string.IsNullOrEmpty(model.SelectedTimeRaw) &&
    TimeSpan.TryParse(model.SelectedTimeRaw, out var parsedTime))
            {
                model.SelectedTime = parsedTime;
            }
            else
            {
                Console.WriteLine("❌ Failed to parse SelectedTimeRaw — fallback to 12:00");
                model.SelectedTime = new TimeSpan(12, 0, 0);
            }


            if (!ModelState.IsValid || model.SelectedSeatId == 0 || model.UserID == 0)
            {
                ModelState.AddModelError("", "Please fill all required fields.");

                var zones = await _zoneService.GetAllZonesAsync();
                var colorPalette = new[] { "#28a745", "#007bff", "#ffc107", "#6610f2", "#17a2b8", "#e83e8c" };

                model.ZonesWithSeats = new List<ZoneWithSeatsViewModel>();

                foreach (var zone in zones)
                {
                    var seats = await _seatService.GetSeatsWithAvailabilityAsync(zone.ZoneID,
                        model.SelectedDate + model.SelectedTime,
                        model.Duration);

                    model.ZonesWithSeats.Add(new ZoneWithSeatsViewModel
                    {
                        ZoneID = zone.ZoneID,
                        Name = zone.Name,
                        Color = colorPalette[zones.IndexOf(zone) % colorPalette.Length],
                        Seats = seats.Select(s => new SeatWithStatusViewModel
                        {
                            SeatID = s.SeatID,
                            SeatNumber = s.SeatNumber,
                            IsAvailable = s.IsAvailable
                        }).ToList()
                    });
                }

                return View("CustomerPanel", model);
            }

            var startDateTime = model.SelectedDate + model.SelectedTime;

            var success = await _bookingService.BookSeatAsync(
                model.SelectedSeatId,
                model.UserID,
                startDateTime,
                model.Duration
            );
            Console.WriteLine($"📅 Date: {model.SelectedDate}, 🕒 Time: {model.SelectedTime}, ⏰ Raw: {model.SelectedTimeRaw} and {startDateTime}");

            if (!success)
            {
                ModelState.AddModelError("", "The seat is already booked for the selected time period.");

                var zones = await _zoneService.GetAllZonesAsync();
                var colorPalette = new[] { "#28a745", "#007bff", "#ffc107", "#6610f2", "#17a2b8", "#e83e8c" };

                model.ZonesWithSeats = new List<ZoneWithSeatsViewModel>();

                foreach (var zone in zones)
                {
                    var seats = await _seatService.GetSeatsWithAvailabilityAsync(zone.ZoneID,
                        startDateTime,
                        model.Duration);

                    model.ZonesWithSeats.Add(new ZoneWithSeatsViewModel
                    {
                        ZoneID = zone.ZoneID,
                        Name = zone.Name,
                        Color = colorPalette[zones.IndexOf(zone) % colorPalette.Length],
                        Seats = seats.Select(s => new SeatWithStatusViewModel
                        {
                            SeatID = s.SeatID,
                            SeatNumber = s.SeatNumber,
                            IsAvailable = s.IsAvailable
                        }).ToList()
                    });
                }

                return View("CustomerPanel", model);
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

        [HttpGet]   /* C-UC-2 */
        public async Task<IActionResult> Panel()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            var zones = await _zoneService.GetAllZonesAsync();
            Console.WriteLine($"Found {zones?.Count} zones");

            if (zones == null || !zones.Any())
            {
                Console.WriteLine("No zones found in database"); 
                                                                 
                zones = new List<Zone>
        {
            new Zone { ZoneID = 1, Name = "VIP Area", Capacity = 10 },
            new Zone { ZoneID = 2, Name = "Gaming Zone", Capacity = 20 }
        };
            }

            var colorPalette = new[] { "#28a745", "#007bff", "#ffc107", "#6610f2", "#17a2b8", "#e83e8c" };

            var zonesWithSeats = new List<ZoneWithSeatsViewModel>();

            foreach (var zone in zones)
            {
                var seats = await _seatService.GetSeatsWithAvailabilityAsync(zone.ZoneID,
                    DateTime.Now.Date + new TimeSpan(DateTime.Now.Hour, 0, 0),
                    60); 

                zonesWithSeats.Add(new ZoneWithSeatsViewModel
                {
                    ZoneID = zone.ZoneID,
                    Name = zone.Name,
                    Capacity = zone.Capacity,
                    Color = colorPalette[zones.IndexOf(zone) % colorPalette.Length],
                    Seats = seats?.Select(s => new SeatWithStatusViewModel
                    {
                        SeatID = s.SeatID,
                        SeatNumber = s.SeatNumber,
                        IsAvailable = s.IsAvailable
                    }).ToList() ?? new List<SeatWithStatusViewModel>()
                });
            }

            var viewModel = new BookingViewModel
            {
                UserID = userId ?? 0,
                SelectedDate = DateTime.Now.Date,
                SelectedTime = new TimeSpan(DateTime.Now.Hour, 0, 0),
                SelectedTimeRaw = new TimeSpan(DateTime.Now.Hour, 0, 0).ToString(@"hh\:mm"),
                Duration = 60,
                ZonesWithSeats = zonesWithSeats
            };

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

       


    }
}
