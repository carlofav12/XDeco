using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using XDeco.Models;
using XDeco.Service;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("PosgreConnection")
    ?? throw new InvalidOperationException("Connection string 'PosgreConnection' not found.");

// Configuración del DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configurar Identity con roles
builder.Services.AddDefaultIdentity<Usuario>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()  // Añadir soporte para roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSingleton<EmailService, EmailService>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AdminAuthorizationFilter>(); // Registra el filtro

var app = builder.Build();

// Configuración del pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Asegúrate de agregar autenticación antes de la autorización
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    // Verifica si la ruta comienza con "/Admin/"
    if (context.Request.Path.StartsWithSegments("/Admin"))
    {
        // Permitir acceso a "/Admin/Index" (la vista de inicio de sesión)
        if (context.Request.Path.Equals("/Admin/Index", StringComparison.OrdinalIgnoreCase))
        {
            await next(); // Permite el acceso
            return;
        }

        // Verifica si el usuario está autenticado
        if (!context.User.Identity.IsAuthenticated)
        {
            // Redirige a la página de acceso denegado o login
            context.Response.Redirect("/Admin/Index"); // Redirigir al login
            return;
        }
    }

    await next(); // Continúa con el siguiente middleware
});



// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
