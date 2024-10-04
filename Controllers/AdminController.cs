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
            _context = context; 
        }

        public IActionResult Index()
        {
            return View(); 
        }

        [HttpPost]
        public IActionResult Login(string usuAdmin, string contraAdmin)
        {
            
            var admin = _context.Administradores
                                .FirstOrDefault(a => a.usuAdmin == usuAdmin && a.contraAdmin == contraAdmin);

            if (admin != null)
            {
                
                ViewBag.SuccessMessage = "Credenciales correctas";
                return RedirectToAction("Vista", "Admin");
            }
            else
            {
                
                ViewBag.ErrorMessage = "Credenciales incorrectas";
                return View("Index"); 
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
             return View("Index","Home");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!"); // Muestra una vista de error
        }
    }
}
