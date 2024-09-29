using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public IActionResult Index()
        {
            var misProductos = _context.Productos.Include(p => p.Categoria).ToList();

            _logger.LogDebug("Productos: {misProductos}", misProductos);

            var viewModel = new CatalogoViewModel
            {
                FormProducto = new Producto(),
                ListProduc = misProductos
            };

            _logger.LogDebug("ViewModel: {viewModel}", viewModel);

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}