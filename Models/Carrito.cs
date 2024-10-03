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

        [ForeignKey("Usuario")]
        public string UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public decimal Total
        {
            get
            {
                decimal total = 0;
                foreach (var producto in Productos)
                {
                    total += producto.Precio;
                }
                return total;
            }
        }
    }
}