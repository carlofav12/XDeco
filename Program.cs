using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using XDeco.Integration.nytimes;
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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Usuario>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()  // Añadir soporte para roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Registro de servicios personalizados
builder.Services.AddSingleton<EmailService, EmailService>();
builder.Services.AddScoped<NYTimesApiIntegration>();

// Configurar controladores, vistas y API Explorer
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger para la documentación de API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "XDeco Api", Version = "v1" });
});

// Registrar HttpClient
builder.Services.AddHttpClient();
builder.Services.AddScoped<AdminAuthorizationFilter>(); // Registra el filtro

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "XDeco Api V1");
    });
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
