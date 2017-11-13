using System.Web.Mvc;
using PocedWeb.Models;

namespace PocedWeb.Controllers
{
    public class AccountController : Controller
    {
        [Route("Login")]
        public ActionResult Login()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (model.Username == model.Password)
            {
                // TODO -- log the user in

                return RedirectToAction("Index", "Articles");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        [Route("Logout")]
        public ActionResult Logout()
        {
            // TODO -- log the user out

            return View();
        }


        [Route("LoginExternal")]
        public ActionResult LoginExternal(string provider, string returnUrl)
        {
            // TODO -- send the user to the external provider

            return new HttpUnauthorizedResult();
        }
    }
}