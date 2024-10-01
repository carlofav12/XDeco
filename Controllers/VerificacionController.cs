using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XDeco.Service;

namespace XDeco.Controllers
{
    public class VerificacionController : Controller
    {
        private readonly ILogger<VerificacionController> _logger;
        private readonly EmailService _emailService; // Servicio para enviar correos
        private static readonly Dictionary<string, string> _verificationCodes = new(); // Almacena códigos de verificación

        public VerificacionController(ILogger<VerificacionController> logger, EmailService emailService)
        {
            _logger = logger;
            _emailService = emailService; // Asignación del servicio inyectado
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAndVerifyCode(string email, string code)
        {
            // Si no se ha ingresado un código, genera uno nuevo
            if (string.IsNullOrEmpty(code))
            {
                // Genera un código aleatorio
                var verificationCode = new Random().Next(100000, 999999).ToString();

                // Almacena el código en un diccionario
                _verificationCodes[email] = verificationCode;

                try
                {
                    // Envía el código al correo electrónico
                    await _emailService.SendEmailAsync(email, "Código de Verificación", $"Tu código es: {verificationCode}");
                    ViewData["Message"] = "Se ha enviado un código a tu correo.";
                }
                catch (Exception ex)
                {
                    ViewData["Message"] = $"Error al enviar correo: {ex.Message}";
                }
            }
            else
            {
                // Verifica el código ingresado
                if (_verificationCodes.TryGetValue(email, out var storedCode) && storedCode == code)
                {
                    ViewData["Message"] = "Código verificado con éxito.";
                    ViewData["IsVerified"] = true; // Indica que la verificación fue exitosa

                    // Aquí puedes establecer el UserId y ConfirmationToken si los tienes disponibles.
                    ViewData["UserId"] = "user-id"; // Reemplaza con el ID real del usuario
                    ViewData["ConfirmationToken"] = "confirmation-token"; // Reemplaza con el token real
                }
                else
                {
                    ViewData["Message"] = "Código incorrecto. Inténtalo nuevamente.";
                    ViewData["IsVerified"] = false; // Indica que la verificación falló
                }
            }

            return View("Index"); // Redirige a la vista para mostrar el mensaje
        }
    }
}