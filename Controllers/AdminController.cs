using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyecto.Data;

namespace XDeco.Controllers
{
    
  public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context; // Usar ApplicationDbContext

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Inicializa el DbContext
        }

        public IActionResult Index()
        {
            return View(); // Muestra la vista de login
        }

        [HttpPost]
        public IActionResult Login(string usuAdmin, string contraAdmin)
        {
            // Busca si hay un administrador con las credenciales ingresadas
            var admin = _context.Administradores
                                .FirstOrDefault(a => a.usuAdmin == usuAdmin && a.contraAdmin == contraAdmin);

            if (admin != null)
            {
                // Si las credenciales son correctas, mostrar un mensaje de éxito
                ViewBag.SuccessMessage = "Credenciales correctas";
                return RedirectToAction("Index", "Home"); // Se mantiene en la vista de login
            }
            else
            {
                // Si las credenciales no coinciden, mostrar un mensaje de error
                ViewBag.ErrorMessage = "Credenciales incorrectas";
                return View("Index"); // Vuelve a la vista de login
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
