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
using System.Security.Claims;

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
                    // Si el producto ya está en el carrito, sumamos la cantidad
                    carritoProducto.Cantidad += cantidad;
                }
                else
                {
                    // Si el producto no está en el carrito, lo agregamos con la cantidad inicial
                    carrito.CarritoProductos.Add(new CarritoProducto
                    {
                        ProductoId = productoId,
                        Cantidad = cantidad,
                        Producto = producto
                    });
                }

                await _context.SaveChangesAsync();

                // Regresa a la página de inicio después de agregar o actualizar el carrito
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al añadir producto al carrito.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // En CarritoController.cs
        [HttpGet]
        public IActionResult GetCarritoCount()
        {
            // Obtener el ID del usuario actual
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Asegúrate de que el modelo 'Carrito' usa 'UsuarioId' en lugar de 'UserId'
            var count = _context.CarritoProductos
                                .Where(cp => cp.Carrito.UsuarioId == userId) // Cambiar 'UserId' por 'UsuarioId'
                                .Sum(cp => cp.Cantidad);
            
            return Json(new { count });
        }


        [HttpPost]
        public async Task<JsonResult> FinalizarCompra([FromBody] DatosCompraModel datos)
        {
            if (datos == null || !ModelState.IsValid)
            {
                return Json(new { resultado = false, mensaje = "Datos de compra no válidos." });
            }

            var userId = _userManager.GetUserId(User);

            // Obtener el carrito del usuario
            var carrito = await _context.Carritos
                .Include(c => c.CarritoProductos)
                .ThenInclude(cp => cp.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito == null || !carrito.CarritoProductos.Any())
            {
                return Json(new { resultado = false, mensaje = "El carrito está vacío." });
            }

            // Crear la compra y asignar productos desde el carrito
            var compra = new Compra
            {
                UsuarioId = userId,
                // El Total es calculado usando la cantidad de productos del carrito y el precio unitario
                Total = datos.Productos.Sum(dp => dp.Cantidad * 
                    (carrito.CarritoProductos.FirstOrDefault(cp => cp.ProductoId == dp.ProductoId)?.Producto.Precio ?? 0)),
                CompraProductos = datos.Productos.Select(dp =>
                {
                    var carritoProducto = carrito.CarritoProductos.FirstOrDefault(cp => cp.ProductoId == dp.ProductoId);
                    return new CompraProducto
                    {
                        ProductoId = dp.ProductoId,
                        Cantidad = dp.Cantidad, // Usamos la cantidad enviada desde el frontend
                        PrecioUnitario = carritoProducto?.Producto.Precio ?? 0
                    };
                }).ToList()
            };

            // Agregar la compra a la base de datos
            _compraService.AgregarCompra(compra);

            // Eliminar los productos del carrito después de la compra
            _context.CarritoProductos.RemoveRange(carrito.CarritoProductos);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Compra procesada exitosamente para el usuario {UserId}: {@Compra}", userId, compra);

            

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
            MercadoPagoConfig.AccessToken = "APP_USR-816685383498223-111000-82efb998cdae42dc39201ca457aeb7f0-2086868631";
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.CarritoProductos)
                .ThenInclude(cp => cp.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (carrito == null || !carrito.CarritoProductos.Any())
            {
                // Mensaje si no hay productos en el carrito
                return RedirectToAction("Index");
            }

            var compra = new Compra
            {
                UsuarioId = userId,
                Total = carrito.CarritoProductos.Sum(cp => cp.Cantidad * cp.Producto.Precio),
                CompraProductos = carrito.CarritoProductos.Select(cp => new CompraProducto
                {
                    ProductoId = cp.ProductoId,
                    Cantidad = cp.Cantidad,
                    PrecioUnitario = cp.Producto.Precio
                }).ToList()
            };

            _compraService.AgregarCompra(compra);

            // Crear preferencia para Mercado Pago
            var preferenceRequest = new PreferenceRequest
            {
                Items = carrito.CarritoProductos.Select(cp => new PreferenceItemRequest
                {
                    Title = cp.Producto.Nombre,
                    Quantity = cp.Cantidad,
                    CurrencyId = "PEN",
                    UnitPrice = cp.Producto.Precio
                }).ToList()
            };

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(preferenceRequest); 

            // Redirigir al checkout de Mercado Pago
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
