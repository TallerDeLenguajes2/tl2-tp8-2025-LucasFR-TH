using espacioUsuarios;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using System.Collections.Generic;

namespace repositorioUsuario;

/// <summary>
/// Implementación del repositorio de Usuarios.
/// Actualmente usa datos hardcodeados, puede ser modificado para usar BD.
/// </summary>
public class UserRepository : IUserRepository
{
    // Lista de usuarios precargados (en producción vendrían de BD)
    private static readonly List<Usuario> usuarios = new()
    {
        new Usuario(1, "admin", "admin123", "Administrador"),
        new Usuario(2, "cliente1", "pass123", "Cliente"),
        new Usuario(3, "cliente2", "pass456", "Cliente")
    };

    /// <summary>
    /// Obtiene un usuario mediante credenciales de login.
    /// </summary>
    public Usuario GetUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return null;

        // Busca el usuario que coincida con username y password
        var usuario = usuarios.FirstOrDefault(u =>
            u.Username == username && u.Password == password);

        return usuario;
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    public Usuario GetById(int id)
    {
        return usuarios.FirstOrDefault(u => u.IdUsuario == id);
    }

    /// <summary>
    /// Obtiene un usuario por su nombre de usuario.
    /// </summary>
    public Usuario GetByUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
            return null;

        return usuarios.FirstOrDefault(u => u.Username == username);
    }
}
