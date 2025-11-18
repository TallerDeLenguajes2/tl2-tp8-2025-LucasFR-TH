using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using espacioProductos;

namespace tl2_tp8_2025_LucasFR_TH.Controllers;

/// <summary>
/// Controlador para gestionar Productos.
/// Implementa patrón de inyección de dependencias.
/// </summary>
public class ProductosController : Controller
{
    private readonly IProductoRepository productoRepository;
    private readonly IAuthenticationService authenticationService;

    /// <summary>
    /// Constructor con inyección de dependencias.
    /// </summary>
    public ProductosController(IProductoRepository productoRepository, IAuthenticationService authenticationService)
    {
        this.productoRepository = productoRepository;
        this.authenticationService = authenticationService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        List<Producto> productos = productoRepository.GetAll();
        return View(productos);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        return View(new ViewModels.ProductoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ViewModels.ProductoViewModel viewModel)
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        // 1. CHEQUEO DE SEGURIDAD DEL SERVIDOR
        if (!ModelState.IsValid)
        {
            // Si falla: Devolvemos el ViewModel con los datos y errores a la Vista
            return View(viewModel);
        }

        // 2. SI ES VÁLIDO: Mapeo Manual de VM a Modelo de Dominio
        var nuevoProducto = new espacioProductos.Producto
        {
            descripcion = viewModel.Descripcion ?? string.Empty,
            precio = (int)viewModel.Precio
        };

        // 3. Llamada al Repositorio
        productoRepository.Create(nuevoProducto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        var producto = productoRepository.GetById(id);
        if (producto == null) return NotFound();
        
        // Mapear el modelo de dominio al ViewModel
        var viewModel = new ViewModels.ProductoViewModel
        {
            IdProducto = producto.idProducto,
            Descripcion = producto.descripcion,
            Precio = producto.precio
        };
        
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ViewModels.ProductoViewModel viewModel)
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        // Validar que el ID de la URL coincida con el del ViewModel
        if (id != viewModel.IdProducto) return BadRequest();
        
        // 1. CHEQUEO DE SEGURIDAD DEL SERVIDOR
        if (!ModelState.IsValid)
        {
            // Si falla: Devolvemos el ViewModel con los datos y errores a la Vista
            return View(viewModel);
        }

        // 2. Mapeo Manual de VM a Modelo de Dominio
        var productoAEditar = new espacioProductos.Producto
        {
            idProducto = viewModel.IdProducto,  // Necesario para el UPDATE
            descripcion = viewModel.Descripcion ?? string.Empty,
            precio = (int)viewModel.Precio
        };

        // 3. Llamada al Repositorio
        productoRepository.Update(id, productoAEditar);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        var producto = productoRepository.GetById(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        // Aplicamos el chequeo de seguridad
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        productoRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Método privado que verifica permisos de administrador.
    /// Redirige al login si no está autenticado.
    /// Redirige a AccesoDenegado si no tiene rol de Administrador.
    /// </summary>
    private IActionResult CheckAdminPermissions()
    {
        // 1. No logueado? -> vuelve al login
        if (!authenticationService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        // 2. No es Administrador? -> Da Error
        if (!authenticationService.HasAccessLevel("Administrador"))
        {
            // Llamamos a AccesoDenegado (llama a la vista correspondiente de Productos)
            return RedirectToAction(nameof(AccesoDenegado));
        }

        return null; // Permiso concedido
    }

    /// <summary>
    /// Acción para mostrar página de acceso denegado.
    /// Se utiliza cuando un usuario intenta acceder sin los permisos necesarios.
    /// </summary>
    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        return View();
    }
}
