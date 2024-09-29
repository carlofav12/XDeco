using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace XDeco.Views.Catalogo
{
    public class Producto : PageModel
    {
        private readonly ILogger<Producto> _logger;

        public Producto(ILogger<Producto> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}