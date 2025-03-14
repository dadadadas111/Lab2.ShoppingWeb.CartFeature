using Lab2.ShoppingWeb.CartFeature.Data;
using Lab2.ShoppingWeb.CartFeature.Data.Interfaces;
using Lab2.ShoppingWeb.CartFeature.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.ShoppingWeb.CartFeature.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;

        public ProductsController(ApplicationDbContext context)
        {
            productRepository = new ProductRepository(context);
        }

        public async Task<IActionResult> Index()
        {
            var products = await productRepository.GetAllAsync();
            return View(products);
        }
    }
}
