using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seat_Reservation_System.Models;
using System.Diagnostics;

namespace Seat_Reservation_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Hard-coded admin credentials
                string adminUsername = "admin";
                string adminPassword = "admin";

                if (model.AdminUsername == adminUsername && model.AdminPassword == adminPassword)
                {
                    // Set a session or cookie to remember that the admin is logged in
                    HttpContext.Session.SetString("AdminLoggedIn", "true");

                    // Redirect to the admin dashboard or any other page
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid admin credentials");
                }
            }

            return View(model);
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                return View();
            }

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLoggedIn");
            return RedirectToAction("Login");
        }

        public IActionResult AdminManageSeats()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                return View();
            }

            return RedirectToAction("Login");
        }

        public IActionResult AdminBookingLog()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                return View();
            }

            return RedirectToAction("Login");
        }

        public IActionResult AdminInquiries()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                var inquiries = _context.Inquiries
                           .Select(i => new Inquiry
                           {
                               InquiryId = i.InquiryId,
                               UserId = i.UserId,
                               Title = i.Title ?? "No Title",
                               Description = i.Description ?? "No Description",
                               AdminReplied = i.AdminReplied,
                               AdminReply = i.AdminReply ?? "No Reply",
                               InquiryDate = i.InquiryDate
                           })
                           .ToList();
                return View(inquiries);
            }

            return RedirectToAction("Login");
        }

        public IActionResult AdminInquiriesHistorySub(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                var inquiry = _context.Inquiries.Find(id);
                if (inquiry == null)
                {
                    return NotFound();
                }

                return View(inquiry);
            }

            return RedirectToAction("Login");
        }

        public IActionResult AdminInquiriesHistoryNotSub(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                var inquiry = _context.Inquiries.Find(id);
                if (inquiry == null)
                {
                    return NotFound();
                }

                return View(inquiry);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult EditReply(int id, string AdminReply)
        {
            // Find the inquiry by id
            var inquiry = _context.Inquiries.Find(id);

            if (inquiry != null)
            {
                // Update the reply and mark it as replied
                inquiry.AdminReply = AdminReply;
                inquiry.AdminReplied = true;

                // Save changes to the database
                _context.SaveChanges();
            }

            // Redirect back to the inquiries list
            return RedirectToAction("AdminInquiries");
        }



    }
}
   

