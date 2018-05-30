using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Poced.Identity.Shared;
using Poced.Logging;
using Poced.Logging.Web;
using Poced.Services.Intrfaces;
using Poced.Web.Models;
using Poced.Web.Models.Account;

namespace Poced.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPocedWebLogger _logger;

        [TempData]
        public string ErrorMessage { get; set; }

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userService.SignOutAsync();
            _logger.WriteDiagnostic(HttpContext, "Poced", "Web", "User logged out.");
            return RedirectToAction(nameof(ArticlesController.Index), "Articles");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new PocedUser { UserName = model.Email, Email = model.Email };
                var result = await _userService.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.WriteDiagnostic(HttpContext, "Poced", "Web", "User created a new account with password.");

                    await _userService.SignInAsync(user, isPersistent: false);
                    _logger.WriteDiagnostic(HttpContext, "Poced", "Web", "User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _userService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _userService.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _userService.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.WriteDiagnostic(HttpContext, "Poced", "Web", $"User logged in with {info.LoginProvider} provider.");
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _userService.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new PocedUser { UserName = model.Email, Email = model.Email };
                var result = await _userService.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userService.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _userService.SignInAsync(user, isPersistent: false);
                        _logger.WriteDiagnostic(HttpContext, "Poced", "Web", $"User created an account using {info.LoginProvider} provider.");
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
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

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(ArticlesController.Index), "Articles");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


    }

}
