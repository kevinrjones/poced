using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
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
                var ci = new ClaimsIdentity("Cookie");
                ci.AddClaim(new Claim(ClaimTypes.Name, model.Username));

                var ctx = Request.GetOwinContext();
                ctx.Authentication.SignIn(ci);

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Articles");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        [Route("Logout")]
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            ctx.Authentication.SignOut();

            return View();
        }


        [Route("LoginExternal")]
        public ActionResult LoginExternal(string provider, string returnUrl)
        {
            var ctx = Request.GetOwinContext();
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };
            ctx.Authentication.Challenge(props, provider);
            return new HttpUnauthorizedResult();
        }

    }
}