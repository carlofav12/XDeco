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
            return View();
        }

        [HttpPost]
        [AllowAnonymous] // Permite que no autenticados puedan acceder a esta acción
        public async Task<IActionResult> Login(string email, string password)
        {

            var admin = _context.Administradores
                                .FirstOrDefault(a => a.usuAdmin == usuAdmin && a.contraAdmin == contraAdmin);

            if (result.Succeeded)
            {

                ViewBag.SuccessMessage = "Credenciales correctas";
                return RedirectToAction("Vista", "Admin");
            }
            else
            {
                // Si las credenciales no coinciden, mostrar un mensaje de error
                ViewBag.ErrorMessage = "Credenciales incorrectas";
                return View("Index"); // Vuelve a la vista de inicio de sesión
            }
        }

        public IActionResult Vista()
        {
            return View(); // Muestra la vista de administración
        }

        public IActionResult ListaClientes()
        {
            var usuarios = _context.Users.ToList(); // Obtener la lista de usuarios
            return View(usuarios); // Pasar la lista a la vista
        }
        public IActionResult Logout()
        {
            return View("Index", "Home");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!"); // Muestra una vista de error
        }
    }
}
