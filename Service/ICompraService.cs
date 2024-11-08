using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using XDeco.Models;
using XDeco.ViewModel;

public interface ICompraService
{
    void AgregarCompra(Compra compra);
    List<Compra> ObtenerCompras();
    List<Compra> ObtenerComprasPorUsuario(string userId);
}

public class CompraService : ICompraService
{
    private readonly ApplicationDbContext _context;

    public CompraService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AgregarCompra(Compra compra)
    {
        // Agregar la compra a la base de datos
        _context.Compras.Add(compra);
        _context.SaveChanges();
    }

    public List<Compra> ObtenerCompras()
    {
        // Obtener todas las compras de la base de datos, incluyendo los productos de la compra
        return _context.Compras
            .Include(c => c.CompraProductos)
            .ThenInclude(cp => cp.Producto)
            .ToList();
    }

    public List<Compra> ObtenerComprasPorUsuario(string userId)
    {
        // Obtener todas las compras de un usuario específico desde la base de datos
        return _context.Compras
            .Where(c => c.UsuarioId == userId)
            .Include(c => c.CompraProductos)
            .ThenInclude(cp => cp.Producto)
            .ToList();
    }
}

