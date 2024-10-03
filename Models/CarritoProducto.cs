using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDeco.Models
{
    [Table("CarritoProductos")]
    public class CarritoProducto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long CarritoId { get; set; }
        public virtual Carrito Carrito { get; set; }

        public long ProductoId { get; set; }
        public virtual Producto Producto { get; set; }

        public int Cantidad { get; set; }

        public decimal Subtotal
        {
            get
            {
                return Cantidad * Producto.Precio;
            }
        }
    }
}
