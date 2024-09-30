using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace XDeco.Models
{
    public class Usuario : IdentityUser
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El apellido paterno es obligatorio")]
        public string ApellidoPaterno { get; set; }

        [Required(ErrorMessage = "El apellido materno es obligatorio")]
        public string ApellidoMaterno { get; set; }
    }
}
