using System.Collections.Generic;
using XDeco.ViewModel;

public interface ICompraService
{
    void AgregarCompra(CompraViewModel compra);
    List<CompraViewModel> ObtenerCompras();
    List<CompraViewModel> ObtenerComprasPorUsuario(string userId);
}

public class CompraService : ICompraService
{
    private static long _nextCompraId = 1; // Variable para llevar el conteo de IDs de compras
    private static List<CompraViewModel> _compras = new List<CompraViewModel>();

    public void AgregarCompra(CompraViewModel compra)
    {
        // Asigna un ID único a la compra
        compra.Id = _nextCompraId++;
        _compras.Add(compra);
    }

    public List<CompraViewModel> ObtenerCompras()
    {
        return _compras;
    }

    public List<CompraViewModel> ObtenerComprasPorUsuario(string userId)
    {
        // Create a new list to hold the purchases for the specified user
        List<CompraViewModel> comprasPorUsuario = new List<CompraViewModel>();

        // Iterate through the list of all purchases
        foreach (var compra in _compras)
        {
            // Assuming CompraViewModel has a property called UsuarioId to check against
            if (compra.UsuarioId == userId)
            {
                comprasPorUsuario.Add(compra); // Add the purchase to the user's list
            }
        }

        return comprasPorUsuario; // Return the filtered list
    }

}
