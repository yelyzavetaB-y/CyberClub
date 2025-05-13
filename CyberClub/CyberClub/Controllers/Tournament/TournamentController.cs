using Microsoft.AspNetCore.Mvc;

namespace CyberClub.Controllers.Tournament
{
    public class TournamentController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Customer/Tournaments.cshtml");
        }
    }
}
