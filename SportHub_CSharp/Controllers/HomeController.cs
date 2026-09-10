using System.Web.Mvc;

namespace SportHub_CSharp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "SportHub - Sports Facility and Equipment Booking System";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact SportHub";

            return View();
        }
    }
}