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
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar el middleware de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Duración de la sesión
    options.Cookie.HttpOnly = true; // Solo accesible por el servidor
    options.Cookie.IsEssential = true; // Necesario para la aplicación
});

// Configurar servicios de Mercado Pago
builder.Services.AddScoped<CardTokenClient>();
builder.Services.AddScoped<CustomerClient>();
builder.Services.AddScoped<PaymentClient>();
builder.Services.AddScoped<IMercadoPagoService, MercadoPagoService>();

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
builder.Services.AddScoped<ICompraService, CompraService>();

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
app.UseSwagger(); // Habilita Swagger en todos los entornos

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "XDeco Api V1");
});

// Configuración adicional para el manejo de errores y seguridad
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

// Configura la sesión
app.UseSession(); // Añadir esto

// Asegúrate de agregar autenticación antes de la autorización
app.UseAuthentication();
app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
