using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Online_IT_Ticketing_System.Models;
using Online_IT_Ticketing_System.DAL;

namespace Online_IT_Ticketing_System.Controllers
{
    public class SuperAdminController : Controller
    {
        public IActionResult SuperAdminDashboard()
        {
            if (HttpContext.Session.GetString("Role") != "SuperAdmin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public IActionResult CreateSubAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateSubAdmin(SubAdminModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = DatabaseHelper.CreateSubAdmin(
                    model.Name,
                    model.JobField,
                    model.Username,
                    model.Email,
                    model.Password
                );

                if (success)
                {
                    return RedirectToAction("SuperAdminDashboard");
                }

                ViewBag.ErrorMessage = "Failed to create SubAdmin.";
            }

            return View(model);
        }

        public IActionResult DeleteSubAdmin(int id)
        {
            try
            {
                bool success = DatabaseHelper.DeleteSubAdmin(id);

                

                return RedirectToAction("ManageSubAdmin");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting SubAdmin with ID {id}: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the SubAdmin.");
            }
        }

        public IActionResult ManageSubAdmin()
        {
            List<SubAdminModel> subAdmins = DatabaseHelper.GetAllSubAdmins();
            return View(subAdmins);
        }

        public IActionResult EditSubAdmin(int id)
        {
            SubAdminModel subAdmin = DatabaseHelper.GetSubAdminById(id);

            if (subAdmin == null)
            {
                return NotFound();
            }

            return View(subAdmin);
        }

        [HttpPost]
        public IActionResult EditSubAdmin(SubAdminModel model)
        {
            if (ModelState.IsValid)
            {
                bool success = DatabaseHelper.UpdateSubAdmin(
                    model.Id,
                    model.Name,
                    model.JobField,
                    model.Username,
                    model.Email,
                    model.Password
                );

                if (success)
                {
                    return RedirectToAction("ManageSubAdmin");
                }

                ModelState.AddModelError("", "Update failed. SubAdmin not found.");
            }

            return View(model);
        }

        public IActionResult ViewTickets()
        {
            List<TicketModel> tickets = DatabaseHelper.GetAllTickets();
            return View(tickets);
        }

       
        public IActionResult AssignTicket(string TicketId)
        {
            var model = new TicketAssignmentViewModel
            {
                TicketId = TicketId,
                SubAdmins = DatabaseHelper.GetSubAdmins()
                
            };


            return View(model);
        }

        [HttpPost]

        public IActionResult AssignTicket(TicketAssignmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                bool success = DatabaseHelper.AssignTicket(model.TicketId, model.SubAdminId);

                if (success)
                {
                    return RedirectToAction("ViewTickets");
                }

                ViewBag.ErrorMessage = "Failed to assign the ticket.";
            }

            model.SubAdmins = DatabaseHelper.GetSubAdmins();
            return View(model);
        }

        [HttpPost]
        public IActionResult SendMessageToUser(string TicketId, string UserName, string Message)
        {
            if (!string.IsNullOrEmpty(Message))
            {
                bool success = DatabaseHelper.SendMessageToUser(TicketId, UserName, Message);

                if (success)
                {
                    TempData["SuccessMessage"] = "Message sent successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to send the message.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Message cannot be empty.";
            }

            return RedirectToAction("ViewTickets");
        }

        [HttpPost]
        public IActionResult UpdateTicketStatus(string TicketId, string Status)
        {
            bool success = DatabaseHelper.UpdateTicketStatus(TicketId, Status);

            if (success)
            {
                return RedirectToAction("ViewTickets");
            }

            return StatusCode(500, "Failed to update the ticket status.");
        }
    }
}
