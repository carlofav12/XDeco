using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XDeco.Models
{
    public class DatosCompraModel
    {
        public string Nombre { get; set; }
        public string NumeroTarjeta { get; set; }
        public string Vigencia { get; set; }
        public string Cvv { get; set; }
        public string Direccion { get; set; }
        public string Contacto { get; set; }
        public string Telefono { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }

        // Lista de productos con cantidades
        public List<ProductoCompra> Productos { get; set; } = new List<ProductoCompra>();
    }

    // Clase para cada producto en la compra, con ID y cantidad
    public class ProductoCompra
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
