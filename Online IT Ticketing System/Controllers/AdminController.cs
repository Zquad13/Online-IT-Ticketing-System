using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ITTicketingSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult SuperAdminDashboard()
        {
            // Check if  SuperAdmin or not
            if (HttpContext.Session.GetString("Role") != "SuperAdmin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
