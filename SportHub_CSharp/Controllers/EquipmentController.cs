using System.Linq;
using System.Web.Mvc;
using SportHub_CSharp.Models;

namespace SportHub_CSharp.Controllers
{
    public class EquipmentController : Controller
    {
        private SportHubEntities1 db = new SportHubEntities1();

        // =========================================================
        // GET: Equipment
        // Display all equipment
        // =========================================================
        public ActionResult Index()
        {
            var equipmentList = db.EQUIPMENTs
                .OrderBy(e => e.Name)
                .ToList();

            return View(equipmentList);
        }

        // =========================================================
        // Dispose Database Connection
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