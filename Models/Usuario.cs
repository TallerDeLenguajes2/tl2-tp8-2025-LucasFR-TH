namespace espacioUsuarios;

/// <summary>
/// Modelo que representa un usuario del sistema.
/// Contiene información de credenciales y autorización.
/// </summary>
public class Usuario
{
    public int IdUsuario { get; set; }
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public Usuario()
    {
    }

    public Usuario(int id, string username, string password, string rol)
    {
        IdUsuario = id;
        Username = username;
        Password = password;
        Rol = rol;
    }
}
