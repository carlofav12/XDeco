using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XDeco.Models;

public class ComprasModel : PageModel
{
    private readonly UserManager<Usuario> _userManager;
    private readonly ICompraService _compraService;

    public ComprasModel(UserManager<Usuario> userManager, ICompraService compraService)
    {
        _userManager = userManager;
        _compraService = compraService;
    }

    // Lista de compras que se mostrarán en la vista
    public List<Compra> Compras { get; set; } = new List<Compra>();

    // Acción para obtener las compras del usuario
    public async Task<IActionResult> OnGetAsync()
    {
        // Obtener el ID del usuario actual
        var userId = _userManager.GetUserId(User);
        
        // Obtener las compras del usuario usando el servicio
        Compras = _compraService.ObtenerComprasPorUsuario(userId);
        
        return Page(); // Renderiza la vista con los datos de compras
    }
}


