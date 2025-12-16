using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Bookify.Web.Filters
{
    public class HandfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private string _policyName;

        public HandfireAuthorizationFilter(string policyName)
        {
            _policyName = policyName;
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var authService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();

            // AuthorizeAsync is asynchronous, but the DashboardAuthorizationFilter interface requires a synchronous method.
            // then You Cant use "await authService.AuthorizeAsync(httpContext.User, _policyName);" and Convert Method to Async
            // but You Can Use " .ConfigureAwait(false).GetAwaiter().GetResult().Succeeded" to Wait for the Result Synchronously
            var isAuthorized = authService.AuthorizeAsync(httpContext.User, _policyName)
                                          .ConfigureAwait(false)
                                          .GetAwaiter()
                                          .GetResult().Succeeded;
            return isAuthorized;
        }
    }
}
