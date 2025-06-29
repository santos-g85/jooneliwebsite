using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class Unauthenticated404Filter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true ||
            context.Filters.Any(item => item is IAllowAnonymousFilter))
        {
            return;
        }

        context.Result = new RedirectToRouteResult(
            new RouteValueDictionary
            {
                { "controller", "Error" },
                { "action", "Error404" }
            });
    }
}
