using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XDeco.Models;

namespace XDeco.ViewModel
{
    public class CarritoViewModel
    {
        // Lista de productos en el carrito con su cantidad
        public List<CarritoProductoViewModel> CarritoProductos { get; set; } = new List<CarritoProductoViewModel>();

        // Total del carrito
        public decimal Total { get; set; }

        // Constructor que recibe el carrito del modelo
        public CarritoViewModel(Carrito carrito)
        {
            // Mapeamos los productos y cantidades del carrito al ViewModel
            foreach (var item in carrito.CarritoProductos)
            {
                CarritoProductos.Add(new CarritoProductoViewModel
                {
                    ProductoId = item.ProductoId,
                    Nombre = item.Producto.Nombre,
                    Precio = item.Producto.Precio,
                    Cantidad = item.Cantidad,
                    Subtotal = item.Subtotal
                });
            }

            // Asignamos el total del carrito
            Total = carrito.Total;
        }
    }

    public class CarritoProductoViewModel
    {
        public long ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }
}