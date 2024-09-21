using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Seat_Reservation_System.Models;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Seat_Reservation_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly string _connectionString;
     

        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

        [HttpGet]
        public IActionResult GetBookingsByDate(string date,string number)
        {
            var bookings = new List<Bookings>();
            string query = "SELECT * FROM Booking WHERE BookDateTime = @date AND SeatNumber = @seatNumber AND isExpire = 0";

            if (string.IsNullOrEmpty(date))
            {
                Console.Write("Missing");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@date", date);
                    command.Parameters.AddWithValue("@seatNumber", number);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bookings.Add(new Bookings
                            {
                                BookId = (int)reader["BookId"],
                                SeatNumber = reader["SeatNumber"]?.ToString() ?? string.Empty,
                                BookDateTime = (DateTime)reader["BookDateTime"],
                                TraineeID = reader["TraineeId"].ToString(),
                                TraineeNIC = reader["TraineeNIC"].ToString(),
                                TraineeName = reader["TraineeName"].ToString()

                            });

                        }
                        reader.Close();
                        
                    }

                }
            }
            return Json(bookings);

        }

        [HttpGet]
        public IActionResult DeleteBookingsBySeatID(int id)
        {
            
            string query = "UPDATE Booking SET isExpire = 1 WHERE BookId = @id";
            
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    int exe = command.ExecuteNonQuery();

                    if (exe > 0)
                    {
                        return Ok();
                    }
                  
                }
            }

            return BadRequest();
        }

    }
}
   

