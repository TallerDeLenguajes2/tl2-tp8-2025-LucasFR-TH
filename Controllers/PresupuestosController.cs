using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using espacioPresupuestos;
using espacioProductos;
using espacioPresupuestoDetalle;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace tl2_tp8_2025_LucasFR_TH.Controllers;

/// <summary>
/// Controlador para gestionar Presupuestos.
/// Implementa patrón de inyección de dependencias.
/// </summary>
public class PresupuestosController : Controller
{
    private readonly IPresupuestoRepository presupuestosRepository;
    private readonly IProductoRepository productoRepository;
    private readonly IAuthenticationService authenticationService;

    /// <summary>
    /// Constructor con inyección de dependencias.
    /// </summary>
    public PresupuestosController(
        IPresupuestoRepository presupuestosRepository,
        IProductoRepository productoRepository,
        IAuthenticationService authenticationService)
    {
        this.presupuestosRepository = presupuestosRepository;
        this.productoRepository = productoRepository;
        this.authenticationService = authenticationService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        // Aplicamos el chequeo de permisos para crear (solo Administrador)
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        // traer productos para selector
        ViewBag.Productos = productoRepository.GetAll();
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
        // Aplicamos el chequeo de permisos para crear (solo Administrador)
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        if (!ModelState.IsValid)
        {
            // repoblar select lists inside Productos entries if needed
            ViewBag.Productos = productoRepository.GetAll();
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
                    producto = new Producto { idProducto = p.IdProducto },
                    cantidad = p.Cantidad
                };
                presupuesto.detalle.Add(pd);
            }
        }

        presupuestosRepository.Create(presupuesto);
        return RedirectToAction(nameof(Index));
    }

    // ==================== TAREA 3.1: Agregar Producto a Presupuesto ====================
    
    // GET: Presupuestos/AgregarProducto
    [HttpGet]
    public IActionResult AgregarProducto(int id)
    {
        // Aplicamos el chequeo de permisos para agregar (solo Administrador)
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        // 1. Obtener los productos para el SelectList
        List<Producto> productos = productoRepository.GetAll();
        
        // 2. Crear el ViewModel
        ViewModels.AgregarProductoViewModel model = new ViewModels.AgregarProductoViewModel
        {
            IdPresupuesto = id,  // Pasamos el ID del presupuesto actual
            // 3. Crear el SelectList
            ListaProductos = new SelectList(productos, "idProducto", "descripcion")
        };
        
        return View(model);
    }

    // POST: Presupuestos/AgregarProducto
    // ❗ El Método CLAVE para la validación de la cantidad
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AgregarProducto(ViewModels.AgregarProductoViewModel model)
    {
        // Aplicamos el chequeo de permisos para agregar (solo Administrador)
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        // 1. Chequeo de Seguridad para la Cantidad
        if (!ModelState.IsValid)
        {
            // LÓGICA CRÍTICA DE RECARGA: Si falla la validación,
            // debemos recargar el SelectList porque se pierde en el POST.
            var productos = productoRepository.GetAll();
            model.ListaProductos = new SelectList(productos, "idProducto", "descripcion");
            
            // Devolvemos el modelo con los errores y el dropdown recargado
            return View(model);
        }

        // 2. Si es VÁLIDO: Llamamos al repositorio para guardar la relación
        presupuestosRepository.AddProducto(model.IdPresupuesto, model.IdProducto, model.Cantidad);

        // 3. Redirigimos al detalle del presupuesto
        return RedirectToAction(nameof(Details), new { id = model.IdPresupuesto });
    }

    // ==================== Fin de Tarea 3.1 ====================

    [HttpGet]
    public IActionResult Edit(int id)
    {
        // Aplicamos el chequeo de permisos para editar (solo Administrador)
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        var presupuesto = presupuestosRepository.GetById(id);
        if (presupuesto == null) return NotFound();

        ViewBag.Productos = productoRepository.GetAll();

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
                    IdProducto = d.producto?.idProducto ?? 0,
                    Cantidad = d.cantidad,
                    ListaProductos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(productoRepository.GetAll(), "idProducto", "descripcion", d.producto?.idProducto)
                });
            }
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ViewModels.PresupuestoViewModel viewModel)
    {
        // Aplicamos el chequeo de permisos para editar (solo Administrador)
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        if (!ModelState.IsValid)
        {
            // repoblar dropdowns for the form before returning
            ViewBag.Productos = productoRepository.GetAll();
            if (viewModel.Productos != null)
            {
                foreach (var p in viewModel.Productos)
                {
                    p.ListaProductos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(productoRepository.GetAll(), "idProducto", "descripcion", p.IdProducto);
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
                    producto = new Producto { idProducto = p.IdProducto },
                    cantidad = p.Cantidad
                });
            }
        }

        presupuestosRepository.Update(id, presupuesto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        // Aplicamos el chequeo de permisos para eliminar (solo Administrador)
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        var presupuesto = presupuestosRepository.GetById(id);
        if (presupuesto == null) return NotFound();
        return View(presupuesto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        // Aplicamos el chequeo de permisos para eliminar (solo Administrador)
        var securityCheck = CheckAdminPermissions();
        if (securityCheck != null) return securityCheck;

        presupuestosRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Index()
    {
        // Comprobación de si está logueado
        if (!authenticationService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        // Verifica Nivel de acceso que necesite validar
        if (authenticationService.HasAccessLevel("Administrador") || authenticationService.HasAccessLevel("Cliente"))
        {
            // Si es válido entra sino vuelve a AccesoDenegado
            List<Presupuesto> presupuestos = presupuestosRepository.GetAll();
            return View(presupuestos);
        }
        else
        {
            return RedirectToAction(nameof(AccesoDenegado));
        }
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        // Comprobación de si está logueado
        if (!authenticationService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Login");
        }

        // Verifica Nivel de acceso que necesite validar
        if (!(authenticationService.HasAccessLevel("Administrador") || authenticationService.HasAccessLevel("Cliente")))
        {
            return RedirectToAction(nameof(AccesoDenegado));
        }

        var presupuesto = presupuestosRepository.GetById(id);
        if (presupuesto == null)
        {
            return NotFound();
        }

        return View(presupuesto);
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
            // Llamamos a AccesoDenegado (llama a la vista correspondiente de Presupuestos)
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
