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
        string query = "SELECT * FROM Inquiries WHERE InquiryId = @id";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var inquiry = new Inquiry
                        {
                            InquiryId = (int)reader["InquiryId"],
                            UserId = reader["UserId"].ToString(),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            InquiryDate = DateOnly.FromDateTime((DateTime)reader["InquiryDate"]),
                            AdminReplied = (bool)reader["AdminReplied"],
                            AdminReply = reader["AdminReply"] != DBNull.Value ? reader["AdminReply"].ToString() : null
                        };

                        return View(inquiry);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }
    }

    return RedirectToAction("Login");
}


        public IActionResult AdminInquiriesHistoryNotSub(int id)
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                string query = "SELECT * FROM Inquiries WHERE InquiryId = @inquiryid";
                Inquiry inquiry = null;

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameter to prevent SQL injection
                        command.Parameters.AddWithValue("@inquiryid", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Assuming you have an Inquiry class with these properties
                                inquiry = new Inquiry
                                {
                                    InquiryId = (int)reader["InquiryId"],
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    UserId = reader["UserId"].ToString(),
                                    InquiryDate = DateOnly.FromDateTime((DateTime)reader["InquiryDate"]),
                                    AdminReplied = (bool)reader["AdminReplied"]
                                };
                            }
                        }
                    }
                }

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
            string query = "UPDATE Inquiries SET AdminReply = @AdminReply, AdminReplied = 1 WHERE InquiryId = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    // Add parameters to avoid SQL injection
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@AdminReply", AdminReply);

                    // Execute the query
                    int rowsAffected = command.ExecuteNonQuery();

                    // Check if the update was successful
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("AdminInquiries");
                    }
                }
            }

            // If the update failed, you can return an error or the same view
            return BadRequest();
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
   

