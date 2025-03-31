using CyberClub.ContextUtilities;
using CyberClub.Domain.Services;
using CyberClub.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CyberClub.Controllers.Customer
{
    [SessionAuthorize]
    public class CustomerController : Controller
    {
        private readonly UserService _userService;
        public IActionResult Panel()
        {
            return View("CustomerPanel");
        }

        public IActionResult Settings()
        {
            var dobStr = HttpContext.Session.GetString("DOB");
            DateTime dob;
            if (!string.IsNullOrEmpty(dobStr) && DateTime.TryParse(dobStr, out dob))
            {
                // Parsed successfully
            }
            else
            {
                // Handle the case where the date is not available
                dob = DateTime.Now; // Default value or handle it as you see fit
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
        //[HttpPost]
        //public IActionResult UpdateSettings(SettingsViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Update the user data in the database
        //        //var user = _userService.UpdateUserAsync(model);

        //        //// Update session data
        //        //HttpContext.Session.SetString("FullName", model.FullName);
        //        //HttpContext.Session.SetString("Email", model.Email);
        //        //HttpContext.Session.SetString("PhoneNumber", model.PhoneNumber);
        //        //HttpContext.Session.SetString("DOB", model.DOB.ToString("yyyy-MM-dd"));

        //        // Redirect to a confirmation or profile page
        //        return RedirectToAction("ProfileUpdated");
        //    }

        //    // Return the view with the current model to display errors
        //    return View("Settings", model);
        //}
    }
}
