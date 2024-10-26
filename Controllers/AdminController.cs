using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Proyecto.Data;
using XDeco.Models;

namespace XDeco.Controllers
{
    [ServiceFilter(typeof(AdminAuthorizationFilter))] // Aplica el filtro personalizado
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            ILogger<AdminController> logger,
            ApplicationDbContext context,
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(); // Muestra la vista de login
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Busca al usuario en la base de datos
            var admin = await _context.Administradores
                .FirstOrDefaultAsync(a => a.usuAdmin == username && a.contraAdmin == password);

            if (admin != null)
            {
                // Aquí estableces el rol manualmente
                if (admin.usuAdmin == username) // Asegúrate de que tienes la lógica para determinar el rol
                {
                    // Establecer una sesión indicando que el usuario está autenticado como admin
                    HttpContext.Session.SetString("IsAdminAuthenticated", "true");
                    HttpContext.Session.SetString("AdminUsername", admin.usuAdmin);

                    // Redirige a la página principal del área de administración
                    return RedirectToAction("Vista");
                }
            }

            // Mostrar un mensaje de error si las credenciales son incorrectas
            ViewBag.ErrorMessage = "Nombre de usuario o contraseña incorrectos.";
            return View("Index");
        }


        public IActionResult Vista()
        {
            return View(); // Muestra la vista de administración
        }

        public IActionResult ListaClientes()
        {
            var usuarios = _context.Users.ToList();
            return View(usuarios);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Redirige a la vista de inicio
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
