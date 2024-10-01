
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace XDeco.Models
{
    [Table("Contacto")]
    public class Contacto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string? Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Titulo { get; set; }

        [Required]
        public string? Mensaje { get; set; }

        [Phone]
        public string? Telefono { get; set; }

        public string? Direccion { get; set; }
    }
}