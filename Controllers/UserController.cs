using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Seat_Reservation_System.Models;
using System.Data.SqlTypes;

namespace Seat_Reservation_System.Controllers
{
    public class UserController : Controller
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Dashboard()
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {

                ViewBag.Username = username;
                return View();
            }

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }

       

        public IActionResult UserProfile()
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    ViewBag.Username = user.Username;
                    ViewBag.Email = user.Email;
                    ViewBag.Password = user.Password;// Pass the email to the view
                    ViewBag.TraineeName = user.TraineeName;
                    ViewBag.TraineeNIC = user.TraineeNIC;
                    return View();
                }
            }

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }
        public IActionResult UserProfileEdit()
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    ViewBag.Username = user.Username;
                    ViewBag.Email = user.Email;
                    ViewBag.Password = user.Password;// Pass the email to the view
                    ViewBag.TraineeName = user.TraineeName;
                    ViewBag.TraineeNIC = user.TraineeNIC;
                    return View();
                }
            }

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult UpdateUserProfile([FromBody] User model)
        {
           
                var currentUsername = HttpContext.Session.GetString("Username");

                if (!string.IsNullOrEmpty(currentUsername))
                {
                    string query = "UPDATE Users SET Username = @Username, Email = @Email, Password = @Password , TraineeName = @traineeName , TraineeNIC = @traineeNIC WHERE Username = @CurrentUsername";

                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", model.Username);
                            cmd.Parameters.AddWithValue("@Email", model.Email);
                            cmd.Parameters.AddWithValue("@Password", model.Password); // Make sure to hash the password in a real application
                            cmd.Parameters.AddWithValue("@CurrentUsername", currentUsername);
                            cmd.Parameters.AddWithValue("@traineeName",model.TraineeName);
                            cmd.Parameters.AddWithValue("@traineeNIC", model.TraineeNIC);

                        int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                // Update session information if needed
                                HttpContext.Session.SetString("Username", model.Username);

                                // Redirect to Index page in Home folder
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }
           
                return BadRequest();
        ;
        }



        public IActionResult UserInquiries()
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    ViewBag.Id = user.Id;
                    ViewBag.Username = user.Username;
                    ViewBag.Email = user.Email;
                    ViewBag.Password = user.Password;// Pass the email to the view
                    return View();
                }
            }

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }

        public IActionResult UserMyBookings()
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    ViewBag.Username = user.Username;
                    ViewBag.Email = user.Email;
                    ViewBag.Password = user.Password;// Pass the email to the view
                    return View();
                }
            }

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }

        public IActionResult UserBookNow()
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    ViewBag.Username = user.Username;
                    ViewBag.Email = user.Email;
                    ViewBag.Password = user.Password;// Pass the email to the view
                    return View();
                }
            }

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }

        public IActionResult UserInquiriesHistory()
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                ViewBag.Username = username;

                // Prepare the SQL query to get the user's inquiries
                string query = @"
                                SELECT 
                                    *
                                FROM Inquiries
                                WHERE UserId = @Username
                                ORDER BY 
                                    AdminReplied DESC, 
                                    InquiryDate ASC";

                List<Inquiry> inquiries = new List<Inquiry>();

                using (SqlConnection conn = new SqlConnection(_connectionString)) // Ensure _connectionString is defined in your class
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        command.Parameters.AddWithValue("@Username", username);

                        // Execute the query
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inquiries.Add(new Inquiry
                                {
                                    InquiryId = (int)reader["InquiryId"],
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    UserId = reader["UserId"].ToString(),
                                    InquiryDate = DateOnly.FromDateTime((DateTime)reader["InquiryDate"]),
                                    AdminReplied = (bool)reader["AdminReplied"],
                                    AdminReply = reader["AdminReply"].ToString()
                                });
                            }
                        }
                    }
                }

                return View(inquiries); // Pass the inquiries list to the view
            }

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }



        public IActionResult UserInquiriesHistoryDetails(int id)
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
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
                                    AdminReplied = (bool)reader["AdminReplied"],
                                    AdminReply = reader["AdminReply"].ToString()
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

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }

        public IActionResult UserInquiriesHistoryDetailsNotRep(int id)
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
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

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }


        public IActionResult SeatBooked()
        {
            // Get the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    ViewBag.Username = user.Username;
                    ViewBag.Email = user.Email;
                    ViewBag.Password = user.Password;// Pass the email to the view
                    return View();
                }
            }

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public IActionResult SubmitInquiry(string Title, string Description)
        {
            var username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
            {
                
                // Now, create a SQL query to insert the inquiry
                string insertInquiryQuery = @"
                INSERT INTO Inquiries (UserId, Title, Description, InquiryDate, AdminReplied)
                VALUES (@UserId, @Title, @Description, @InquiryDate, @AdminReplied)";

                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Use SqlCommand to execute the insert query
                    using (var command = new SqlCommand(insertInquiryQuery, conn))
                    {
                        // Add parameters for the SQL query
                        command.Parameters.AddWithValue("@UserId", username);
                        command.Parameters.AddWithValue("@Title", Title);
                        command.Parameters.AddWithValue("@Description", Description);
                        command.Parameters.AddWithValue("@InquiryDate", DateTime.Now); // Inquiry date is the current date
                        command.Parameters.AddWithValue("@AdminReplied", false); // AdminReplied is initially false

                        // Execute the insert command
                        command.ExecuteNonQuery();
                    }
                }

                // Redirect to the UserInquiriesHistory view after successful insert
                return RedirectToAction("UserInquiriesHistory", "User");
            }

            // If session is expired or username not found, redirect to login
            return RedirectToAction("Login", "Account");
        }


    }
}
