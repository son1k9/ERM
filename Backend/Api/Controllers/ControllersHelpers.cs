using Models;

namespace Api.Controllers;

public static class Helpers
{
    public static User GetAuthorizedUser(HttpContext httpContext)
    {
        if (httpContext.Items["User"] is User user)
        {
            return user;
        }
        throw new InvalidOperationException("No authorized user");
    }
}