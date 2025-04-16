using CyberClub.ContextUtilities;
using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            model.Zones = await _zoneService.GetAllZonesAsync();
            model.UserID = HttpContext.Session.GetInt32("UserID") ?? 0;
            if (model.SelectedZoneId > 0 && model.SelectedDate != default && model.SelectedTime != default && model.Duration > 0)
            {
                model.Seats = await _seatService.GetAvailableSeatAsync(
                    model.SelectedZoneId,
                    model.SelectedDate.Date + model.SelectedTime,
                    model.Duration
                );
            }
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

            var success = await _bookingService.BookSeatAsync(
                model.SelectedSeatId,
                model.UserID,
                model.SelectedDate.Date + model.SelectedTime,
                model.Duration
            );

            if (!success)
            {
                ModelState.AddModelError("", "error.");
                model.Zones = await _zoneService.GetAllZonesAsync();
                model.Seats = await _seatService.GetSeatsByZoneIdAsync(model.SelectedZoneId);
                return View("CustomerPanel", model);
            }

            return RedirectToAction("Customer");
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

        [HttpGet]
        public async Task<IActionResult> Panel()                    /*  C-UC-2*/
        {
            var zones = await _zoneService.GetAllZonesAsync();
            int? userId = HttpContext.Session.GetInt32("UserID");
            Console.WriteLine("UserID in session: " + HttpContext.Session.GetInt32("UserID"));

            var viewModel = new BookingViewModel
            {
                UserID = userId ?? 0,
                Zones = zones ?? new List<Zone>(),
                SelectedDate = DateTime.Today,
                SelectedTime = new TimeSpan(12, 0, 0),
                Duration = 60
            };

            return View("CustomerPanel", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetSeatsByZone(int zoneId)
        {
            var seats = await _seatService.GetSeatsByZoneIdAsync(zoneId);
            return Json(seats.Select(s => new { s.SeatID, s.SeatNumber }));
        }

    }
}
