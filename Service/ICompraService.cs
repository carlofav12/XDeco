using System.Collections.Generic;
using XDeco.ViewModel;

public interface ICompraService
{
    void AgregarCompra(CompraViewModel compra);
    List<CompraViewModel> ObtenerCompras();
}

public class CompraService : ICompraService
{
    private static List<CompraViewModel> _compras = new List<CompraViewModel>();

    public void AgregarCompra(CompraViewModel compra)
    {
        _compras.Add(compra);
    }

    public List<CompraViewModel> ObtenerCompras()
    {
        return _compras;
    }
}