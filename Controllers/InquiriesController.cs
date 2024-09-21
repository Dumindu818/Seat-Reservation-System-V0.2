using System.Linq;
using Seat_Reservation_System.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Seat_Reservation_System.Controllers
{
    public class InquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InquiriesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Inquiries
        public ActionResult Index()
        {
            // Get all inquiries and sort them
            List<Inquiry> inquiries = _context.Inquiries
                .OrderBy(i => i.AdminReplied)
                .ThenBy(i => i.InquiryDate)
                .ToList();

            return View(inquiries);
        }
    }
}
