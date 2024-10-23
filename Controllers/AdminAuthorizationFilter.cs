using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AdminAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated || 
            !context.HttpContext.User.IsInRole("Admin"))
        {
            // Redirige a la página de acceso denegado
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }
    }
}
