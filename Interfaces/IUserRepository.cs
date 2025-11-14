using espacioUsuarios;

namespace tl2_tp8_2025_LucasFR_TH.Interfaces;

/// <summary>
/// Interfaz de abstracción para el repositorio de Usuarios.
/// Proporciona métodos de acceso a datos relacionados con autenticación.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Obtiene un usuario mediante sus credenciales de login.
    /// Valida username y password.
    /// </summary>
    /// <param name="username">Nombre de usuario.</param>
    /// <param name="password">Contraseña del usuario.</param>
    /// <returns>Objeto Usuario si las credenciales son válidas, null si no.</returns>
    Usuario GetUser(string username, string password);

    /// <summary>
    /// Obtiene un usuario específico por su ID.
    /// </summary>
    /// <param name="id">ID del usuario.</param>
    /// <returns>Usuario si existe, null si no.</returns>
    Usuario GetById(int id);

    /// <summary>
    /// Obtiene un usuario por su nombre de usuario.
    /// </summary>
    /// <param name="username">Nombre de usuario.</param>
    /// <returns>Usuario si existe, null si no.</returns>
    Usuario GetByUsername(string username);
}
