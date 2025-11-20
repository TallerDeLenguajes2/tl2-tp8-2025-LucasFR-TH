using espacioUsuarios;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace servicioAutenticacion;

/// <summary>
/// SERVICIO DE AUTENTICACIÓN - Implementación de IAuthenticationService
/// 
/// RESPONSABILIDAD: Centralizar toda la lógica de autenticación y autorización del sistema.
/// 
/// TECNOLOGÍA: Sesiones de ASP.NET Core + HttpContext
/// - Las sesiones se almacenan en la cookie ASPNETCORE_SESSION
/// - Cada variable de sesión es accedida a través del IHttpContextAccessor
/// - Las sesiones expiran después de 30 minutos de inactividad (configurado en Program.cs)
/// 
/// FLUJO DE AUTENTICACIÓN:
/// 1. Usuario ingresa credenciales en el LoginController
/// 2. LoginController llama a authService.Login(username, password)
/// 3. AuthenticationService consulta UserRepository.GetUser()
/// 4. Si el usuario existe, se guardan sus datos en la sesión
/// 5. En cada request subsecuente, se verifica IsAuthenticated() y HasAccessLevel()
/// 
/// SEGURIDAD:
/// - Las credenciales NO se almacenan en sesión (solo username y rol)
/// - Las consultas a BD usan parametrización (previene SQL injection)
/// - HttpOnly cookie previene acceso desde JavaScript (XSS protection)
/// - Las variables de sesión se limpian completamente en Logout()
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUserRepository userRepository;

    // ========== CLAVES DE SESIÓN ==========
    // Estas constantes definen los nombres de las variables almacenadas en la sesión.
    // IMPORTANTE: Deben ser consistentes en toda la aplicación.
    private const string USUARIO_SESSION_KEY = "UsuarioAutenticado";   // Almacena el username
    private const string ROL_SESSION_KEY = "RolUsuario";               // Almacena el rol (Admin/Cliente)
    private const string ID_USUARIO_SESSION_KEY = "IdUsuario";         // Almacena el ID del usuario

    /// <summary>
    /// Constructor con Inyección de Dependencias.
    /// 
    /// PARÁMETROS:
    /// - httpContextAccessor: Para acceder a la sesión del request actual
    /// - userRepository: Para consultar datos de usuarios en la BD
    /// </summary>
    public AuthenticationService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.userRepository = userRepository;
    }

    /// <summary>
    /// MÉTODO: Login
    /// 
    /// PROPÓSITO: Autenticar un usuario y crear su sesión.
    /// 
    /// FLUJO:
    /// 1. Consultar BD por credenciales (usuario + contraseña)
    /// 2. Si no existe usuario, retornar false
    /// 3. Si existe, guardar datos en sesión y retornar true
    /// 
    /// DATOS GUARDADOS EN SESIÓN:
    /// - USUARIO_SESSION_KEY: nombre de usuario (para identificación)
    /// - ROL_SESSION_KEY: rol del usuario (para autorización)
    /// - ID_USUARIO_SESSION_KEY: ID interno (para referencias futuras)
    /// 
    /// RETORNA: true si el login fue exitoso, false si falló
    /// </summary>
    public bool Login(string username, string password)
    {
        // PASO 1: Intentar obtener el usuario de la BD
        // GetUser() ejecuta una consulta SQL con parámetros (segura contra SQL injection)
        var usuario = userRepository.GetUser(username, password);

        // PASO 2: Si el usuario no existe, retornar false
        if (usuario == null)
            return false;

        // PASO 3: Obtener la sesión del contexto HTTP actual
        // Nota: ? (null-coalescing) devuelve null si HttpContext es null
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return false;

        // PASO 4: Guardar información del usuario en sesión
        // SetString() almacena valores string en la cookie de sesión
        session.SetString(USUARIO_SESSION_KEY, usuario.Username);
        session.SetString(ROL_SESSION_KEY, usuario.Rol);
        // SetInt32() almacena valores int
        session.SetInt32(ID_USUARIO_SESSION_KEY, usuario.IdUsuario);

        // PASO 5: Login exitoso
        return true;
    }

    /// <summary>
    /// MÉTODO: Logout
    /// 
    /// PROPÓSITO: Cerrar la sesión del usuario actual.
    /// 
    /// FUNCIONALIDAD:
    /// - Elimina TODAS las variables de sesión relacionadas con el usuario
    /// - Limpia completamente los datos de autenticación
    /// - Après Logout, IsAuthenticated() retornará false
    /// 
    /// SEGURIDAD: No deja rastro de datos del usuario anterior
    /// </summary>
    public void Logout()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return;

        // Limpiar todas las variables de sesión relacionadas con autenticación
        // Remove() elimina la clave especificada de la sesión
        session.Remove(USUARIO_SESSION_KEY);
        session.Remove(ROL_SESSION_KEY);
        session.Remove(ID_USUARIO_SESSION_KEY);
    }

    /// <summary>
    /// MÉTODO: IsAuthenticated
    /// 
    /// PROPÓSITO: Verificar si hay un usuario actualmente autenticado.
    /// 
    /// LÓGICA:
    /// - Lee la sesión para buscar USUARIO_SESSION_KEY
    /// - Si existe y NO es vacío → usuario autenticado (retorna true)
    /// - Si no existe o es vacío → usuario NO autenticado (retorna false)
    /// 
    /// USO EN CONTROLADORES:
    /// if (!_authService.IsAuthenticated()) 
    ///     return RedirectToAction("Index", "Login");
    /// 
    /// RETORNA: true si hay usuario autenticado, false si no
    /// </summary>
    public bool IsAuthenticated()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return false;

        // GetString() obtiene un valor string de la sesión
        // Si la clave no existe, devuelve null
        string usuario = session.GetString(USUARIO_SESSION_KEY);
        return !string.IsNullOrEmpty(usuario);
    }

    /// <summary>
    /// MÉTODO: GetCurrentUser
    /// 
    /// PROPÓSITO: Obtener el objeto Usuario completo del usuario autenticado.
    /// 
    /// FLUJO:
    /// 1. Verificar que hay sesión válida
    /// 2. Obtener el username de la sesión
    /// 3. Consultar BD por el usuario completo
    /// 4. Retornar objeto Usuario (con todos sus atributos)
    /// 
    /// USO: Cuando necesitas acceder a propiedades del usuario (no solo el nombre/rol)
    /// 
    /// RETORNA: Objeto Usuario si está autenticado, null si no
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
        // GetByUsername() ejecuta una consulta SQL parametrizada
        return userRepository.GetByUsername(username);
    }

    /// <summary>
    /// MÉTODO: HasAccessLevel
    /// 
    /// PROPÓSITO: Verificar si el usuario actual tiene un rol específico.
    /// 
    /// PARÁMETRO:
    /// - requiredAccessLevel: El rol requerido (ej: "Administrador", "Cliente")
    /// 
    /// LÓGICA:
    /// 1. Verificar si está autenticado
    /// 2. Si no está autenticado, retornar false
    /// 3. Si está autenticado, comparar su rol con el requerido
    /// 4. Retornar true si coinciden, false si no
    /// 
    /// USO EN CONTROLADORES:
    /// if (!_authService.HasAccessLevel("Administrador")) 
    ///     return RedirectToAction("AccesoDenegado");
    /// 
    /// RETORNA: true si el usuario tiene el rol requerido, false si no
    /// </summary>
    public bool HasAccessLevel(string requiredAccessLevel)
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return false;

        // VALIDACIÓN CRÍTICA: Si no está autenticado, NO tiene acceso
        // Esto previene que usuarios no autenticados pasen el control
        if (!IsAuthenticated())
            return false;

        // Obtener el rol del usuario de la sesión
        string userRole = session.GetString(ROL_SESSION_KEY);
        
        // COMPARACIÓN: El rol debe coincidir EXACTAMENTE
        // Ejemplo: "Administrador" == "Administrador" → true
        //          "Cliente" == "Administrador" → false
        return userRole == requiredAccessLevel;
    }

    /// <summary>
    /// MÉTODO: GetCurrentUserRole
    /// 
    /// PROPÓSITO: Obtener el rol del usuario actualmente autenticado.
    /// 
    /// RETORNA: String con el rol (ej: "Administrador", "Cliente")
    ///          String vacío si no está autenticado
    /// 
    /// USO: Para mostrar el rol en la UI, en logs, etc.
    /// </summary>
    public string GetCurrentUserRole()
    {
        var session = httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return string.Empty;

        string role = session.GetString(ROL_SESSION_KEY);
        return role ?? string.Empty;  // Retorna el rol o string.Empty si es null
    }
}
