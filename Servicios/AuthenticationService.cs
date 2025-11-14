using espacioUsuarios;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace servicioAutenticacion;

/// <summary>
/// Servicio de autenticación que utiliza sesiones de ASP.NET Core.
/// Maneja login, logout y verificación de permisos basados en roles.
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUserRepository userRepository;

    // Claves de sesión
    private const string USUARIO_SESSION_KEY = "UsuarioAutenticado";
    private const string ROL_SESSION_KEY = "RolUsuario";
    private const string ID_USUARIO_SESSION_KEY = "IdUsuario";

    public AuthenticationService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.userRepository = userRepository;
    }

    /// <summary>
    /// Intenta autenticar un usuario y establece la sesión.
    /// </summary>
    public bool Login(string username, string password)
    {
        var usuario = userRepository.GetUser(username, password);

        if (usuario == null)
            return false;

        // Obtener la sesión del contexto HTTP actual
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return false;

        // Guardar información del usuario en sesión
        session.SetString(USUARIO_SESSION_KEY, usuario.Username);
        session.SetString(ROL_SESSION_KEY, usuario.Rol);
        session.SetInt32(ID_USUARIO_SESSION_KEY, usuario.IdUsuario);

        return true;
    }

    /// <summary>
    /// Cierra la sesión del usuario actual.
    /// </summary>
    public void Logout()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return;

        // Limpiar todas las variables de sesión relacionadas con autenticación
        session.Remove(USUARIO_SESSION_KEY);
        session.Remove(ROL_SESSION_KEY);
        session.Remove(ID_USUARIO_SESSION_KEY);
    }

    /// <summary>
    /// Verifica si hay un usuario actualmente autenticado.
    /// </summary>
    public bool IsAuthenticated()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return false;

        // Si el usuario existe en sesión, está autenticado
        string usuario = session.GetString(USUARIO_SESSION_KEY);
        return !string.IsNullOrEmpty(usuario);
    }

    /// <summary>
    /// Obtiene el usuario actualmente autenticado.
    /// </summary>
    public Usuario GetCurrentUser()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return null;

        string username = session.GetString(USUARIO_SESSION_KEY);
        if (string.IsNullOrEmpty(username))
            return null;

        // Retornar objeto Usuario recuperado del repositorio
        return userRepository.GetByUsername(username);
    }

    /// <summary>
    /// Verifica si el usuario actual tiene un rol específico.
    /// </summary>
    public bool HasAccessLevel(string requiredAccessLevel)
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return false;

        // Si no está autenticado, no tiene acceso
        if (!IsAuthenticated())
            return false;

        string userRole = session.GetString(ROL_SESSION_KEY);
        return userRole == requiredAccessLevel;
    }

    /// <summary>
    /// Obtiene el rol del usuario actualmente autenticado.
    /// </summary>
    public string GetCurrentUserRole()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return string.Empty;

        string role = session.GetString(ROL_SESSION_KEY);
        return role ?? string.Empty;
    }
}
