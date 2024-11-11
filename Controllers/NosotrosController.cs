using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace XDeco.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NosotrosController : Controller
    {
        private readonly ILogger<NosotrosController> _logger;
        private readonly HttpClient _httpClient;

        public NosotrosController(ILogger<NosotrosController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string phrase = "Frase no disponible";
            string author = "Autor desconocido";
            
            try
            {
                var response = await _httpClient.GetStringAsync("https://frasedeldia.azurewebsites.net/api/phrase");
                
                dynamic phraseData = JsonConvert.DeserializeObject(response);
                phrase = phraseData.phrase;
                author = phraseData.author;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la frase del día: {ex.Message}");
            }

            ViewData["Phrase"] = phrase;
            ViewData["Author"] = author;

            return View();
        }

        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
