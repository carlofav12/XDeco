using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AdminAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Permitir acceso a "/Admin/Index" sin autenticación
        if (context.HttpContext.Request.Path.Equals("/Admin/Index", StringComparison.OrdinalIgnoreCase))
        {
            return; // Permitir acceso
        }

        // Verifica si el usuario está autenticado y tiene el rol de Admin
        if (!context.HttpContext.User.Identity.IsAuthenticated || 
            !context.HttpContext.User.IsInRole("Admin"))
        {
            // Redirige a la página de acceso denegado
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }
    }
}


