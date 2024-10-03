using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace XDeco.Models
{
    [Table("Carritos")]
    public class Carrito
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string UsuarioId { get; set; }

        // Relación entre Carrito y Producto, usando CarritoProducto para manejar cantidades
        public virtual ICollection<CarritoProducto> CarritoProductos { get; set; } = new List<CarritoProducto>();

        public decimal Total
        {
            get
            {
                return CarritoProductos.Sum(cp => cp.Subtotal);
            }
        }
    }
}
