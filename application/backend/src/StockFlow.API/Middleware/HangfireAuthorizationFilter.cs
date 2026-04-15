using Hangfire.Dashboard;

namespace StockFlow.API.Middleware;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow access in development
        if (httpContext.Request.Host.Host.Contains("localhost"))
        {
            return true;
        }

        // In production, check if user is authenticated and has Admin role
        return httpContext.User.Identity?.IsAuthenticated == true &&
               httpContext.User.IsInRole("Admin");
    }
}
