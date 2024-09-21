using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Seat_Reservation_System.Models;
using System.Data.SqlTypes;

namespace Seat_Reservation_System.Controllers
{
    public class UserController : Controller
    {
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

        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult UpdateUserProfile(User model, string confirmPassword)
        {
            // Check if the model is valid and the password matches the confirmation password
            if (ModelState.IsValid && model.Password == confirmPassword)
            {
                // Find the current user based on the username from the session
                var currentUser = _context.Users.FirstOrDefault(u => u.Username == model.Username);
                if (currentUser != null)
                {
                    // Update the user details
                    currentUser.Username = model.Username;
                    currentUser.Email = model.Email;
                    currentUser.Password = model.Password; // Make sure to hash the password in a real application

                    // Mark the entity as modified
                    _context.Entry(currentUser).State = EntityState.Modified;

                    // Save changes to the database
                    _context.SaveChanges();

                    // Update session information if needed
                    HttpContext.Session.SetString("Username", currentUser.Username);

                    // Redirect to Index page in Home folder
                    return RedirectToAction("Index", "Home");
                }
            }

            // If validation fails or password doesn't match, return to the edit view with the model data
            ModelState.AddModelError("", "Password confirmation does not match or input is invalid.");
            return View("UserProfileEdit", model);
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
                    var user = _context.Users.FirstOrDefault(u => u.Username == username);
                    if (user != null)
                    {
                        var inquiries = _context.Inquiries
                            .Where(i => i.UserId == user.Id)
                            .Select(i => new Inquiry
                            {
                                Title = i.Title ?? "No Title", // Handle null title
                                InquiryDate = i.InquiryDate, // If InquiryDate is not nullable
                                AdminReplied = i.AdminReplied // If AdminReplied is not nullable
                            })
                            .OrderByDescending(i => i.AdminReplied) // AdminReplied inquiries at the top
                            .ThenBy(i => i.InquiryDate) // Sort by InquiryDate for inquiries with the same AdminReplied status
                            .ToList();

                        return View(inquiries);
                    }
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
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    ViewBag.Id = user.Id;
                    ViewBag.Username = user.Username;
                    ViewBag.Email = user.Email;
                    ViewBag.Password = user.Password;// Pass the email to the view
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
            }

            // If the session does not exist, redirect to the login page
            return RedirectToAction("Login", "Account");
        }

        public IActionResult UserInquiriesHistoryDetailsNotRep()
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
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    // Create a new Inquiry object
                    var inquiry = new Inquiry
                    {
                        UserId = user.Id,
                        Title = Title,
                        Description = Description,
                        InquiryDate = DateOnly.FromDateTime(DateTime.Now), // Save current date
                        AdminReplied = false
                    };

                    // Add the inquiry to the database
                    _context.Inquiries.Add(inquiry);
                    _context.SaveChanges();

                    // Redirect to the UserInquiriesHistory view
                    return RedirectToAction("UserInquiriesHistory", "User");
                }
            }

            // If user is not found or session expired, redirect to login
            return RedirectToAction("Login", "Account");
        }


    }
}
