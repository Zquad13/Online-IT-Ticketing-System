using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ITTicketingSystem.Controllers
{
    public class UserController : Controller
    {
        // Display User dashboard
        public IActionResult UserDashboard()
        {
            // If the user is not logged in, redirect them to the login page
            if (HttpContext.Session.GetString("Role") != "User")
            {
                return RedirectToAction("Login", "Account");
            }

            return View(); // Return the UserDashboard view
        }

        // ContactUs action
        public IActionResult ContactUs()
        {
            // If the user is not logged in, redirect them to the login page
            if (HttpContext.Session.GetString("Role") != "User")
            {
                return RedirectToAction("Login", "Account");
            }

            return View(); // Return the ContactUs view
        }
    }
}
