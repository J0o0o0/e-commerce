using System.Security.Claims;

namespace e_commerce.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("User ID not found in token");

            return int.Parse(userIdClaim);
        }
    }
}