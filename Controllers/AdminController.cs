using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Proyecto.Data;
using XDeco.Models;
using XDeco.ViewModel;

namespace XDeco.Controllers
{
    [ServiceFilter(typeof(AdminAuthorizationFilter))] // Aplica el filtro personalizado
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ICompraService _compraService;

        public AdminController(
            ILogger<AdminController> logger,
            ApplicationDbContext context,
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ICompraService compraService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _compraService = compraService;
        }


        public IActionResult ListaCompras()
        {
            List<Compra> compras = _compraService.ObtenerCompras();
            
            // Verificamos si la lista de compras está vacía
            if (compras == null || !compras.Any())
            {
                _logger.LogInformation("No hay compras disponibles en la base de datos.");
            }
            else
            {
                _logger.LogInformation($"Se han encontrado {compras.Count} compras en la base de datos.");
            }
            
            if (compras == null)
            {
                compras = new List<Compra>(); // Asegura que nunca sea null
            }

            return View(compras);
        }



        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(); // Muestra la vista de login
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Busca al usuario en la base de datos
            var admin = await _context.Administradores
                .FirstOrDefaultAsync(a => a.usuAdmin == username && a.contraAdmin == password);

            if (admin != null)
            {
                // Aquí estableces el rol manualmente
                if (admin.usuAdmin == username) // Asegúrate de que tienes la lógica para determinar el rol
                {
                    // Establecer una sesión indicando que el usuario está autenticado como admin
                    HttpContext.Session.SetString("IsAdminAuthenticated", "true");
                    HttpContext.Session.SetString("AdminUsername", admin.usuAdmin);

                    // Redirige a la página principal del área de administración
                    return RedirectToAction("Vista");
                }
            }

            // Mostrar un mensaje de error si las credenciales son incorrectas
            ViewBag.ErrorMessage = "Nombre de usuario o contraseña incorrectos.";
            return View("Index");
        }
        
        //LOGICA PARA LISTAR CLIENTES , EDITAR ELIMINAR
        public IActionResult Vista()
        {
            return View(); // Muestra la vista de administración
        }

        public IActionResult ListaClientes()
        {
            var usuarios = _context.Users.ToList();
            return View(usuarios);
        }
           [HttpGet]
        public IActionResult Edit(string id)
        {
            var usuario = _context.Users.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return PartialView("_EditUserPartial", usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Usuario model)
        {
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (usuario != null)
            {
                usuario.Nombres = model.Nombres;
                usuario.ApellidoPaterno = model.ApellidoPaterno;
                usuario.ApellidoMaterno = model.ApellidoMaterno;
                usuario.Email = model.Email;
                usuario.PhoneNumber=model.PhoneNumber;
               

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ListaClientes");
        }

        //METODO PARA ELIMINAR 
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var usuario = _context.Users.FirstOrDefault(u => u.Id == id);

            if (usuario != null)
            {
                _context.Users.Remove(usuario);
                _context.SaveChanges();
            }

            return RedirectToAction("ListaClientes");
        }

        //TERMINA LA LOGICA PARA LOS CLIENTES
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!"); // Muestra una vista de error
        }

         public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); 
        }

        public IActionResult Dashboard()
        {
            return View(); 
        }

        //LOGICA PARA LISTAR , ELIMINAR CONTACTOS.

         public async Task<IActionResult> ListarContacto()
        {
            List<Contacto> contactos = await _context.Contacto.ToListAsync();
            return View(contactos); 
        }
        [HttpPost]
        public async Task<IActionResult> Eliminar(long id) 
        {
            
            var contacto = await _context.Contacto.FirstOrDefaultAsync(c => c.Id == id);

            if (contacto != null)
            {
                _context.Contacto.Remove(contacto); 
                await _context.SaveChangesAsync(); 
            }

            return RedirectToAction("ListarContacto"); 
        }
        
        //PRODUCTOS
        public IActionResult ListaProductos()
        {
            var productos = _context.Productos.ToList();
            return View(productos);
        }

        //ELIMINAR PRODUCTO
        [HttpPost]
        public async Task<IActionResult> EliminarProducto(long id) 
        {
            
            var producto = await _context.Productos.FirstOrDefaultAsync(c => c.Id == id);

            if (producto != null)
            {
                _context.Productos.Remove(producto); 
                await _context.SaveChangesAsync(); 
            }

            return RedirectToAction("ListaProductos"); 
        }

        //EDITAR PRODUCTO
        [HttpGet]
        public async Task<IActionResult> EditarProducto(int id)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return PartialView("_EditarProducto", producto); // Asegúrate de tener esta vista parcial
        }

        [HttpPost]
        public async Task<IActionResult> EditarProducto(Producto model)
        {
            // Buscar el producto en la base de datos usando el Id
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == model.Id);
            
            if (producto != null)
            {
                // Actualizar las propiedades del producto con los datos enviados desde el formulario
                producto.Nombre = model.Nombre;
                producto.Descripcion = model.Descripcion;
                producto.Precio = model.Precio;
                producto.Material = model.Material;
                producto.Color = model.Color;
                producto.EsAjustable = model.EsAjustable;
                producto.ImageURL = model.ImageURL;
                producto.UrlObj = model.UrlObj;

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }

            // Redirigir a la lista de productos después de la actualización
            return RedirectToAction("ListaProductos");
        }

    }
}
