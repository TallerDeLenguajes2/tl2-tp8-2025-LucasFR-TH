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
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(string nombreDestinatario, int[] productoIds, int[] cantidades)
    {
        var presupuesto = new Presupuesto
        {
            nombreDestinatario = nombreDestinatario,
            fechaCreacion = DateTime.Now,
            detalle = new List<PresupuestoDetalle>()
        };

        if (productoIds != null && cantidades != null)
        {
            for (int i = 0; i < productoIds.Length && i < cantidades.Length; i++)
            {
                var pd = new PresupuestoDetalle
                {
                    producto = new Producto { idProducto = productoIds[i] },
                    cantidad = cantidades[i]
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
        return View(presupuesto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, string nombreDestinatario, int[] productoIds, int[] cantidades)
    {
        var presupuesto = new Presupuesto
        {
            idPresupuesto = id,
            nombreDestinatario = nombreDestinatario,
            fechaCreacion = DateTime.Now,
            detalle = new List<PresupuestoDetalle>()
        };

        if (productoIds != null && cantidades != null)
        {
            for (int i = 0; i < productoIds.Length && i < cantidades.Length; i++)
            {
                var pd = new PresupuestoDetalle
                {
                    producto = new Producto { idProducto = productoIds[i] },
                    cantidad = cantidades[i]
                };
                presupuesto.detalle.Add(pd);
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
