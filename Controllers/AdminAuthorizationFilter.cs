using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AdminAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Permitir acceso a "/Admin/Index" y "/Admin/Login" sin autenticación
        var path = context.HttpContext.Request.Path;
        if (path.Equals("/Admin/Index", StringComparison.OrdinalIgnoreCase) ||
            path.Equals("/Admin/Login", StringComparison.OrdinalIgnoreCase))
        {
            return; // Permitir acceso
        }

        // Verifica si el usuario está autenticado como administrador
        var isAdminAuthenticated = context.HttpContext.Session.GetString("IsAdminAuthenticated");
        if (string.IsNullOrEmpty(isAdminAuthenticated) || isAdminAuthenticated != "true")
        {
            // Redirige a la página de acceso denegado
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }

    }
}



