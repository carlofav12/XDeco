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
        public ContactoController(ApplicationDbContext context) { 
            _context = context;
        }

        // GET: contacto/index
        [HttpGet("Contacto")]
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
            
            // Realizar la predicción utilizando el modelo ML.NET
            var input = new ModelInput { Mensaje = objcontato.Mensaje };
            var result = MLtextclasification.Predict(input); // Llama al método Predict 
            
            string sensibilidad;
            string mensajeClasificacion;
            if (result.PredictedLabel == 0)
            {
                mensajeClasificacion = "Gracias por su contacto.";
                sensibilidad="Positivo";
            }
            else if (result.PredictedLabel == 1)
            {
                mensajeClasificacion = "Estamos intentando mejorar. Su opinión es importante.";
                sensibilidad="Negativo";
            }
            else
            {
                mensajeClasificacion = "Clasificación desconocida.";
                sensibilidad="No identificado";
            }
            
         
            objcontato.Sensibilidad = sensibilidad;

            
            _context.Add(objcontato);
            await _context.SaveChangesAsync();

            TempData["Message"] = mensajeClasificacion; // Almacena el mensaje en TempData 
            return RedirectToAction("Index");
        }

        }
}