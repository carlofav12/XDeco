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
using XDeco.Service;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;

namespace XDeco.Controllers
{
    [Authorize]
    public class CarritoController : Controller
    {
        private readonly ILogger<CarritoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMercadoPagoService _mercadoPagoService;

        public CarritoController(ILogger<CarritoController> logger, ApplicationDbContext context, UserManager<Usuario> userManager, IMercadoPagoService mercadoPagoService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _mercadoPagoService = mercadoPagoService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoProductos)
                .ThenInclude(cp => cp.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

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
    try
    {
        var producto = await _context.Productos.FindAsync(productoId);
        if (producto == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var carrito = await _context.Carritos
            .Include(c => c.CarritoProductos)
            .FirstOrDefaultAsync(c => c.UsuarioId == userId);

        if (carrito == null)
        {
            carrito = new Carrito { UsuarioId = userId };
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
                Cantidad = cantidad,
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al añadir producto al carrito.");
        return StatusCode(500, "Error interno del servidor.");
    }
}





        [HttpPost]
        public async Task<JsonResult> FinalizarCompra([FromBody] DatosCompraModel datos)
        {
            if (datos == null || !ModelState.IsValid)
            {
                return Json(new { resultado = false, mensaje = "Datos de compra no válidos." });
            }

            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoProductos)
                .ThenInclude(cp => cp.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito == null || !carrito.CarritoProductos.Any())
            {
                return Json(new { resultado = false, mensaje = "El carrito está vacío." });
            }

            // Lógica para procesar la compra.
            bool resultado = true; // Cambiar según la lógica de tu negocio
            string mensaje = "Compra realizada con éxito."; // Cambiar según sea necesario

            // Limpiar el carrito después de la compra
            _context.Carritos.Remove(carrito);
            await _context.SaveChangesAsync();

            // Mostrar mensaje de éxito con SweetAlert
            return Json(new { resultado, mensaje });
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCarrito(long carritoId, long productoId)
        {
            var userId = _userManager.GetUserId(User);

            // Buscar el carrito del usuario
            var carrito = await _context.Carritos
                .Include(c => c.CarritoProductos)
                .FirstOrDefaultAsync(c => c.Id == carritoId && c.UsuarioId == userId);

            if (carrito == null)
            {
                return NotFound();
            }

            // Buscar el producto dentro del carrito
            var carritoProducto = carrito.CarritoProductos.FirstOrDefault(cp => cp.ProductoId == productoId);

            if (carritoProducto != null)
            {
                carrito.CarritoProductos.Remove(carritoProducto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PagoConMercadoPago()
        {
            // Configuración de Mercado Pago
            MercadoPagoConfig.AccessToken = "TEST-2040961728898445-102315-dfd5f8da7ed98d6c0428ddd0d21ab1e5-2052616451";

            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoProductos)
                .ThenInclude(cp => cp.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito == null || !carrito.CarritoProductos.Any())
            {
                return RedirectToAction("Index"); // O algún mensaje de error indicando que no hay productos en el carrito
            }

            var preferenceRequest = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>()
            };

            foreach (var item in carrito.CarritoProductos)
            {
                var preferenceItem = new PreferenceItemRequest
                {
                    Title = item.Producto.Nombre,
                    Quantity = item.Cantidad,
                    CurrencyId = "PEN",
                    UnitPrice = (decimal)item.Producto.Precio
                };
                preferenceRequest.Items.Add(preferenceItem);
            }

            var client = new PreferenceClient();
            Preference preference = client.Create(preferenceRequest);

            // Redirigir al checkout de Mercado Pago sandbox
            return Redirect(preference.InitPoint);
        }

    }

}
