using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using XDeco.Models;

namespace XDeco.Controllers
{
    [Route("contacto")] // Cambia a una ruta más específica si es necesario
    public class ContactoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contacto/Create
        
        public IActionResult Index()
        {
            return View();
        }

        // POST: Contacto/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contacto objcontato)
        {
            if (ModelState.IsValid) // Verifica si el modelo es válido
            {
                _context.Add(objcontato);
                await _context.SaveChangesAsync();

                ViewData["Message"] = "Se registró el contacto";
                return RedirectToAction("Index"); // Redirige a Index para evitar reenvío de formulario
            }

            return View("Index", objcontato); // Devuelve a la vista Index con el modelo si hay errores
        }
    }
}
