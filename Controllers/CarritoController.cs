using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Proyecto.Data;
using XDeco.Models;
using XDeco.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace XDeco.Controllers
{
    [Authorize]
    public class CarritoController : Controller
    {
        private readonly ILogger<CarritoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public CarritoController(ILogger<CarritoController> logger, ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);


            var carrito = _context.Carritos
                .Include(c => c.CarritoProductos)
                .ThenInclude(cp => cp.Producto)
                .FirstOrDefault(c => c.UsuarioId == userId);

            if (carrito == null)
            {

                carrito = new Carrito { UsuarioId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            var viewModel = new CarritoViewModel(carrito);
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AñadirAlCarrito(long productoId, int cantidad = 1)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.Id == productoId);
            if (producto == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var carrito = _context.Carritos
                .FirstOrDefault(c => c.UsuarioId == userId);

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    UsuarioId = userId
                };
                _context.Carritos.Add(carrito);
            }


            var carritoProducto = carrito.CarritoProductos.FirstOrDefault(cp => cp.ProductoId == productoId);
            if (carritoProducto != null)
            {

                carritoProducto.Cantidad += cantidad;
            }
            else
            {

                carrito.CarritoProductos.Add(new CarritoProducto
                {
                    Producto = producto,
                    Cantidad = cantidad
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}