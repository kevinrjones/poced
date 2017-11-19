using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using PocedServices.Intrfaces;
using PocedWeb.Models;

namespace PocedWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
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
            var props = new AuthenticationProperties
            {
                RedirectUri = "/ExternalCallback"
            };
            ctx.Authentication.Challenge(props, provider);
            return new HttpUnauthorizedResult();
        }

        [HttpPost]
        [Route("Account/Register")]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _userService.CreateUser(model.Username, model.Password);

                if (result != null)
                {
                    return View("RegisterSuccess");
                }

                ModelState.AddModelError("", "Unable to register user");
            }

            return View();
        }

        [Route("ExternalCallback")]
        public async Task<ActionResult> ExternalCallback()
        {
            var ctx = Request.GetOwinContext();
            var external = await ctx.Authentication.AuthenticateAsync("ExternalCookie");
            if (external == null) return RedirectToAction("Login");

            var idClaim = external.Identity.FindFirst(ClaimTypes.NameIdentifier);
            var provider = idClaim.Issuer;
            var providerId = idClaim.Value;

            var claimsIdentity = _userService.CreateUserIdentity(provider, providerId);
            if (claimsIdentity != null)
            {
                ctx.Authentication.SignIn(claimsIdentity);
                ctx.Authentication.SignOut("ExternalCookie");
                return RedirectToAction("Index", "Articles");
            }
            else
            {
                return View("RegisterExternal");
            }
        }

        [HttpPost]
        [Route("Account/RegisterExternal")]
        public async Task<ActionResult> RegisterExternal(RegisterExternalModel model)
        {
            var ctx = Request.GetOwinContext();
            var external = await ctx.Authentication.AuthenticateAsync("ExternalCookie");
            if (external == null) return RedirectToAction("Login");

            var idClaim = external.Identity.FindFirst(ClaimTypes.NameIdentifier);
            var provider = idClaim.Issuer;
            var providerId = idClaim.Value;

            if (ModelState.IsValid)
            {
                var user = _userService.CreateAndLoginUser(model.Username, provider, providerId, external.Identity.Claims);
                if (user != null)
                {
                    ctx.Authentication.SignOut("ExternalCookie");

                    var ci = _userService.CreateIdentity(user, "Cookie");
                    ctx.Authentication.SignIn(ci);

                    return RedirectToAction("RegisterSuccess");
                }

                ModelState.AddModelError("", "Unable to register user");
            }

            return View();
        }


        [Route("Account/RegisterSuccess")]
        public ActionResult RegisterSuccess()
        {
            return View();
        }

        [Route("Account/Profile")]
        public ActionResult Profile()
        {
            var user = _userService.FindByName(User.Identity.Name);
            IList<Claim> claims = _userService.GetClaims(user.Id);
            var vm = new ProfileModel(claims);
            return View("Profile", vm);
        }

        [HttpPost]
        [Route("Account/UpdateProfile")]
        public ActionResult UpdateProfile(ProfileModel model)
        {
            var user = _userService.FindByName(User.Identity.Name);
            var claims = _userService.GetClaims(user.Id);

            var givenName = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
            if (givenName != null)
            {
                bool result = _userService.RemoveClaim(user.Id, givenName);
                if (!result) ModelState.AddModelError("", $"Unable to remove GivenName claim");
            }

            var surname = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
            if (surname != null)
            {
                var result = _userService.RemoveClaim(user.Id, surname);
                if (!result) ModelState.AddModelError("", $"Unable to remove Surame claim");
            }

            if (!String.IsNullOrWhiteSpace(model.First))
            {
                bool result = _userService.AddClaim(user.Id, new Claim(ClaimTypes.GivenName, model.First));
                if (!result) ModelState.AddModelError("", $"Unable to add GivenName claim");
            }
            if (!String.IsNullOrWhiteSpace(model.Last))
            {
                var result = _userService.AddClaim(user.Id, new Claim(ClaimTypes.Surname, model.Last));
                if (!result) ModelState.AddModelError("", $"Unable to add GivenName claim");
            }

            var ci = _userService.CreateIdentity(user, "Cookie");
            var ctx = Request.GetOwinContext();
            ctx.Authentication.SignIn(ci);

            if (ModelState.IsValid) ViewData["Success"] = true;
            return Profile();
        }
    }

}
