using Lab2.ShoppingWeb.CartFeature.Models;

namespace Lab2.ShoppingWeb.CartFeature.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? SearchName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SelectedCategoryIds { get; set; }
        public string? SelectedSupplierIds { get; set; }
    }
}
