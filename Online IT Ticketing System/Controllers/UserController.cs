using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Online_IT_Ticketing_System.DAL;
using Online_IT_Ticketing_System.Models;

namespace ITTicketingSystem.Controllers
{
    public class UserController : Controller
    {
       
        public IActionResult UserDashboard()
        {
            
            if (HttpContext.Session.GetString("Role") != "User")
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the UserId from the session
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            // Get ticket counts from DatabaseHelper
            var (totalTickets, closedTickets, processingTickets) = DatabaseHelper.GetTicketCountsForUser(userId);

            // Create a model for the dashboard
            var model = new DashboardViewModel
            {
                TotalTickets = totalTickets,
                ClosedTickets = closedTickets,
                ProcessingTickets = processingTickets
            };

            return View(model); // Pass the data to the UserDashboard view
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
