using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Online_IT_Ticketing_System.Models;
using Online_IT_Ticketing_System.DAL;
using System.Collections.Generic;

namespace Online_IT_Ticketing_System.Controllers
{
    public class SubAdminController : Controller
    {
        public IActionResult SubAdminDashboard()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the SubAdmin's ID from the database using the username
            int subAdminId = DatabaseHelper.GetSubAdminIdByUsername(username);

            // Get the SubAdmin's tickets using SubAdminId
            var tickets = DatabaseHelper.GetTicketsForSubAdmin(subAdminId);

            return View(tickets);
        }

        [HttpPost]
        public IActionResult SendMessageToUser(string TicketId, string UserName, string Message)
        {
            if (!string.IsNullOrEmpty(Message))
            {
                bool isSent = DatabaseHelper.SendMessageToUser(TicketId, UserName, Message);

                if (isSent)
                    TempData["SuccessMessage"] = "Message sent successfully.";
                else
                    TempData["ErrorMessage"] = "Failed to send the message.";
            }
            else
            {
                TempData["ErrorMessage"] = "Message cannot be empty.";
            }

            return RedirectToAction("SubAdminDashboard");
        }

        [HttpPost]
        public IActionResult UpdateTicketStatus(string TicketId, string Status)
        {
            bool isUpdated = DatabaseHelper.UpdateTicketStatus(TicketId, Status);

            if (isUpdated)
                TempData["SuccessMessage"] = "Ticket status updated successfully.";
            else
                TempData["ErrorMessage"] = "Failed to update ticket status.";

            return RedirectToAction("SubAdminDashboard");
        }
    }
}
