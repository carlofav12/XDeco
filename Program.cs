using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using XDeco.Models;
using XDeco.Service;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Client.CardToken;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Customer;

var builder = WebApplication.CreateBuilder(args);

//mecado pago servicios
builder.Services.AddScoped<CardTokenClient>();
builder.Services.AddScoped<CustomerClient>();
builder.Services.AddScoped<PaymentClient>();
builder.Services.AddScoped<IMercadoPagoService, MercadoPagoService>();
// Configurar HttpClient para MercadoPagoService
builder.Services.AddHttpClient<IMercadoPagoService, MercadoPagoService>((serviceProvider, httpClient) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var accessToken = configuration["MercadoPagoConfig:AccessToken"];

    httpClient.BaseAddress = new Uri("https://api.mercadopago.com/v1/");
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    httpClient.DefaultRequestHeaders.Add("User-Agent", "MercadoPagoApp");
});

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
    if (context.Request.Path.StartsWithSegments("/Admin") && !context.User.Identity.IsAuthenticated)
    {
        // Redirige a la página de acceso denegado
        context.Response.Redirect("/Home/AccessDenied");
        return; // Termina la ejecución
    }

    await next(); // Continúa con el siguiente middleware
});


// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
