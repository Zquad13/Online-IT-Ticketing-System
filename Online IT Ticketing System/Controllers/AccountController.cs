using ITTicketingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Online_IT_Ticketing_System.DAL;
using Online_IT_Ticketing_System.Models;
using System;

namespace ITTicketingSystem.Controllers
{
    public class AccountController : Controller
    {
        private const string SuperAdminUsername = "admin";
        private const string SuperAdminPassword = "qwerty123456";

        [HttpGet]
        public IActionResult Login()
        {
            string username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                string role = HttpContext.Session.GetString("Role");
                if (role == "SuperAdmin")
                {
                    return RedirectToAction("SuperAdminDashboard", "SuperAdmin");
                }
                else if (role == "User")
                {
                    return RedirectToAction("UserDashboard", "User");
                }
                else if (role == "SubAdmin")
                {
                    return RedirectToAction("SubAdminDashboard", "SubAdmin");
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if it's the SuperAdmin login
                if (model.Username == SuperAdminUsername && model.Password == SuperAdminPassword)
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    HttpContext.Session.SetString("Role", "SuperAdmin");
                    return RedirectToAction("SuperAdminDashboard", "SuperAdmin");
                }

                // Check if it's a valid User login
                if (IsValidUser(model.Username, model.Password))
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    HttpContext.Session.SetString("Role", "User");
                    return RedirectToAction("UserDashboard", "User");
                }

                // Check if it's a valid SubAdmin login
                if (IsValidSubAdmin(model.Username, model.Password))
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    HttpContext.Session.SetString("Role", "SubAdmin");
                    return RedirectToAction("SubAdminDashboard", "SubAdmin");
                }

                ViewBag.ErrorMessage = "Invalid username or password.";
            }

            return View(model);
        }

        private bool IsValidUser(string username, string password)
        {
            // Validate user using DatabaseHelper
            string storedPasswordHash = DatabaseHelper.ValidateUser(username);
            return !string.IsNullOrEmpty(storedPasswordHash) && BCrypt.Net.BCrypt.Verify(password, storedPasswordHash);
        }

        private bool IsValidSubAdmin(string username, string password)
        {
            // Validate SubAdmin using DatabaseHelper
            string storedPassword = DatabaseHelper.ValidateSubAdmin(username);
            return !string.IsNullOrEmpty(storedPassword) && storedPassword == password;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Hash the password using BCrypt for users
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                bool isRegistered = DatabaseHelper.SignUpUser(
                    model.FirstName,
                    model.LastName,
                    model.DateOfBirth,
                    model.Gender,
                    model.PhoneNumber,
                    model.Email,
                    model.Address,
                    model.State,
                    model.City,
                    model.Username,
                    hashedPassword
                );

                if (isRegistered)
                {
                    return RedirectToAction("Login", "Account");
                }

                ViewBag.ErrorMessage = "Registration failed. Please try again.";
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
