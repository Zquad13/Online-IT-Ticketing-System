using ITTicketingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Online_IT_Ticketing_System.DAL;
using Online_IT_Ticketing_System.Models;
using System;
using Microsoft.AspNetCore.Authentication;

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
                // Hash the password using BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Check if the email is already taken using DatabaseHelper
                if (DatabaseHelper.IsEmailTaken(model.Email))
                {
                    ViewBag.ErrorMessage = "The email is already registered.";
                    return View(model); // Return with error message
                }

                // Check if the username is already taken using DatabaseHelper
                if (DatabaseHelper.IsUsernameTaken(model.Username))
                {
                    ViewBag.ErrorMessage = "The username is already taken.";
                    return View(model); // Return with error message
                }

                // Register the user using DatabaseHelper
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

                // Check if the registration was successful
                if (isRegistered)
                {
                    // Redirect to the login page after successful registration
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    // If registration fails, show an error message
                    ViewBag.ErrorMessage = "Registration failed. Please try again.";
                }
            }

            // Return to the registration view if the model state is invalid or registration fails
            return View(model);
        }

        public IActionResult Logout()
        {
            // Clear the session or authentication token
            HttpContext.Session.Clear(); // Example for session-based authentication
            HttpContext.SignOutAsync(); // Example for cookie-based authentication

            // Disable caching for the previous pages
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            return RedirectToAction("Login", "Account");
        }

    }
}
