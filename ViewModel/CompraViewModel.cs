using System.Collections.Generic;

namespace XDeco.ViewModel
{
    public class CompraViewModel
    {
        public long Id { get; set; } // ID único de la compra
        public long CarritoId { get; set; } // El ID del carrito
        public List<CompraProducto> Productos { get; set; } // Lista de productos en la compra
        public decimal TotalAPagar { get; set; } // Total a pagar
        public string MetodoPago { get; set; } // Método de pago elegido
        public string TipoTarjeta { get; set; } // Tipo de tarjeta (Visa, Mastercard, etc.)
        
        // Información adicional del cliente
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        
        // ID del usuario que realizó la compra
        public string UsuarioId { get; set; } // Agregado para asociar la compra con el usuario
    }

    public class CompraProducto
    {
        public long ProductoId { get; set; } // ID del producto
        public string Nombre { get; set; }
        public int Cantidad { get; set; } // Cantidad de este producto
        public decimal PrecioUnitario { get; set; } // Precio por unidad
        public string ImagenUrl { get; set; }
    }
}
