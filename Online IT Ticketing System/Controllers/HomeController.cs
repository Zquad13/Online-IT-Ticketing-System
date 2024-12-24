using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Online_IT_Ticketing_System.Models;

namespace Online_IT_Ticketing_System.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}