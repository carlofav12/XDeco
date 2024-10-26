using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using XDeco.Integration.nytimes;
using XDeco.Models;
using XDeco.Service;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("PosgreConnection")
    ?? throw new InvalidOperationException("Connection string 'PosgreConnection' not found.");

// Configurar DbContext con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configurar el filtro para excepciones de desarrollo
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configurar identidad y opciones de usuario
builder.Services.AddDefaultIdentity<Usuario>(options => options.SignIn.RequireConfirmedAccount = true)
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

var app = builder.Build();

// Configurar el pipeline de la aplicación
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
app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
