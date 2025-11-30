using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using espacioProductos;

namespace tl2_tp8_2025_LucasFR_TH.Controllers;
public class ProductosController : Controller
{
    // ========== DEPENDENCIAS INYECTADAS ==========
    // Recibidas en constructor por ASP.NET Core DI Container (Program.cs)
    // Marcadas como readonly para prevenir modificaciones accidentales
    
    private readonly IProductoRepository productoRepository;       // CRUD de productos en BD
    private readonly IAuthenticationService authenticationService; // Autenticación/Autorización

    /// <summary>
    /// Constructor con Inyección de Dependencias.
    /// 
    /// ASP.NET Core resuelve automáticamente estas dependencias del contenedor DI
    /// 
    /// PARÁMETROS:
    /// - productoRepository: Acceso a datos de productos en SQLite
    /// - authenticationService: Verificación de permisos y sesión del usuario
    /// </summary>
    public ProductosController(IProductoRepository productoRepository, IAuthenticationService authenticationService)
    {
        this.productoRepository = productoRepository;
        this.authenticationService = authenticationService;
    }

    // ============================================
    // ACCIÓN: Index (GET) - Listar todos los productos
    // ============================================
    /// <summary>
    /// PROPÓSITO: Mostrar listado de todos los productos disponibles
    /// 
    /// SEGURIDAD:
    /// - Requiere: Autenticado + Rol Administrador
    /// - Si falla: Redirección al login o AccesoDenegado
    /// </summary>
    /// 
    /// Al inicio de TODAS las acciones de ProductosController (Index, Details, Create, Edit, Delete), aplique la siguiente lógica: 
    /// 1. Si !_authService.IsAuthenticated(), redirigir a /Login/Index. 
    /// 2. Si !_authService.HasAccessLevel("Administrador"), redirigir a Productos/AccesoDenegado.
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

    private IActionResult CheckAdminPermissions()
    {
        // ⭐ CHEQUEO 1: ¿Está autenticado?
        // IsAuthenticated() verifica si existe "UsuarioAutenticado" en la sesión
        if (!authenticationService.IsAuthenticated())
        {
            // NO está logueado → Redirigir al login
            // El usuario deberá ingresar sus credenciales
            return RedirectToAction("Index", "Login");
        }

        // ⭐ CHEQUEO 2: ¿Es Administrador?
        // HasAccessLevel() compara el rol en sesión con la cadena requerida
        if (!authenticationService.HasAccessLevel("Administrador"))
        {
            // Está logueado PERO no es Administrador → Acceso denegado
            // Redirige a vista informativa que explica la restricción
            return RedirectToAction(nameof(AccesoDenegado));
        }

        // ✅ PASÓ TODOS LOS CHEQUEOS
        // Retornar null indica que el chequeo fue exitoso
        // El método que lo llamó debe continuar su lógica
        return null;
    }

    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        // El usuario está logueado, pero no tiene el rol suficiente
        return View();
    }
}
