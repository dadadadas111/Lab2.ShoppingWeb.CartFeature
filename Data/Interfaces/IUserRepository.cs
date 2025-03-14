using Lab2.ShoppingWeb.CartFeature.Models;

namespace Lab2.ShoppingWeb.CartFeature.Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
