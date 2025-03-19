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
                bool isAdmin = false;
                var email = validPayload.Claims["email"].ToString();
                if (email != null && email.Contains("@fpt.edu.vn"))
                {
                    isAdmin = true;
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, validPayload.Uid),
                    new Claim(ClaimTypes.Email, validPayload.Claims["email"].ToString()),
                    new Claim(ClaimTypes.Expiration, validPayload.ExpirationTimeSeconds.ToString()),
                    new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
                };

                var identity = new ClaimsIdentity(claims, "firebase");
                return new ClaimsPrincipal(identity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null; // Invalid token
            }
        }
    }
}
