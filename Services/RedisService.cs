using StackExchange.Redis;
using System.Text.Json;

namespace Lab2.ShoppingWeb.CartFeature.Services
{
    public class RedisService
    {
        private readonly IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<List<CartItem>> GetCartAsync(string userId)
        {
            try
            {
                string cartJson = await _db.StringGetAsync($"cart:{userId}");
                return cartJson != null ? JsonSerializer.Deserialize<List<CartItem>>(cartJson) : new List<CartItem>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<CartItem>();
            }
        }
        public async Task SaveCartAsync(string userId, List<CartItem> cart)
        {
            try
            {
                string cartJson = JsonSerializer.Serialize(cart);
                await _db.StringSetAsync($"cart:{userId}", cartJson);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CartItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
