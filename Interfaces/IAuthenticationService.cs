using espacioUsuarios;

namespace tl2_tp8_2025_LucasFR_TH.Interfaces;

/// <summary>
/// INTERFAZ DE SERVICIO DE AUTENTICACIÓN - Contrato para gestión de sesiones y autorización
/// 
/// RESPONSABILIDAD: Definir métodos que centraliza la lógica de autenticación
/// 
/// IMPLEMENTACIÓN: servicioAutenticacion.AuthenticationService
/// 
/// USO: Se inyecta en ProductosController, PresupuestosController y LoginController
/// 
/// SEGURIDAD: Este patrón permite cambiar la implementación sin afectar los controladores
/// (P. ej., pasar de sesiones a JWT tokens sin cambiar controladores)
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// MÉTODO: Login
    /// 
    /// PROPÓSITO: Autenticar un usuario y establecer su sesión
    /// 
    /// PARÁMETROS:
    /// - username: Nombre de usuario
    /// - password: Contraseña del usuario
    /// 
    /// FLUJO:
    /// 1. Consultar BD por credenciales
    /// 2. Si existen → Guardar datos en sesión y retornar true
    /// 3. Si no existen → Retornar false
    /// 
    /// DATOS ALMACENADOS EN SESIÓN:
    /// - Username
    /// - Rol (Administrador/Cliente)
    /// - ID del usuario
    /// 
    /// RETORNA: true si login exitoso, false si falló
    /// </summary>
    bool Login(string username, string password);

    /// <summary>
    /// MÉTODO: Logout
    /// 
    /// PROPÓSITO: Cerrar la sesión del usuario actual
    /// 
    /// FUNCIONALIDAD:
    /// - Elimina TODAS las variables de sesión
    /// - Limpia completamente los datos de autenticación
    /// - Après Logout, IsAuthenticated() retornará false
    /// </summary>
    void Logout();

    /// <summary>
    /// MÉTODO: IsAuthenticated
    /// 
    /// PROPÓSITO: Verificar si existe un usuario autenticado en la sesión actual
    /// 
    /// LÓGICA:
    /// - Lee la sesión buscando el username
    /// - SI existe → Retorna true
    /// - SI no existe → Retorna false
    /// 
    /// USO EN CONTROLADORES:
    /// if (!_authService.IsAuthenticated()) 
    ///     return RedirectToAction("Index", "Login");
    /// 
    /// RETORNA: true si hay sesión válida, false si no
    /// </summary>
    bool IsAuthenticated();

    /// <summary>
    /// MÉTODO: GetCurrentUser
    /// 
    /// PROPÓSITO: Obtener el objeto Usuario completo del usuario autenticado
    /// 
    /// RETORNA: Objeto Usuario (con todos sus atributos) si está autenticado
    ///          null si no hay usuario autenticado
    /// 
    /// USO: Cuando necesitas acceso a propiedades del usuario (ID, nombre, etc.)
    /// </summary>
    Usuario GetCurrentUser();

    /// <summary>
    /// MÉTODO: HasAccessLevel
    /// 
    /// PROPÓSITO: Verificar si el usuario actual tiene un rol específico
    /// 
    /// PARÁMETRO:
    /// - requiredAccessLevel: El rol a verificar (ej: "Administrador", "Cliente")
    /// 
    /// LÓGICA:
    /// 1. Verificar si IsAuthenticated()
    /// 2. Si SÍ → Comparar rol del usuario con el requerido
    /// 3. Si NO → Retornar false
    /// 
    /// USO EN CONTROLADORES:
    /// if (!_authService.HasAccessLevel("Administrador")) 
    ///     return RedirectToAction("AccesoDenegado");
    /// 
    /// RETORNA: true si el usuario tiene el rol, false si no
    /// 
    /// NOTA: La comparación es EXACTA (case-sensitive)
    /// </summary>
    bool HasAccessLevel(string requiredAccessLevel);

    /// <summary>
    /// MÉTODO: GetCurrentUserRole
    /// 
    /// PROPÓSITO: Obtener el rol del usuario autenticado
    /// 
    /// RETORNA: String con el rol (ej: "Administrador", "Cliente")
    ///          String.Empty si no está autenticado
    /// 
    /// USO: Para mostrar el rol en UI, logs, auditoría, etc.
    /// </summary>
    string GetCurrentUserRole();
}
