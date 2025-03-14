using Lab2.ShoppingWeb.CartFeature.Services;

namespace Lab2.ShoppingWeb.CartFeature.Middleware
{
    public class FirebaseAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FirebaseAuthService _authService;

        public FirebaseAuthMiddleware(RequestDelegate next, FirebaseAuthService authService)
        {
            _next = next;
            _authService = authService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var principal = await _authService.VerifyTokenAsync(token);
                if (principal != null)
                {
                    context.User = principal;
                }
            }

            await _next(context);
        }
    }
}
