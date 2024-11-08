using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace XDeco.Models
{
    [Table("Compras")]
    public class Compra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // ID único de la compra, generado automáticamente

        public string UsuarioId { get; set; } // ID del usuario que realizó la compra

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } // Relación con el modelo Usuario para acceder a sus detalles

        // Relación entre Compra y CompraProducto para manejar los productos en la compra
        public virtual ICollection<CompraProducto> CompraProductos { get; set; } = new List<CompraProducto>();

        public decimal Total { get; set; } // Total de la compra, se almacenará en la base de datos
    }
}
