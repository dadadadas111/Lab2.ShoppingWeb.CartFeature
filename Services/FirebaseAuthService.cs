using System.Security.Claims;

namespace Lab2.ShoppingWeb.CartFeature.Services
{
    public class FirebaseAuthService
    {
        public async Task<ClaimsPrincipal?> VerifyTokenAsync(string token)
        {
            try
            {
                var validPayload = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, validPayload.Uid),
                    new Claim(ClaimTypes.Email, validPayload.Claims["email"].ToString()),
                    new Claim(ClaimTypes.Expiration, validPayload.ExpirationTimeSeconds.ToString())
                };

                var identity = new ClaimsIdentity(claims, "firebase");
                return new ClaimsPrincipal(identity);
            }
            catch
            {
                return null; // Invalid token
            }
        }
    }
}
