using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using repositorioPresupuesto;
using espacioPresupuestos;
using repositorioProducto;
using espacioProductos;
using espacioPresupuestoDetalle;

namespace tl2_tp8_2025_LucasFR_TH.Controllers;

public class PresupuestosController : Controller
{
    private PresupuestosRepository presupuestosRepository;

    public PresupuestosController()
    {
        presupuestosRepository = new PresupuestosRepository();
    }

    [HttpGet]
    public IActionResult Create()
    {
        // traer productos para selector
        var prodRepo = new ProductoRepository();
        ViewBag.Productos = prodRepo.GetAll();
        var vm = new ViewModels.PresupuestoViewModel
        {
            FechaCreacion = DateTime.Now
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ViewModels.PresupuestoViewModel viewModel)
    {
        var prodRepo = new ProductoRepository();
        if (!ModelState.IsValid)
        {
            // repoblar select lists inside Productos entries if needed
            ViewBag.Productos = prodRepo.GetAll();
            return View(viewModel);
        }

        var presupuesto = new Presupuesto
        {
            nombreDestinatario = viewModel.NombreDestinatario,
            fechaCreacion = viewModel.FechaCreacion,
            detalle = new List<PresupuestoDetalle>()
        };

        if (viewModel.Productos != null)
        {
            foreach (var p in viewModel.Productos)
            {
                if (p == null) continue;
                var pd = new PresupuestoDetalle
                {
                    producto = new Producto { idProducto = p.productoId },
                    cantidad = p.cantidad
                };
                presupuesto.detalle.Add(pd);
            }
        }

        presupuestosRepository.Create(presupuesto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var presupuesto = presupuestosRepository.GetById(id);
        if (presupuesto == null) return NotFound();

        var prodRepo = new ProductoRepository();
        ViewBag.Productos = prodRepo.GetAll();

        // map to viewmodel
        var vm = new ViewModels.PresupuestoViewModel
        {
            IdPresupuesto = presupuesto.idPresupuesto,
            NombreDestinatario = presupuesto.nombreDestinatario,
            FechaCreacion = presupuesto.fechaCreacion,
            Productos = new List<ViewModels.AgregarProductoViewModel>()
        };

        if (presupuesto.detalle != null)
        {
            foreach (var d in presupuesto.detalle)
            {
                vm.Productos.Add(new ViewModels.AgregarProductoViewModel
                {
                    productoId = d.producto?.idProducto ?? 0,
                    cantidad = d.cantidad,
                    ListaProductos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(prodRepo.GetAll(), "idProducto", "descripcion", d.producto?.idProducto)
                });
            }
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ViewModels.PresupuestoViewModel viewModel)
    {
        var prodRepo = new ProductoRepository();
        if (!ModelState.IsValid)
        {
            // repoblar dropdowns for the form before returning
            ViewBag.Productos = prodRepo.GetAll();
            if (viewModel.Productos != null)
            {
                foreach (var p in viewModel.Productos)
                {
                    p.ListaProductos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(prodRepo.GetAll(), "idProducto", "descripcion", p.productoId);
                }
            }
            return View(viewModel);
        }

        var presupuesto = new Presupuesto
        {
            idPresupuesto = id,
            nombreDestinatario = viewModel.NombreDestinatario,
            fechaCreacion = viewModel.FechaCreacion,
            detalle = new List<PresupuestoDetalle>()
        };

        if (viewModel.Productos != null)
        {
            foreach (var p in viewModel.Productos)
            {
                if (p == null) continue;
                presupuesto.detalle.Add(new PresupuestoDetalle
                {
                    producto = new Producto { idProducto = p.productoId },
                    cantidad = p.cantidad
                });
            }
        }

        presupuestosRepository.Update(id, presupuesto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var presupuesto = presupuestosRepository.GetById(id);
        if (presupuesto == null) return NotFound();
        return View(presupuesto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        presupuestosRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Presupuesto> presupuestos = presupuestosRepository.GetAll();
        return View(presupuestos);
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var presupuesto = presupuestosRepository.GetById(id);
        if (presupuesto == null)
        {
            return NotFound();
        }

        return View(presupuesto);
    }

}
