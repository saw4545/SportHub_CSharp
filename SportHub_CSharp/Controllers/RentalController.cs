using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SportHub_CSharp.Models;

namespace SportHub_CSharp.Controllers
{
    public class RentalController : Controller
    {
        private SportHubEntities1 db = new SportHubEntities1();

        // GET: Rental
        public ActionResult Index()
        {
            int? memberId = Session["MemberID"] as int?;

            if (memberId == null)
            {
                return RedirectToAction("Login", "Member");
            }

            var rentals = db.RENTALs
                .Include(r => r.EQUIPMENT)
                .Include(r => r.MEMBER)
                .Where(r => r.MemberID == memberId.Value)
                .OrderByDescending(r => r.RentDate)
                .ToList();

            return View(rentals);
        }

        // GET: Rental/Create/5
        public ActionResult Create(int? id)
        {
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            if (id == null)
            {
                return RedirectToAction("Index", "Equipment");
            }

            var equipment = db.EQUIPMENTs
                .FirstOrDefault(e => e.EquipmentID == id.Value);

            if (equipment == null)
            {
                return HttpNotFound();
            }

            if (equipment.QuantityAvailable <= 0)
            {
                TempData["ErrorMessage"] =
                    "Sorry, this equipment is currently unavailable.";

                return RedirectToAction("Index", "Equipment");
            }

            ViewBag.EquipmentID = equipment.EquipmentID;
            ViewBag.EquipmentName = equipment.Name;
            ViewBag.RentalFee = 0;

            return View();
        }

        // POST: Rental/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int EquipmentID)
        {
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            int memberId = Convert.ToInt32(Session["MemberID"]);

            var equipment = db.EQUIPMENTs
                .FirstOrDefault(e => e.EquipmentID == EquipmentID);

            if (equipment == null)
            {
                return HttpNotFound();
            }

            if (equipment.QuantityAvailable <= 0)
            {
                TempData["ErrorMessage"] =
                    "Sorry, this equipment is no longer available.";

                return RedirectToAction("Index", "Equipment");
            }

            var rental = new RENTAL
            {
                MemberID = memberId,
                EquipmentID = EquipmentID,
                RentDate = DateTime.Now,
                ReturnDate = null,
                Status = "Active"
            };

            db.RENTALs.Add(rental);

            equipment.QuantityAvailable =
                equipment.QuantityAvailable - 1;

            db.SaveChanges();

            TempData["SuccessMessage"] =
                "Equipment rented successfully.";

            return RedirectToAction("Index");
        }

        // GET: Rental/ReturnEquipment/5
        public ActionResult ReturnEquipment(int? id)
        {
            if (Session["MemberID"] == null)
            {
                return RedirectToAction("Login", "Member");
            }

            if (id == null)
            {
                return RedirectToAction("Index");
            }

            int memberId = Convert.ToInt32(Session["MemberID"]);

            var rental = db.RENTALs
                .Include(r => r.EQUIPMENT)
                .FirstOrDefault(r =>
                    r.RentalID == id.Value &&
                    r.MemberID == memberId);

            if (rental == null)
            {
                return HttpNotFound();
            }

            if (rental.Status == "Returned")
            {
                TempData["ErrorMessage"] =
                    "This equipment has already been returned.";

                return RedirectToAction("Index");
            }

            rental.ReturnDate = DateTime.Now;
            rental.Status = "Returned";

            if (rental.EQUIPMENT != null)
            {
                rental.EQUIPMENT.QuantityAvailable =
                    rental.EQUIPMENT.QuantityAvailable + 1;
            }

            db.SaveChanges();

            TempData["SuccessMessage"] =
                "Equipment returned successfully.";

            return RedirectToAction("Index");
        }

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