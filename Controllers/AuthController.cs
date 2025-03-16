using System.Security.Claims;
using Lab2.ShoppingWeb.CartFeature.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.ShoppingWeb.CartFeature.Controllers
{
    public class AuthController : Controller
    {
        private readonly FirebaseAuthService _authService;

        public AuthController(FirebaseAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string idToken, string? returnUrl)
        {
            var user = await _authService.VerifyTokenAsync(idToken);
            if (user != null)
            {
                var exp = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Expiration)?.Value;
                long expirationTimeLong = 0;
                if (exp != null) {
                    long.TryParse(exp, out expirationTimeLong);
                }
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationTimeLong).UtcDateTime;
                // Check if token is expired
                if (expirationTime < DateTime.UtcNow)
                {
                    ViewBag.Error = "Session expired. Please log in again.";
                    return View();
                }

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = expirationTime // Set cookie expiration to match Firebase token
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user, authProperties);

                return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "Home") : Redirect(returnUrl);
            }
            else
            {
                // sign out since the token is invalid
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                ViewBag.Error = "Invalid login";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
