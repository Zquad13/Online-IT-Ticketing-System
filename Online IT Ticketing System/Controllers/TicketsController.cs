using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Online_IT_Ticketing_System.Models;
using Online_IT_Ticketing_System.DAL;
using Microsoft.AspNetCore.Http;

namespace Online_IT_Ticketing_System.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ILogger<TicketsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CreateTicket()
        {
            string userName = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new TicketModel
            {
                UserName = userName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(TicketModel ticket, IFormFile AttachmentPath)
        {
            if (!ModelState.IsValid)
            {
                if (AttachmentPath != null && AttachmentPath.Length > 0)
                {
                    try
                    {
                        
                        using (var memoryStream = new MemoryStream())
                        {
                            await AttachmentPath.CopyToAsync(memoryStream);
                            ticket.AttachmentData = memoryStream.ToArray(); // Store the byte array in the ticket model
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing attachment: {ex.Message}");
                        throw;
                    }
                }

                DatabaseHelper.SaveTicketToDatabase(ticket);
                return RedirectToAction("MyTickets");
            }

            return View(ticket);
        }

        [HttpGet]
        public IActionResult MyTickets()
        {
            string userName = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var tickets = DatabaseHelper.GetTicketsByUserName(userName);
            return View(tickets);
        }

    }

}
