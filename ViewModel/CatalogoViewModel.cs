using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XDeco.Models;

namespace XDeco.ViewModel
{
    public class CatalogoViewModel
    {
        public Producto? FormProducto { get; set; }
        public IEnumerable<Producto>? ListProduc { get; set; }
    }
}