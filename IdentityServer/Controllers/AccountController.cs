using System.Threading.Tasks;
using IdentityServer.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        private const string LOGIN_TYPE_PARAM_NAME = "type";
        private const string INTERNAL_USER = "in";
        private const string EXTRNAL_USER = "ex";

        public AccountController(
            IIdentityServerInteractionService interaction,
            IOptions<AzureAdB2CConfig> b2cOptions)
        {
            _interaction = interaction;
            AzureAdB2CConfig = b2cOptions.Value;
        }

        public AzureAdB2CConfig AzureAdB2CConfig { get; set; }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            return context.Parameters[LOGIN_TYPE_PARAM_NAME] != INTERNAL_USER ? SignIn(returnUrl) : InternalSignIn(returnUrl);

            //return RedirectToAction("Index", "Home", new { returnUrl = returnUrl });
        }
        
        [HttpGet]
        public IActionResult LoginPage(string returnUrl = null)
        {
            return RedirectToAction("Index", "Home", new { returnUrl = returnUrl });
        }

        [HttpGet]
        public IActionResult SignIn(string returnUrl = null)
        {
            returnUrl = returnUrl ?? HttpContext.Request.Query["returnUrl"];
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            return Challenge(
                new AuthenticationProperties() { RedirectUri = redirectUrl }, "B2C");
        }
        
        [HttpGet]
        public IActionResult InternalSignIn(string returnUrl = null)
        {
            returnUrl = returnUrl ?? HttpContext.Request.Query["returnUrl"];
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            return Challenge(
                new AuthenticationProperties() { RedirectUri = redirectUrl }, "AD");
        }

        [HttpGet]
        public IActionResult ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, remoteError);
                return View("Error");
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            return await SignOut(logoutId);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut(string logoutId)
        {
            // sign out local identity server
            await HttpContext.SignOutAsync();

            // sign out AD or B2C
            var callbackUrl = Url.Action(nameof(SignedOut), "Account", new { logoutId = logoutId }, protocol: Request.Scheme);
            return SignOut(new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme, "AD", "B2C");
        }

        [HttpGet]
        public async Task<IActionResult> SignedOut(string logoutId)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            if (!string.IsNullOrWhiteSpace(logout?.PostLogoutRedirectUri))
            {
                return Redirect(logout?.PostLogoutRedirectUri);
            }

            return View();
        }

        //[HttpGet]
        //public IActionResult ResetPassword()
        //{
        //    var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
        //    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //    properties.Items[AzureAdB2CConfig.PolicyAuthenticationProperty] = AzureAdB2CConfig.ResetPasswordPolicyId;
        //    return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        //}

        //[HttpGet]
        //public IActionResult EditProfile()
        //{
        //    var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
        //    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //    properties.Items[AzureAdB2CConfig.PolicyAuthenticationProperty] = AzureAdB2CConfig.EditProfilePolicyId;
        //    return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        //}
    }
}