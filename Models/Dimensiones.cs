using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace XDeco.Models
{
    [Table("Dimensiones")]
    public class Dimensions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public decimal Largo { get; set; }

        public decimal Ancho { get; set; }

        public decimal Alto { get; set; }

        public long ProductoId { get; set; }


        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }
    }
}