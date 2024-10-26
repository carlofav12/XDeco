using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace XDeco.Integration.nytimes
{
    public class NYTimesApiIntegration
    {
        private readonly ILogger<NYTimesApiIntegration> _logger;

        // La URL de la API de noticias, el query se pasará dinámicamente
        private const string API_URL = "https://newsapi.org/v2/everything?q={0}&from=2024-10-01&sortBy=publishedAt&apiKey=41522f04ee08497882d367457b6c35e7";

        public NYTimesApiIntegration(ILogger<NYTimesApiIntegration> logger)
        {
            _logger = logger;
        }

        public async Task<List<Article>> GetNews(string query)
        {
            var url = string.Format(API_URL, Uri.EscapeDataString(query));
            List<Article> filtro = new List<Article>();

            try
            {
                using (var client = new WebClient())
                {
                    var json = await client.DownloadStringTaskAsync(url);
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(json);
                    filtro = apiResponse.Articles.ToList();
                    filtro.RemoveAll(item => item.UrlToImage == null);
                }
            }
            catch (WebException webEx)
            {
                _logger.LogError($"Error al llamar a la API: {webEx.Message}");
                // Puedes manejar el error de acuerdo a tus necesidades
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado: {ex.Message}");
            }

            return filtro;
        }
    }
}
