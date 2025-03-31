using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CyberClub.ContextUtilities
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}
