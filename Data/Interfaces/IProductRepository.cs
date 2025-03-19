using Lab2.ShoppingWeb.CartFeature.Models;

namespace Lab2.ShoppingWeb.CartFeature.Data.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<(IEnumerable<Product> Products, int TotalPages)> GetWithFilterAsync(
           string searchName = null,
           List<Guid> categoryIds = null,
           List<Guid> supplierIds = null,
           decimal? minPrice = null,
           decimal? maxPrice = null,
           bool? onlyAvailable = null,
           int pageNumber = 1,
           int pageSize = 10,
           string sortOption = "name_asc");
    }
}
