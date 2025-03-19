using Microsoft.AspNetCore.SignalR;

namespace Lab2.ShoppingWeb.CartFeature.Hubs
{
    public class ProductHub : Hub
    {
        public async Task NotifyProductUpdated(int productId, string productName)
        {
            await Clients.All.SendAsync("ProductUpdated", productId, productName);
        }
    }
}
