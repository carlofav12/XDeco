using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XDeco.Integration.nytimes;

namespace XDeco.Controllers
{
    public class NoticiasController : Controller
    {
        private readonly ILogger<NoticiasController> _logger;
        private readonly NYTimesApiIntegration _lista;

        public NoticiasController(ILogger<NoticiasController> logger, NYTimesApiIntegration lista)
        {
            _logger = logger;
            _lista = lista;
        }
        
        
        public async Task<IActionResult> Index(string? searchString)
        {
            List<Article> listB = new List<Article>();
            if (!String.IsNullOrEmpty(searchString))
            {
                listB = await _lista.GetNews(searchString);
            }
            
            return View(listB);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
