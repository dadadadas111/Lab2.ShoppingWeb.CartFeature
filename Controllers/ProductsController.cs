using Lab2.ShoppingWeb.CartFeature.Data;
using Lab2.ShoppingWeb.CartFeature.Data.Interfaces;
using Lab2.ShoppingWeb.CartFeature.Data.Repositories;
using Lab2.ShoppingWeb.CartFeature.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab2.ShoppingWeb.CartFeature.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly ICategoryRepository categoryRepository;

        public ProductsController(ApplicationDbContext context)
        {
            productRepository = new ProductRepository(context);
            supplierRepository = new SupplierRepository(context);
            categoryRepository = new CategoryRepository(context);
        }

        public async Task<IActionResult> Index(
            [FromQuery] string searchName,
            [FromQuery] List<Guid> categoryIds,
            [FromQuery] List<Guid> supplierIds,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? onlyAvailable,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 9)
        {
            var (products, totalPages) = await productRepository.GetWithFilterAsync(
                searchName, categoryIds, supplierIds, minPrice, maxPrice, onlyAvailable, pageNumber, pageSize);

            var suppliers = await supplierRepository.GetAllAsync();
            var categories = await categoryRepository.GetAllAsync();

            var viewModel = new ProductViewModel
            {
                Products = products,
                Suppliers = suppliers,
                Categories = categories,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                SearchName = searchName,
                MaxPrice = maxPrice,
                MinPrice = minPrice,
                SelectedCategoryIds = categoryIds != null ? string.Join(",", categoryIds) : null,
                SelectedSupplierIds = supplierIds != null ? string.Join(",", supplierIds) : null
            };

            return View(viewModel);
        }

    }
}
