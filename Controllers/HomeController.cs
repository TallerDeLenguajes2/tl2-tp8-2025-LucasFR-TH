using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_LucasFR_TH.Models;

namespace tl2_tp8_2025_LucasFR_TH.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Acción que se ejecuta cuando un usuario autenticado intenta acceder a un recurso sin permisos.
    /// </summary>
    public IActionResult AccesoDenegado()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
