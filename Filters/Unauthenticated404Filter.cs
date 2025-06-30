using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

public class Unauthenticated404Filter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();

        if (endpoint != null && endpoint.Metadata.Any(m => m is IAllowAnonymous))
        {
            return;
        }

        var user = context.HttpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new NotFoundResult();
            return;
        }
    }
}
