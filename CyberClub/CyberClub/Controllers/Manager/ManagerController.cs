using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberClub.Controllers.Manager
{
    public class ManagerController : Controller
    {
        private readonly UserService _userService;

        public ManagerController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        
    }
}
