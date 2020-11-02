using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace TorrServ.Controllers
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {            
             if (context.GetHttpContext().User.IsInRole("admin"))
         {
               
             return true; 
         }

            return false; 
        }
    }
}
