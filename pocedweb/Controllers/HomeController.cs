using System.Web.Mvc;

namespace PocedWeb.Controllers
{
    public class HomeController : Controller
    {
        [Route("Home/About")]

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

    }
}