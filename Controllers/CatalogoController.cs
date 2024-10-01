using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XDeco.Models;
using XDeco.ViewModel;
using Proyecto.Data;
using Microsoft.EntityFrameworkCore;

namespace XDeco.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;

        public CatalogoController(ILogger<CatalogoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Acción para la página de catálogo con filtros de categoría y ordenación por precio
        public IActionResult Index(string category = "all", string orderBy = "lower-price")
        {
            // Obtener todos los productos y sus categorías
            var productos = _context.Productos.Include(p => p.Categoria).AsQueryable();

            // Filtrar por categoría si no es 'all'
            if (category != "all")
            {
                productos = productos.Where(p => p.Categoria.Nombre.ToLower() == category.ToLower());
            }

            // Ordenar los productos según el filtro de precio
            switch (orderBy)
            {
                case "lower-price":
                    productos = productos.OrderBy(p => p.Precio);
                    break;
                case "higher-price":
                    productos = productos.OrderByDescending(p => p.Precio);
                    break;
            }

            // Guardar los valores seleccionados en ViewBag para mostrarlos en la vista
            ViewBag.Category = category;
            ViewBag.OrderBy = orderBy;

            // Obtener todas las categorías disponibles para mostrarlas en el dropdown
            ViewBag.Categorias = _context.Categorias.ToList();

            // Crear el ViewModel y pasar los productos filtrados
            var viewModel = new CatalogoViewModel
            {
                FormProducto = new Producto(),
                ListProduc = productos.ToList()
            };

            return View(viewModel);
        }

        // Acción para mostrar un producto específico
        public IActionResult Producto(int id)
        {
            var producto = _context.Productos.Include(p => p.Categoria)
                .Include(p => p.Dimensiones)
                .FirstOrDefault(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
