namespace espacioUsuarios;

/// <summary>
/// Modelo que representa un usuario del sistema.
/// Contiene información de credenciales y autorización.
/// </summary>
public class Usuario
{
    /// <summary>
    /// ID único del usuario en la base de datos.
    /// </summary>
    public int IdUsuario { get; set; }

    /// <summary>
    /// Nombre de usuario para login (único).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario (idealmente hasheada en producción).
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Rol del usuario que determina permisos de acceso.
    /// Valores: "Administrador" o "Cliente"
    /// </summary>
    public string Rol { get; set; } = string.Empty;

    /// <summary>
    /// Constructor por defecto.
    /// </summary>
    public Usuario()
    {
    }

    /// <summary>
    /// Constructor con parámetros.
    /// </summary>
    public Usuario(int id, string username, string password, string rol)
    {
        IdUsuario = id;
        Username = username;
        Password = password;
        Rol = rol;
    }
}
