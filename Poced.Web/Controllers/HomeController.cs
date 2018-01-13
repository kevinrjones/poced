using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Poced.Web.Controllers
{
//    [Authorize]
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