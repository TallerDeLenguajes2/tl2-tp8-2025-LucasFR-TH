using Microsoft.AspNetCore.Mvc;
using repositorioProducto;
using espacioProductos;

namespace tl2_tp8_2025_LucasFR_TH.Controllers;

public class ProductosController : Controller
{
    private ProductoRepository productoRepository;

    public ProductosController()
    {
        productoRepository = new ProductoRepository();
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Producto> productos = productoRepository.GetAll();
        return View(productos);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View(new ViewModels.ProductoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ViewModels.ProductoViewModel viewModel)
    {
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
        var producto = productoRepository.GetById(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        productoRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
