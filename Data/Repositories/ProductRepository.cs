﻿using Lab2.ShoppingWeb.CartFeature.Data.Interfaces;
using Lab2.ShoppingWeb.CartFeature.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.ShoppingWeb.CartFeature.Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<(IEnumerable<Product> Products, int TotalPages)> GetWithFilterAsync(
            string searchName = null,
            List<Guid> categoryIds = null,
            List<Guid> supplierIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? onlyAvailable = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Products
                .Include(p => p.Supplier)
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                query = query.Where(p => p.Name.Contains(searchName));

            if (categoryIds != null && categoryIds.Any())
                query = query.Where(p => categoryIds.Contains(p.CategoryId));

            if (supplierIds != null && supplierIds.Any())
                query = query.Where(p => supplierIds.Contains(p.SupplierId));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (onlyAvailable.HasValue && onlyAvailable.Value)
                query = query.Where(p => p.StockQuantity > 0);

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalPages);
        }
    }
}
