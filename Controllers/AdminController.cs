using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyecto.Data;
using XDeco.Models;

namespace XDeco.Controllers
{
    [ServiceFilter(typeof(AdminAuthorizationFilter))] // Aplica el filtro personalizado
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<Usuario> _userManager; // Usa UserManager para manejar usuarios
        private readonly SignInManager<Usuario> _signInManager; // Para manejar el inicio de sesión
        private readonly ApplicationDbContext _context;

        public AdminController(
            ILogger<AdminController> logger, 
            ApplicationDbContext context,
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager; // Inicializa UserManager
            _signInManager = signInManager; // Inicializa SignInManager
        }

        public IActionResult Index()
        {
            return View(); // Muestra la vista de inicio de sesión
        }

        [HttpPost]
        [AllowAnonymous] // Permite que no autenticados puedan acceder a esta acción
        public async Task<IActionResult> Login(string email, string password)
        {
            // Intenta iniciar sesión con las credenciales proporcionadas
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard"); // Redirige al panel de administración
            }
            else
            {
                ViewBag.ErrorMessage = "Credenciales incorrectas";
                return View("Index"); // Vuelve a la vista de inicio de sesión
            }
        }

        public IActionResult Dashboard()
        {
            return View(); // Muestra el panel de administración
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!"); // Muestra una vista de error
        }
    }
}
