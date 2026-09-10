using SportHub_CSharp.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SportHub_CSharp.Controllers
{
    public class BookingController : Controller
    {
        private SportHubEntities1 db = new SportHubEntities1();

        // =========================================================
        // GET: Booking
        // =========================================================

        public ActionResult Index()
        {
            var bookings = db.BOOKINGs
                .Include(b => b.FACILITY)
                .Include(b => b.MEMBER)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            return View(bookings);
        }


        // =========================================================
        // GET: Booking/Create
        // =========================================================

        [HttpGet]
        public ActionResult Create(int? id)
        {
            // User must be logged in
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            // Facility ID required
            if (id == null)
            {
                TempData["ErrorMessage"] =
                    "Please select a facility first.";

                return RedirectToAction("Index", "Facility");
            }

            // Find facility
            var facility = db.FACILITies
                .FirstOrDefault(f => f.FacilityID == id.Value);

            if (facility == null)
            {
                return HttpNotFound();
            }

            // Check facility status
            if (!string.Equals(
                facility.Status,
                "Available",
                StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] =
                    "Sorry, this facility is currently unavailable.";

                return RedirectToAction("Index", "Facility");
            }

            // Send facility information to view
            ViewBag.FacilityID = facility.FacilityID;
            ViewBag.FacilityName = facility.Name;
            ViewBag.FacilityType = facility.Type;
            ViewBag.Location = facility.Location;
            ViewBag.HourlyRate = facility.HourlyRate;

            return View();
        }


        // =========================================================
        // POST: Booking/Create
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            int FacilityID,
            DateTime BookingDate,
            string TimeSlot)
        {
            // Login check
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            int memberId =
                Convert.ToInt32(Session["MemberID"]);


            // =====================================================
            // FIND FACILITY
            // =====================================================

            var facility = db.FACILITies
                .FirstOrDefault(f =>
                    f.FacilityID == FacilityID);

            if (facility == null)
            {
                TempData["ErrorMessage"] =
                    "The selected facility could not be found.";

                return RedirectToAction("Index", "Facility");
            }


            // =====================================================
            // CHECK FACILITY STATUS
            // =====================================================

            if (!string.Equals(
                facility.Status,
                "Available",
                StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] =
                    "Sorry, this facility is currently unavailable.";

                return RedirectToAction("Index", "Facility");
            }


            // =====================================================
            // VALIDATE DATE
            // =====================================================

            if (BookingDate.Date < DateTime.Today)
            {
                TempData["ErrorMessage"] =
                    "You cannot make a booking for a past date.";

                return RedirectToAction(
                    "Create",
                    new { id = FacilityID });
            }


            // =====================================================
            // VALIDATE TIME SLOT
            // =====================================================

            if (string.IsNullOrWhiteSpace(TimeSlot))
            {
                TempData["ErrorMessage"] =
                    "Please select a time slot.";

                return RedirectToAction(
                    "Create",
                    new { id = FacilityID });
            }


            // =====================================================
            // CHECK DUPLICATE BOOKING
            // =====================================================

            bool alreadyBooked = db.BOOKINGs.Any(b =>
                b.FacilityID == FacilityID &&
                DbFunctions.TruncateTime(b.BookingDate)
                    == BookingDate.Date &&
                b.TimeSlot == TimeSlot &&
                b.Status == "Confirmed");


            if (alreadyBooked)
            {
                TempData["ErrorMessage"] =
                    "Sorry, this facility is already booked for the selected date and time.";

                return RedirectToAction(
                    "Create",
                    new { id = FacilityID });
            }


            // =====================================================
            // CREATE BOOKING
            // =====================================================

            var booking = new BOOKING
            {
                MemberID = memberId,
                FacilityID = FacilityID,
                BookingDate = BookingDate.Date,
                TimeSlot = TimeSlot,
                Status = "Confirmed",
                CreatedAt = DateTime.Now
            };


            db.BOOKINGs.Add(booking);

            db.SaveChanges();


            // =====================================================
            // SUCCESS
            // =====================================================

            TempData["SuccessMessage"] =
                "Booking confirmed successfully!";


            return RedirectToAction("MyBookings");
        }


        // =========================================================
        // GET: Booking/MyBookings
        // =========================================================

        [HttpGet]
        public ActionResult MyBookings()
        {
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            int memberId =
                Convert.ToInt32(Session["MemberID"]);


            var bookings = db.BOOKINGs
                .Include(b => b.FACILITY)
                .Where(b => b.MemberID == memberId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();


            return View(bookings);
        }


        // =========================================================
        // GET: Booking/Details/5
        // =========================================================

        public ActionResult Details(int? id)
        {
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            int memberId =
                Convert.ToInt32(Session["MemberID"]);


            var booking = db.BOOKINGs
                .Include(b => b.FACILITY)
                .FirstOrDefault(b =>
                    b.BookingID == id.Value &&
                    b.MemberID == memberId);


            if (booking == null)
            {
                return HttpNotFound();
            }


            return View(booking);
        }


        // =========================================================
        // GET: Booking/Cancel/5
        // =========================================================

        public ActionResult Cancel(int? id)
        {
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            int memberId =
                Convert.ToInt32(Session["MemberID"]);


            var booking = db.BOOKINGs
                .Include(b => b.FACILITY)
                .FirstOrDefault(b =>
                    b.BookingID == id.Value &&
                    b.MemberID == memberId);


            if (booking == null)
            {
                return HttpNotFound();
            }


            if (booking.Status == "Cancelled")
            {
                TempData["ErrorMessage"] =
                    "This booking has already been cancelled.";

                return RedirectToAction("MyBookings");
            }


            return View(booking);
        }


        // =========================================================
        // POST: Booking/CancelConfirmed
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelConfirmed(int BookingID)
        {
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            int memberId =
                Convert.ToInt32(Session["MemberID"]);


            var booking = db.BOOKINGs
                .FirstOrDefault(b =>
                    b.BookingID == BookingID &&
                    b.MemberID == memberId);


            if (booking == null)
            {
                return HttpNotFound();
            }


            if (booking.Status == "Cancelled")
            {
                TempData["ErrorMessage"] =
                    "This booking has already been cancelled.";

                return RedirectToAction("MyBookings");
            }


            booking.Status = "Cancelled";

            db.SaveChanges();


            TempData["SuccessMessage"] =
                "Booking cancelled successfully.";


            return RedirectToAction("MyBookings");
        }


        // =========================================================
        // DISPOSE
        // =========================================================

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}