using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using tl2_tp8_2025_LucasFR_TH.ViewModels;

namespace tl2_tp8_2025_LucasFR_TH.Controllers;

/// <summary>
/// Controlador encargado de manejar la autenticación de usuarios.
/// Gestiona el login y logout del sistema.
/// </summary>
public class LoginController : Controller
{
    private readonly IAuthenticationService authenticationService;

    public LoginController(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    /// <summary>
    /// GET: /Login/Index
    /// Muestra el formulario de login.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        // Si el usuario ya está autenticado, redirige a Home
        if (authenticationService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel());
    }

    /// <summary>
    /// POST: /Login/Index
    /// Procesa el login del usuario.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(LoginViewModel model)
    {
        // Validar el modelo
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Intentar autenticar al usuario
        bool loginExitoso = authenticationService.Login(model.Username, model.Password);

        if (loginExitoso)
        {
            // Login exitoso: redirigir a Home
            return RedirectToAction("Index", "Home");
        }

        // Login fallido: mostrar error
        ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
        return View(model);
    }

    /// <summary>
    /// GET: /Login/Logout
    /// Cierra la sesión del usuario actual.
    /// </summary>
    [HttpGet]
    public IActionResult Logout()
    {
        authenticationService.Logout();
        return RedirectToAction("Index");
    }
}
