namespace MyEntityMembershipService.Extensions
{

    public static class HttpContextExtensions
    {
        public static string? GetAuthorizationToken(this HttpContext httpContext)
        {
            if (httpContext?.Request?.Headers?.TryGetValue("Authorization", out var authHeader) == true)
            {
                // Extract the token part after "Bearer "
                return authHeader.ToString().Split(' ').Last();
            }
            return null;
        }
    }
}
