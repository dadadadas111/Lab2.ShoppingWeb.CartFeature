using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.ShoppingWeb.CartFeature.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
