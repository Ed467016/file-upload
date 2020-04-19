using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProtectedFiles.Web.Infrastructure.Filters
{
    /// <summary>
    /// IMPORTANT. Never use this as your main authorization filter.
    /// This method is designed only for test purposes and never for production.
    /// Use <see cref="ProtectedFiles.Web.Infrastructure.Authorization.Handlers.SessionAuthorizationHandler"/>
    /// which is configured in start up.
    /// </summary>
    public class SessionAdminAuthorizationFilterAttribute : TypeFilterAttribute
    {
        public SessionAdminAuthorizationFilterAttribute(int role) 
            : base(typeof(SessionAdminAuthorizationRequirement))
        {
            Arguments = new object[] 
            { 
                role,
            };
        }
    }

    public class SessionAdminAuthorizationRequirement : IAuthorizationFilter
    {
        private readonly int _role;

        public SessionAdminAuthorizationRequirement(int role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var sessionRole = context.HttpContext.Session.GetInt32("LoggedInRole");

            if (!sessionRole.HasValue || sessionRole != _role)
            {
                var viewResult = new ViewResult();
                viewResult.ViewName = "/Views/Shared/AccessDenied.cshtml";
                context.Result = viewResult;
            }
        }
    }
}
