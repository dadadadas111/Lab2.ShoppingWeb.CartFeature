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
            [FromQuery] string categoryIds_string,
            [FromQuery] string supplierIds_string,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? onlyAvailable,
            [FromQuery] string sort,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 9)
        {

            List<Guid> categoryIds = categoryIds_string?.Split(',').Select(Guid.Parse).ToList();
            List<Guid> supplierIds = supplierIds_string?.Split(',').Select(Guid.Parse).ToList();

            var (products, totalPages) = await productRepository.GetWithFilterAsync(
                searchName, categoryIds, supplierIds, minPrice, maxPrice, onlyAvailable, page, pageSize, sort);

            var suppliers = await supplierRepository.GetAllAsync();
            var categories = await categoryRepository.GetAllAsync();

            // when return the list of suppliers and categories, sort the list by name, and the selected items should be on top
            suppliers = suppliers.OrderBy(s => s.Name).ToList();
            categories = categories.OrderBy(c => c.Name).ToList();

            string SelectedCategoryIds = "";
            string SelectedSupplierIds = "";

            // selected items should be on top
            if (categoryIds != null)
            {
                var selectedCategories = categories.Where(c => categoryIds.Contains(c.Id)).ToList();
                foreach (var category in selectedCategories)
                {
                    SelectedCategoryIds += category.Id + ",";
                }
                SelectedCategoryIds = SelectedCategoryIds.TrimEnd(',');
                categories = selectedCategories.Concat(categories.Except(selectedCategories)).ToList();
            }

            if (supplierIds != null)
            {
                var selectedSuppliers = suppliers.Where(s => supplierIds.Contains(s.Id)).ToList();
                foreach (var supplier in selectedSuppliers)
                {
                    SelectedSupplierIds += supplier.Id + ",";
                }
                SelectedSupplierIds = SelectedSupplierIds.TrimEnd(',');
                suppliers = selectedSuppliers.Concat(suppliers.Except(selectedSuppliers)).ToList();
            }

            var viewModel = new ProductViewModel
            {
                Products = products,
                Suppliers = suppliers,
                Categories = categories,
                TotalPages = totalPages,
                CurrentPage = page,
                SearchName = searchName,
                MaxPrice = maxPrice,
                MinPrice = minPrice,
                SelectedCategoryIds = SelectedCategoryIds,
                SelectedSupplierIds = SelectedSupplierIds,
                SortOption = sort,
            };

            return View(viewModel);
        }

    }
}
