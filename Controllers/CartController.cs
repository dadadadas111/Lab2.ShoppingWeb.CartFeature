using Lab2.ShoppingWeb.CartFeature.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Security.Claims;

namespace Lab2.ShoppingWeb.CartFeature.Controllers
{
    [Route("Cart")]
    public class CartController : Controller
    {
        private readonly IDatabase _redisDb;

        public CartController(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        private string GetUserCartKey() => $"cart:{User.FindFirst(ClaimTypes.Email)?.Value}";

        // GET: /Cart/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("get")]
        public IActionResult GetCart()
        {
            Console.WriteLine("Getting cart for user: " + GetUserCartKey());
            var cartJson = _redisDb.StringGet(GetUserCartKey());
            Console.WriteLine("Cart JSON: " + cartJson);
            // If cart is empty, return an empty array
            if (cartJson.IsNullOrEmpty)
            {
                return Ok(new CartItem[0]);
            }
            return Ok(JsonConvert.DeserializeObject<CartItem[]>(cartJson));
        }

        [HttpPost("update")]
        public IActionResult UpdateCart([FromBody] List<CartItem> cart)
        {
            _redisDb.StringSet(GetUserCartKey(), JsonConvert.SerializeObject(cart));
            return Ok();
        }

    }

    public class CartItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string imageUrl { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }
}
