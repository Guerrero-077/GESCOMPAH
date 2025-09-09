using Hangfire.Dashboard;

namespace WebGESCOMPAH.Security
{
    public sealed class HangfireDashboardAuth : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var http = context.GetHttpContext();

            return http.User.Identity?.IsAuthenticated == true &&
                   (http.User.IsInRole("Administrador") || http.User.IsInRole("Arrendador"));
        }
    }
}
