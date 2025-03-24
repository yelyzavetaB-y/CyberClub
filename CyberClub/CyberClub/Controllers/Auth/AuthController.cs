using Microsoft.AspNetCore.Mvc;

namespace CyberClub.Controllers.Auth
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || password != confirmPassword)
            {
                ViewBag.Error = "Invalid data provided.";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            return RedirectToAction("Index", "Home");
        }

    }
}
