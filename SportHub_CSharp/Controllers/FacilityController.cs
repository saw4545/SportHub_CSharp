using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SportHub_CSharp.Models;

namespace SportHub_CSharp.Controllers
{
    public class FacilityController : Controller
    {
        private SportHubEntities1 db = new SportHubEntities1();

        // GET: Facility
        // GET: Facility
        public ActionResult Index(string search)
        {
            var facilities = db.FACILITies.AsQueryable();

            // Facility search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                facilities = facilities.Where(f =>
                    f.Name.Contains(search) ||
                    f.Type.Contains(search) ||
                    f.Location.Contains(search)
                );
            }

            var result = facilities
                .OrderBy(f => f.Name)
                .ToList();

            ViewBag.Search = search;

            return View(result);
        }

        // GET: Facility/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            var facility = db.FACILITies
                .FirstOrDefault(f => f.FacilityID == id.Value);

            if (facility == null)
            {
                return HttpNotFound();
            }

            return View(facility);
        }

        // GET: Facility/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Facility/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FACILITY facility)
        {
            if (ModelState.IsValid)
            {
                db.FACILITies.Add(facility);
                db.SaveChanges();

                TempData["SuccessMessage"] =
                    "Facility created successfully.";

                return RedirectToAction("Index");
            }

            return View(facility);
        }

        // GET: Facility/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            var facility = db.FACILITies.Find(id.Value);

            if (facility == null)
            {
                return HttpNotFound();
            }

            return View(facility);
        }

        // POST: Facility/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FACILITY facility)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facility).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] =
                    "Facility updated successfully.";

                return RedirectToAction("Index");
            }

            return View(facility);
        }

        // GET: Facility/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            var facility = db.FACILITies.Find(id.Value);

            if (facility == null)
            {
                return HttpNotFound();
            }

            return View(facility);
        }

        // POST: Facility/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var facility = db.FACILITies.Find(id);

            if (facility == null)
            {
                return HttpNotFound();
            }

            db.FACILITies.Remove(facility);
            db.SaveChanges();

            TempData["SuccessMessage"] =
                "Facility deleted successfully.";

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