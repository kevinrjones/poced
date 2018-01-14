using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Poced.Logging;
using Poced.Logging.Web;
using Poced.Services.Intrfaces;
using Poced.Web.Models;

namespace Poced.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPocedWebLogger _logger;

        public AccountController(IUserService userService, IPocedWebLogger logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            var result = await _userService.PasswordSignInAsync(model.Email, model.Password, model.RememberMe);
            if (result.Succeeded)
            {
                // todo: 
                _logger.WriteDiagnostic(HttpContext, "Poced", "Web", "Successful Login");
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.WriteDiagnostic(HttpContext, "Poced", "Web", "User account locked out.", null, LogLevel.Warning);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }


        public ActionResult Logout()
        {
            //var ctx = Request.GetOwinContext();
            //ctx.Authentication.SignOut();

            return View();
        }



        public ActionResult LoginExternal(string provider, string returnUrl)
        {
            //var ctx = Request.GetOwinContext();
            //var props = new AuthenticationProperties
            //{
            //    RedirectUri = "/ExternalCallback"
            //};
            //ctx.Authentication.Challenge(props, provider);
            return new UnauthorizedResult();
        }

        [HttpPost]
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


        public async Task<ActionResult> ExternalCallback()
        {
            //var ctx = Request.GetOwinContext();
            //var external = await ctx.Authentication.AuthenticateAsync("ExternalCookie");
            //if (external == null) return RedirectToAction("Login");

            //var idClaim = external.Identity.FindFirst(ClaimTypes.NameIdentifier);
            //var provider = idClaim.Issuer;
            //var providerId = idClaim.Value;

            //var claimsIdentity = _userService.CreateUserIdentity(provider, providerId);
            //if (claimsIdentity != null)
            //{
            //    ctx.Authentication.SignIn(claimsIdentity);
            //    ctx.Authentication.SignOut("ExternalCookie");
            //    return RedirectToAction("Index", "Articles");
            //}
            //else
            //{
            return View("RegisterExternal");
            //}
        }

        [HttpPost]
        public async Task<ActionResult> RegisterExternal(RegisterExternalModel model)
        {
            //var ctx = Request.GetOwinContext();
            //var external = await ctx.Authentication.AuthenticateAsync("ExternalCookie");
            //if (external == null) return RedirectToAction("Login");

            //var idClaim = external.Identity.FindFirst(ClaimTypes.NameIdentifier);
            //var provider = idClaim.Issuer;
            //var providerId = idClaim.Value;

            //if (ModelState.IsValid)
            //{
            //    var user = _userService.CreateAndLoginUser(model.Email, provider, providerId, external.Identity.Claims);
            //    if (user != null)
            //    {
            //        ctx.Authentication.SignOut("ExternalCookie");

            //        var ci = _userService.CreateIdentity(user, "Cookie");
            //        ctx.Authentication.SignIn(ci);

            //        return RedirectToAction("RegisterSuccess");
            //    }

            //    ModelState.AddModelError("", "Unable to register user");
            //}

            return View();
        }


        public ActionResult RegisterSuccess()
        {
            return View();
        }

        public ActionResult Profile()
        {
            var user = _userService.FindByName(User.Identity.Name);
            IList<Claim> claims = _userService.GetClaims(user.Id);
            var vm = new ProfileModel(claims);
            return View("Profile", vm);
        }

        [HttpPost]
        public ActionResult UpdateProfile(ProfileModel model)
        {
            //var user = _userService.FindByName(User.Identity.Name);
            //var claims = _userService.GetClaims(user.Id);

            //var givenName = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
            //if (givenName != null)
            //{
            //    bool result = _userService.RemoveClaim(user.Id, givenName);
            //    if (!result) ModelState.AddModelError("", $"Unable to remove GivenName claim");
            //}

            //var surname = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
            //if (surname != null)
            //{
            //    var result = _userService.RemoveClaim(user.Id, surname);
            //    if (!result) ModelState.AddModelError("", $"Unable to remove Surame claim");
            //}

            //if (!String.IsNullOrWhiteSpace(model.First))
            //{
            //    bool result = _userService.AddClaim(user.Id, new Claim(ClaimTypes.GivenName, model.First));
            //    if (!result) ModelState.AddModelError("", $"Unable to add GivenName claim");
            //}
            //if (!String.IsNullOrWhiteSpace(model.Last))
            //{
            //    var result = _userService.AddClaim(user.Id, new Claim(ClaimTypes.Surname, model.Last));
            //    if (!result) ModelState.AddModelError("", $"Unable to add GivenName claim");
            //}

            //var ci = _userService.CreateIdentity(user, "Cookie");
            //var ctx = Request.GetOwinContext();
            //ctx.Authentication.SignIn(ci);

            //if (ModelState.IsValid) ViewData["Success"] = true;
            return Profile();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(ArticlesController.Index), "Articles");
        }

    }

}
