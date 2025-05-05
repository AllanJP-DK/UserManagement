// Services/CurrentUserService.cs
namespace UserManagement.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCurrentUserId()
        {
            // For development without authentication, return a hardcoded ID
            return Guid.Parse("01969bb5-90e0-755b-b6cb-7cfa4db50ad3"); // Admin user ID
        }

        public string GetCurrentUsername()
        {
            // For development without authentication, return a hardcoded username
            return "admin"; // Admin username

            // When using authentication, you would use this:
            /*
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            return httpContext.User.Identity?.Name;
            */
        }
    }
}