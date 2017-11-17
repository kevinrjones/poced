using System;
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

            var claimsIdentity = _userService.GetUserClaims(provider, providerId);
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
                    
                    var ci =_userService.CreateIdentity(user, "Cookie");
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

        //public ActionResult Profile()
        //{
        //    var user = userManager.FindByName(User.Identity.Name);
        //    var claims = userManager.GetClaims(user.Id);
        //    var vm = new ProfileModel(claims);
        //    return View("Profile", vm);
        //}

        //[HttpPost]
        //public ActionResult UpdateProfile(ProfileModel model)
        //{
        //    var user = userManager.FindByName(User.Identity.Name);
        //    var claims = userManager.GetClaims(user.Id);

        //    var givenName = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
        //    if (givenName != null)
        //    {
        //        var result = userManager.RemoveClaim(user.Id, givenName);
        //        if (!result.Succeeded) ModelState.AddModelError("", result.Errors.First());
        //    }

        //    var surname = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
        //    if (surname != null)
        //    {
        //        var result = userManager.RemoveClaim(user.Id, surname);
        //        if (!result.Succeeded) ModelState.AddModelError("", result.Errors.First());
        //    }

        //    if (!String.IsNullOrWhiteSpace(model.First))
        //    {
        //        var result = userManager.AddClaim(user.Id, new Claim(ClaimTypes.GivenName, model.First));
        //        if (!result.Succeeded) ModelState.AddModelError("", result.Errors.First());
        //    }
        //    if (!String.IsNullOrWhiteSpace(model.Last))
        //    {
        //        var result = userManager.AddClaim(user.Id, new Claim(ClaimTypes.Surname, model.Last));
        //        if (!result.Succeeded) ModelState.AddModelError("", result.Errors.First());
        //    }

        //    var ci = userManager.CreateIdentity(user, "Cookie");
        //    var ctx = Request.GetOwinContext();
        //    ctx.Authentication.SignIn(ci);

        //    if (ModelState.IsValid) ViewData["Success"] = true;
        //    return Profile();
        //}
    }

}
