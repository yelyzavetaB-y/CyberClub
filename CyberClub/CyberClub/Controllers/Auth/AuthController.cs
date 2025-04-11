using CyberClub.Domain.Models;
using CyberClub.Domain.Services;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CyberClub.Controllers.Auth
{
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()                                    /*A-UC-2*/
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }



        [HttpGet("")]
        [HttpGet("Login")]                                                           /*A-UC-0*/
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                return RedirectToAction("Panel", "Customer");
            }
            return View();
        }


        [HttpPost("Login")]                                                          /*A-UC-1*/
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.AuthenticateUserAsync(loginViewModel.Email, loginViewModel.Password);
                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserID", user.Id);
                    HttpContext.Session.SetString("User", user.Email);
                    return RedirectToAction("Panel", "Customer");
                }
                ModelState.AddModelError("", "error");
            }
            return View(loginViewModel);
        }

        [HttpGet("Signup")]                                                        /*C-UC-0*/
        public IActionResult Signup()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                return RedirectToAction("Panel", "Customer");
            }
            return View();
        }

        [HttpPost("Signup")]                                                               /*C-UC-1*/
        public async Task<IActionResult> Signup(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    RoleId = model.RoleId,
                    FullName = model.FullName,
                    HashPassword = model.Password
                };

                var userProfile = new UserProfile
                {
                    PhoneNumber = model.PhoneNumber,
                    DOB = model.DOB.Value,
                    User = user
                };

                bool isCreated = await _userService.CreateUserWithProfileAsync(user, userProfile);
                if (isCreated)
                {
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    ModelState.AddModelError("", "error");
                }
            }

            return View(model);
        }
    }
}
