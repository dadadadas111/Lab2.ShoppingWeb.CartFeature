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

        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet("get")]
        //public IActionResult GetCart()
        //{
        //    Console.WriteLine("Getting cart for user: " + GetUserCartKey());
        //    var cartJson = _redisDb.StringGet(GetUserCartKey());
        //    Console.WriteLine("Cart JSON: " + cartJson);
        //    // If cart is empty, return an empty array
        //    if (cartJson.IsNullOrEmpty)
        //    {
        //        return Ok(new CartItem[0]);
        //    }
        //    return Ok(JsonConvert.DeserializeObject<CartItem[]>(cartJson));
        //}
        [HttpGet("get")]
        public IActionResult GetCart()
        {
            var userCartKey = GetUserCartKey();
            var cartEntries = _redisDb.HashGetAll(userCartKey);

            if (cartEntries.Length == 0)
            {
                return Ok(new CartItem[0]);
            }

            var cartItems = cartEntries.Select(entry => JsonConvert.DeserializeObject<CartItem>(entry.Value)).ToArray();
            return Ok(cartItems);
        }


        //[HttpPost("update")]
        //public IActionResult UpdateCart([FromBody] List<CartItem> cart)
        //{
        //    _redisDb.StringSet(GetUserCartKey(), JsonConvert.SerializeObject(cart));
        //    return Ok();
        //}
        [HttpPost("update")]
        public IActionResult UpdateCart([FromBody] List<CartItem> cart)
        {
            ClearCart();
            var userCartKey = GetUserCartKey();
            var batch = _redisDb.CreateBatch();

            foreach (var item in cart)
            {
                batch.HashSetAsync(userCartKey, item.id, JsonConvert.SerializeObject(item));
            }
            batch.Execute();
            return Ok();
        }


        //[HttpPost("sync")]
        //public IActionResult SyncCart([FromBody] List<CartItem> cart)
        //{
        //    // get the current cart stored in Redis, if any not in the request, add it to the request
        //    var currentCartJson = _redisDb.StringGet(GetUserCartKey());
        //    if (!currentCartJson.IsNullOrEmpty)
        //    {
        //        var currentCart = JsonConvert.DeserializeObject<CartItem[]>(currentCartJson);
        //        foreach (var item in currentCart)
        //        {
        //            if (!cart.Any(i => i.id == item.id))
        //            {
        //                cart.Add(item);
        //            }
        //        }
        //    }
        //    _redisDb.StringSet(GetUserCartKey(), JsonConvert.SerializeObject(cart));
        //    return Ok();
        //}
        [HttpPost("sync")]
        public IActionResult SyncCart([FromBody] List<CartItem> cart)
        {
            var userCartKey = GetUserCartKey();
            var existingCartEntries = _redisDb.HashGetAll(userCartKey);

            var existingCart = existingCartEntries
                .Select(entry => JsonConvert.DeserializeObject<CartItem>(entry.Value))
                .ToDictionary(item => item.id, item => item);

            foreach (var item in cart)
            {
                existingCart[item.id] = item;  // Overwrite or add new item
            }

            var batch = _redisDb.CreateBatch();
            foreach (var item in existingCart.Values)
            {
                batch.HashSetAsync(userCartKey, item.id, JsonConvert.SerializeObject(item));
            }
            batch.Execute();

            return Ok();
        }

        private void ClearCart()
        {
            _redisDb.KeyDelete(GetUserCartKey());
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
