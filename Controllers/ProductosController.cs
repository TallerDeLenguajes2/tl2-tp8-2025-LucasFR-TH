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
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var producto = new espacioProductos.Producto
        {
            descripcion = viewModel.descripcion ?? string.Empty,
            precio = viewModel.precio
        };

        productoRepository.Create(producto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var producto = productoRepository.GetById(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ViewModels.ProductoViewModel viewModel)
    {
        if (id != viewModel.idProducto) return BadRequest();
        if (!ModelState.IsValid) return View(viewModel);

        var producto = new espacioProductos.Producto
        {
            idProducto = viewModel.idProducto,
            descripcion = viewModel.descripcion ?? string.Empty,
            precio = viewModel.precio
        };

        productoRepository.Update(id, producto);
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
