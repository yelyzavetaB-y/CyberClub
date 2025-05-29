using CyberClub.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberClub.Controllers.Tournament
{
    public class TournamentController : Controller
    {
        private readonly TournamentService _tournamentService;

        public TournamentController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }
        public async Task<IActionResult> Index()
        {
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            return View("~/Views/Manager/Tournaments/TournamentList.cshtml", tournaments);
        }

        public async Task<IActionResult> IndexForCustomers()
        {
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            return View("~/Views/Customer/Tournaments.cshtml", tournaments);
        }
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int tournamentId)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            // ✅ Check if tournament exists and is open
            var tournament = await _tournamentService.GetByIdAsync(tournamentId);
            if (tournament == null || tournament.Status != "Upcoming")
            {
                TempData["Message"] = "This tournament is not available for registration.";
                return RedirectToAction("IndexForCustomers");
            }

            // ✅ Optional: Max participants limit
            int currentCount = await _tournamentService.GetRegisteredCountAsync(tournamentId);
            if (tournament.MaxParticipants.HasValue && currentCount >= tournament.MaxParticipants.Value)
            {
                TempData["Message"] = "Tournament is full.";
                return RedirectToAction("IndexForCustomers");
            }

            // Proceed with registration
            bool success = await _tournamentService.RegisterUserAsync(tournamentId, userId.Value);

            TempData["Message"] = success
                ? "You have successfully registered!"
                : "Registration failed.";

            return RedirectToAction("MyBookings", "Customer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int tournamentId)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            bool success = await _tournamentService.CancelUserBookingAsync(tournamentId, userId.Value);

            TempData["Message"] = success
                ? "Tournament registration cancelled."
                : "Cancellation failed or you are not registered.";

            return RedirectToAction("MyBookings", "Customer");
        }




    }
}
