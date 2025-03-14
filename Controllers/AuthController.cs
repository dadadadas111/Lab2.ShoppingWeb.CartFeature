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
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            ViewBag.Error = "Invalid login";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
