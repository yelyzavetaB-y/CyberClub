using CyberClub.Domain.Models;
using CyberClub.Domain.Models.Enum;
using CyberClub.Domain.Services;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CyberClub.Controllers.Manager
{
    public class ManagerController : Controller
    {
        private readonly UserService _userService;
        private readonly BookingService _bookingService;
        private readonly TournamentService _tournamentService;
        public ManagerController(UserService userService, BookingService bookingService, TournamentService tournamentService)
        {
            _userService = userService;
            _bookingService = bookingService;
            _tournamentService = tournamentService;
        }
        public async Task<IActionResult> Dashboard()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();

            var total = bookings.Count;
            var upcoming = bookings.Count(b => b.StartTime >= DateTime.Now);
            var cancelled = bookings.Count(b => b.Status == Status.Cancelled);
            var avgDuration = bookings.Any() ? bookings.Average(b => b.Duration) : 0;

            var zoneStats = bookings
                .Where(b => b.Zone != null)
                .GroupBy(b => b.Zone.Name)
                .Select(g => new { Zone = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            var model = new DashboardViewModel
            {
                TotalBookings = total,
                UpcomingBookings = upcoming,
                CancelledBookings = cancelled,
                AverageDuration = (int)avgDuration,
                ZoneBookingStats = zoneStats
                    .Select(z => new ZoneStat { ZoneName = z.Zone, BookingCount = z.Count })
                    .ToList()
            };

            return View("Dashboard", model);
        }


        public async Task<IActionResult> Bookings()
        {
            var all = await _bookingService.GetAllBookingsAsync();

            var upcoming = all
                .Where(b => b.StartTime.AddMinutes(b.Duration) >= DateTime.Now)
                .Select(b => new BookingAdminViewModel
                {
                    BookingID = b.BookingID,
                    UserEmail = b.User?.Email,
                    ZoneName = b.Zone?.Name,
                    SeatNumber = b.Seat?.SeatNumber,
                    StartTime = b.StartTime,
                    Duration = b.Duration,
                    Status = b.Status.ToString()
                }).ToList();

            return View("Bookings", upcoming);
        }
        public async Task<IActionResult> PastBookings()
        {
            var all = await _bookingService.GetAllBookingsAsync();

            var past = all
                .Where(b => b.StartTime.AddMinutes(b.Duration) < DateTime.Now)
                .Select(b => new BookingAdminViewModel
                {
                    BookingID = b.BookingID,
                    UserEmail = b.User?.Email,
                    ZoneName = b.Zone?.Name,
                    SeatNumber = b.Seat?.SeatNumber,
                    StartTime = b.StartTime,
                    Duration = b.Duration,
                    Status = b.Status.ToString()
                }).ToList();

            return View("PastBookings", past);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View("Tournaments/CreateNew");
        }

        [HttpPost]
        public async Task<IActionResult> Create(TournamentCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View("CreateNew", model);

            var tournament = new Domain.Models.Tournament
            {
                Name = model.Name,
                Game = model.Game,
                StartDateTime = model.Date + model.Time,
                Status = model.Status,
                Description = model.Description,
                MaxParticipants = model.MaxParticipants,
                ThemeColor = model.ThemeColor
            };

            await _tournamentService.CreateTournamentAsync(tournament);
            return RedirectToAction("Index", "Tournament");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int id, string newStatus)
        {
            if (!Enum.TryParse<Status>(newStatus, out var parsedStatus))
            {
                TempData["Error"] = "Invalid status.";
                return RedirectToAction("Bookings");
            }

            var bookings = await _bookingService.GetAllBookingsAsync();
            var booking = bookings.FirstOrDefault(b => b.BookingID == id);
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Bookings");
            }

            if (booking.Status == parsedStatus)
                return RedirectToAction("Bookings");

            booking.Status = parsedStatus;

            const string updateQuery = "UPDATE Booking SET Status = @Status WHERE BookingID = @BookingID";
            var parameters = new[]
            {
                new SqlParameter("@Status", booking.Status.ToString()),
                new SqlParameter("@BookingID", booking.BookingID)
            };

            var success = await _bookingService.UpdateBookingStatusAsync(booking);
            if (!success)
                ModelState.AddModelError("", "Failed to update booking status.");

            return RedirectToAction("Bookings");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            var tournament = await _tournamentService.GetByIdAsync(id);
            if (tournament == null)
            {
                TempData["Message"] = "Tournament not found.";
                return RedirectToAction("Index", "Tournament");
            }

            // ✅ Check if users are registered
            int registeredCount = await _tournamentService.GetRegisteredCountAsync(id);
            if (registeredCount > 0)
            {
                TempData["Message"] = "You cannot delete this tournament because users are already registered.";
                return RedirectToAction("Index", "Tournament");
            }

            bool success = await _tournamentService.DeleteTournamentAsync(id);

            TempData["Message"] = success
                ? "Tournament deleted successfully."
                : "Failed to delete tournament.";

            return RedirectToAction("Index", "Tournament");
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            bool success = await _bookingService.CancelBookingAsync(bookingId);
            if (!success)
            {
                TempData["Message"] = "Cancellation failed.";
            }
            return RedirectToAction("Bookings");
        }



    }
}
