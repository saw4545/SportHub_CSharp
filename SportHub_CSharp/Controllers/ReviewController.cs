using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SportHub_CSharp.Models;

namespace SportHub_CSharp.Controllers
{
    public class ReviewController : Controller
    {
        private SportHubEntities1 db = new SportHubEntities1();

        // ================================
        // REVIEW INDEX
        // ================================

        public ActionResult Index(string search, int? rating)
        {
            var reviews = db.REVIEWs
                .Include(r => r.BOOKING)
                .AsQueryable();

            // Search by review comment or booking ID
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                int bookingId;

                bool isBookingId = int.TryParse(search, out bookingId);

                if (isBookingId)
                {
                    reviews = reviews.Where(r =>
                        r.ReviewComment.Contains(search) ||
                        r.BookingID == bookingId
                    );
                }
                else
                {
                    reviews = reviews.Where(r =>
                        r.ReviewComment.Contains(search)
                    );
                }
            }

            // Filter by rating
            if (rating.HasValue && rating.Value >= 1 && rating.Value <= 5)
            {
                reviews = reviews.Where(r =>
                    r.Rating == rating.Value
                );
            }

            var result = reviews
                .OrderByDescending(r => r.ReviewDate)
                .ToList();

            ViewBag.Search = search;
            ViewBag.Rating = rating;

            return View(result);
        }

        // ================================
        // CREATE REVIEW - GET
        // ================================
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            int memberId = Convert.ToInt32(Session["MemberID"]);

            // Get bookings belonging to logged-in member
            var bookings = db.BOOKINGs
                .Where(b => b.MemberID == memberId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            ViewBag.BookingList = new SelectList(
                bookings,
                "BookingID",
                "BookingID"
            );

            return View();
        }


        // ================================
        // CREATE REVIEW - POST
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            int? BookingID,
            int? Rating,
            string ReviewComment)
        {
            // Check login
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            int memberId = Convert.ToInt32(Session["MemberID"]);


            // ================================
            // VALIDATION
            // ================================

            if (BookingID == null)
            {
                ModelState.AddModelError(
                    "BookingID",
                    "Please select a booking."
                );
            }

            if (Rating == null)
            {
                ModelState.AddModelError(
                    "Rating",
                    "Please select a rating."
                );
            }

            if (Rating.HasValue &&
                (Rating.Value < 1 || Rating.Value > 5))
            {
                ModelState.AddModelError(
                    "Rating",
                    "Rating must be between 1 and 5."
                );
            }

            if (string.IsNullOrWhiteSpace(ReviewComment))
            {
                ModelState.AddModelError(
                    "ReviewComment",
                    "Please enter your review."
                );
            }


            // ================================
            // CHECK BOOKING
            // ================================

            BOOKING booking = null;

            if (BookingID.HasValue)
            {
                booking = db.BOOKINGs
                    .FirstOrDefault(b =>
                        b.BookingID == BookingID.Value &&
                        b.MemberID == memberId);
            }

            if (BookingID.HasValue && booking == null)
            {
                ModelState.AddModelError(
                    "BookingID",
                    "Invalid booking selected."
                );
            }


            // ================================
            // SAVE REVIEW
            // ================================

            if (ModelState.IsValid)
            {
                // Prevent duplicate review for same booking
                bool alreadyReviewed = db.REVIEWs
                    .Any(r => r.BookingID == BookingID.Value);

                if (alreadyReviewed)
                {
                    TempData["ErrorMessage"] =
                        "You have already submitted a review for this booking.";

                    return RedirectToAction("Index");
                }

                REVIEW review = new REVIEW
                {
                    BookingID = BookingID.Value,
                    Rating = Rating.Value,
                    ReviewComment = ReviewComment.Trim(),
                    ReviewDate = DateTime.Now
                };

                db.REVIEWs.Add(review);

                db.SaveChanges();

                TempData["SuccessMessage"] =
                    "Your review was submitted successfully.";

                return RedirectToAction("Index");
            }


            // Rebuild dropdown if validation fails
            var bookings = db.BOOKINGs
                .Where(b => b.MemberID == memberId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            ViewBag.BookingList = new SelectList(
                bookings,
                "BookingID",
                "BookingID",
                BookingID
            );

            return View();
        }


        // ================================
        // EDIT - GET
        // ================================
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            REVIEW review = db.REVIEWs.Find(id);

            if (review == null)
            {
                return HttpNotFound();
            }

            return View(review);
        }


        // ================================
        // EDIT - POST
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(REVIEW review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State =
                    EntityState.Modified;

                db.SaveChanges();

                TempData["SuccessMessage"] =
                    "Review updated successfully.";

                return RedirectToAction("Index");
            }

            return View(review);
        }


        // ================================
        // DELETE - GET
        // ================================
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            REVIEW review = db.REVIEWs
                .Include(r => r.BOOKING)
                .FirstOrDefault(r => r.ReviewID == id.Value);

            if (review == null)
            {
                return HttpNotFound();
            }

            return View(review);
        }


        // ================================
        // DELETE - POST
        // ================================
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            REVIEW review = db.REVIEWs.Find(id);

            if (review == null)
            {
                return HttpNotFound();
            }

            db.REVIEWs.Remove(review);

            db.SaveChanges();

            TempData["SuccessMessage"] =
                "Review deleted successfully.";

            return RedirectToAction("Index");
        }


        // ================================
        // DISPOSE
        // ================================
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