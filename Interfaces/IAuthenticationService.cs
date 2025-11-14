using espacioUsuarios;

namespace tl2_tp8_2025_LucasFR_TH.Interfaces;

/// <summary>
/// Interfaz de servicio de autenticación y autorización.
/// Maneja el flujo de login/logout y control de acceso basado en roles.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Intenta autenticar un usuario con username y password.
    /// Si es exitoso, establece la sesión.
    /// </summary>
    /// <param name="username">Nombre de usuario.</param>
    /// <param name="password">Contraseña del usuario.</param>
    /// <returns>true si la autenticación fue exitosa, false si falló.</returns>
    bool Login(string username, string password);

    /// <summary>
    /// Cierra la sesión del usuario actual.
    /// Limpia todas las variables de sesión relacionadas.
    /// </summary>
    void Logout();

    /// <summary>
    /// Verifica si hay un usuario actualmente autenticado (sesión válida).
    /// </summary>
    /// <returns>true si existe una sesión de usuario válida, false si no.</returns>
    bool IsAuthenticated();

    /// <summary>
    /// Obtiene el usuario actualmente autenticado.
    /// </summary>
    /// <returns>Objeto Usuario si está autenticado, null si no.</returns>
    Usuario GetCurrentUser();

    /// <summary>
    /// Verifica si el usuario actual tiene un rol específico.
    /// </summary>
    /// <param name="requiredAccessLevel">Rol requerido (ej. "Administrador").</param>
    /// <returns>true si el usuario tiene el rol requerido, false si no.</returns>
    bool HasAccessLevel(string requiredAccessLevel);

    /// <summary>
    /// Obtiene el rol del usuario actualmente autenticado.
    /// </summary>
    /// <returns>Rol del usuario o string.Empty si no está autenticado.</returns>
    string GetCurrentUserRole();
}
