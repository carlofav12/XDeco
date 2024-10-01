using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace XDeco.Models
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        public Decimal Precio { get; set; }

        public string? ImageURL { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }
        public bool EsAjustable { get; set; }
        public Dimensions? Dimensiones { get; set; }
    }
}