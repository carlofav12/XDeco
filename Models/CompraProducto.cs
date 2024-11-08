using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XDeco.Models
{
    [Table("CompraProductos")]
    public class CompraProducto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // ID único del registro de producto en la compra, generado automáticamente

        public long CompraId { get; set; } // ID de la compra asociada

        [ForeignKey("CompraId")]
        public virtual Compra Compra { get; set; } // Relación con la entidad Compra

        public virtual Producto Producto { get; set; }  // Aquí la propiedad 'Producto' debe ser virtual para la relación de navegación
        public long ProductoId { get; set; } // ID del producto
        public int Cantidad { get; set; } // Cantidad de este producto

        public decimal PrecioUnitario { get; set; } // Precio por unidad del producto

        [NotMapped] // No se guarda en la base de datos
        public decimal Subtotal
        {
            get
            {
                // Calcula el subtotal multiplicando la cantidad por el precio unitario
                return Cantidad * PrecioUnitario;
            }
        }
    }
}
