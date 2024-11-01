using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Data;
using XDeco.Models;
using Ml_xdeco;
using static Ml_xdeco.MLtextclasification;

namespace XDeco.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: contacto/index
        [HttpGet("index")]
        public IActionResult Index()
        {
            return View(); 
        }

        // POST: contacto/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contacto objcontato)
        {
            if (!ModelState.IsValid) 
            {
                return View("Index", objcontato);
            }

            // Agregar el contacto a la base de datos
            _context.Add(objcontato);
            await _context.SaveChangesAsync();

            // Realizar la predicción utilizando el modelo ML.NET
            var input = new ModelInput { Mensaje = objcontato.Mensaje };
            var result = MLtextclasification.Predict(input); // Llama al método Predict

            // Determinar el mensaje según la predicción
            string mensajeClasificacion;
            if (result.PredictedLabel == 0)
            {
                mensajeClasificacion = "Gracias por su contacto.";
            }
            else if (result.PredictedLabel == 1)
            {
                mensajeClasificacion = "Estamos intentando mejorar. Su opinión es importante.";
            }
            else
            {
                mensajeClasificacion = "Clasificación desconocida.";
            }

            TempData["Message"] = mensajeClasificacion; // Almacena el mensaje en TempData
            return RedirectToAction("Index");
        }
    }
}