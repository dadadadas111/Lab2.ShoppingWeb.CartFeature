using Lab2.ShoppingWeb.CartFeature.Models;

namespace Lab2.ShoppingWeb.CartFeature.Data.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
    }
}
