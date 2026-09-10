using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SportHub_CSharp.Models;

namespace SportHub_CSharp.Controllers
{
    public class InquiryController : Controller
    {
        private SportHubEntities1 db = new SportHubEntities1();


        // =========================================================
        // CREATE - GET
        // =========================================================

        [HttpGet]
        public ActionResult Create()
        {
            return View(new INQUIRY());
        }


        // =========================================================
        // CREATE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(INQUIRY inquiry)
        {
            if (ModelState.IsValid)
            {
                inquiry.SentDate = DateTime.Now;

                db.INQUIRies.Add(inquiry);
                db.SaveChanges();

                TempData["SuccessMessage"] =
                    "Your inquiry has been sent successfully.";

                return RedirectToAction("Create");
            }

            return View(inquiry);
        }


        // =========================================================
        // INDEX
        // =========================================================

        [HttpGet]
        public ActionResult Index()
        {
            var inquiries = db.INQUIRies
                .OrderByDescending(x => x.SentDate)
                .ToList();

            return View(inquiries);
        }


        // =========================================================
        // DETAILS
        // =========================================================

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var inquiry = db.INQUIRies
                .FirstOrDefault(x => x.InquiryID == id.Value);

            if (inquiry == null)
            {
                return HttpNotFound();
            }

            return View(inquiry);
        }


        // =========================================================
        // EDIT - GET
        // =========================================================

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var inquiry = db.INQUIRies
                .FirstOrDefault(x => x.InquiryID == id.Value);

            if (inquiry == null)
            {
                return HttpNotFound();
            }

            return View(inquiry);
        }


        // =========================================================
        // EDIT - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(INQUIRY inquiry)
        {
            if (!ModelState.IsValid)
            {
                return View(inquiry);
            }

            var existingInquiry = db.INQUIRies
                .FirstOrDefault(x => x.InquiryID == inquiry.InquiryID);

            if (existingInquiry == null)
            {
                return HttpNotFound();
            }

            existingInquiry.GuestName = inquiry.GuestName;
            existingInquiry.GuestEmail = inquiry.GuestEmail;
            existingInquiry.Message = inquiry.Message;

            db.SaveChanges();

            TempData["SuccessMessage"] =
                "Inquiry updated successfully.";

            return RedirectToAction("Index");
        }


        // =========================================================
        // DELETE - GET / CONFIRMATION PAGE
        // =========================================================

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            // IMPORTANT:
            // If no ID is supplied, return to Inquiry list
            // instead of generating a 400 Bad Request.

            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var inquiry = db.INQUIRies
                .FirstOrDefault(x => x.InquiryID == id.Value);

            if (inquiry == null)
            {
                return HttpNotFound();
            }

            return View(inquiry);
        }


        // =========================================================
        // DELETE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int InquiryID)
        {
            var inquiry = db.INQUIRies
                .FirstOrDefault(x => x.InquiryID == InquiryID);

            if (inquiry == null)
            {
                return HttpNotFound();
            }

            db.INQUIRies.Remove(inquiry);
            db.SaveChanges();

            TempData["SuccessMessage"] =
                "Inquiry deleted successfully.";

            return RedirectToAction("Index");
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