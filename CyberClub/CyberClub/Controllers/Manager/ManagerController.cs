using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

      
        public async Task<IActionResult> Bookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync(); 
            var model = bookings.Select(b => new BookingAdminViewModel
            {
                BookingID = b.BookingID,
                UserEmail = b.User?.Email,
                ZoneName = b.Zone?.Name,
                SeatNumber = b.Seat?.SeatNumber,
                StartTime = b.StartTime,
                Duration = b.Duration,
                Status = b.Status.ToString()
            }).ToList();

            return View("Bookings", model);
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
                Description = model.Description
            };

            await _tournamentService.CreateTournamentAsync(tournament);
            return RedirectToAction("CreateNew");
        }

    }
}
