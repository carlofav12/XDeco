using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XDeco.Models;
using Proyecto.Data;
using Microsoft.EntityFrameworkCore;

namespace XDeco.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ILogger<ProductosController> _logger;
        private readonly ApplicationDbContext _context;

        public ProductosController(ILogger<ProductosController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var productos = _context.Productos.Include(p => p.Categoria).ToList();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var producto = _context.Productos.Include(p => p.Categoria)
                .Include(p => p.Dimensiones)
                .FirstOrDefault(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }
    }
    // agregar metodos modifica eliminar
}