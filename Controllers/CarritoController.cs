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
        private readonly ICompraService _compraService;

        public CarritoController(ILogger<CarritoController> logger, ApplicationDbContext context, UserManager<Usuario> userManager, IMercadoPagoService mercadoPagoService, ICompraService compraService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _mercadoPagoService = mercadoPagoService;
            _compraService = compraService;
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
                    // Actualiza la cantidad existente
                    carritoProducto.Cantidad += cantidad; // Asegúrate de que la cantidad se está sumando
                }
                else
                {
                    // Añade el nuevo producto al carrito
                    carrito.CarritoProductos.Add(new CarritoProducto
                    {
                        ProductoId = productoId,
                        Cantidad = cantidad,
                        Producto = producto // Asegúrate de que estás enlazando el producto correctamente
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

            // Crear el objeto CompraViewModel con las cantidades enviadas desde la vista
            var compraViewModel = new CompraViewModel
            {
                CarritoId = carrito.Id,
                Productos = datos.Productos.Select(dp => {
                    var carritoProducto = carrito.CarritoProductos.FirstOrDefault(cp => cp.ProductoId == dp.ProductoId);
                    return new CompraProducto
                    {
                        ProductoId = dp.ProductoId,
                        Cantidad = dp.Cantidad, // Usamos la cantidad enviada desde el frontend
                        PrecioUnitario = carritoProducto?.Producto.Precio ?? 0,
                        Nombre = carritoProducto?.Producto.Nombre,
                        ImagenUrl = carritoProducto?.Producto.ImageURL
                    };
                }).ToList(),
                TotalAPagar = datos.Productos.Sum(dp => dp.Cantidad * (carrito.CarritoProductos
                                        .FirstOrDefault(cp => cp.ProductoId == dp.ProductoId)?.Producto.Precio ?? 0)),
                Nombre = datos.Nombre,
                Direccion = datos.Direccion,
                Telefono = datos.Telefono,
                UsuarioId = userId
            };

            _compraService.AgregarCompra(compraViewModel);
            _logger.LogInformation("Compra procesada exitosamente para el usuario {UserId}: {@CompraViewModel}", userId, compraViewModel);

            _context.CarritoProductos.RemoveRange(carrito.CarritoProductos);
            await _context.SaveChangesAsync();


            return Json(new { resultado = true, mensaje = "Compra procesada exitosamente." });
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

        [HttpGet]
        public IActionResult Compras()
        {
            ViewData["ActivePage"] = "Compras";
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);
            
            // Use the service to get purchases for this user
            var compras = _compraService.ObtenerComprasPorUsuario(userId);
            
            
            return View(compras); // Return the purchases in a view
        }

    }

}
