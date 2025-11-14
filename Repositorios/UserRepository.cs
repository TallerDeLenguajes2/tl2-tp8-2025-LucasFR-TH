using espacioUsuarios;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace repositorioUsuario;

/// <summary>
/// Implementación del repositorio de Usuarios.
/// Lee datos desde la base de datos SQLite.
/// </summary>
public class UserRepository : IUserRepository
{
    private string cadenaConexion = "Data Source = Db/Tienda.db";

    /// <summary>
    /// Obtiene un usuario mediante credenciales de login.
    /// </summary>
    public Usuario GetUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return null;

        string query = "SELECT Id, Nombre, User, Pass, Rol FROM usuarios WHERE User = @username AND Pass = @password LIMIT 1";

        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();

        var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter("@username", username));
        command.Parameters.Add(new SqliteParameter("@password", password));

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var usuario = new Usuario
            {
                IdUsuario = Convert.ToInt32(reader["Id"]),
                Username = reader["User"].ToString() ?? string.Empty,
                Password = reader["Pass"].ToString() ?? string.Empty,
                Rol = reader["Rol"].ToString() ?? string.Empty
            };

            return usuario;
        }

        return null;
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    public Usuario GetById(int id)
    {
        string query = "SELECT Id, Nombre, User, Pass, Rol FROM usuarios WHERE Id = @id LIMIT 1";

        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();

        var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter("@id", id));

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var usuario = new Usuario
            {
                IdUsuario = Convert.ToInt32(reader["Id"]),
                Username = reader["User"].ToString() ?? string.Empty,
                Password = reader["Pass"].ToString() ?? string.Empty,
                Rol = reader["Rol"].ToString() ?? string.Empty
            };

            return usuario;
        }

        return null;
    }

    /// <summary>
    /// Obtiene un usuario por su nombre de usuario.
    /// </summary>
    public Usuario GetByUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
            return null;

        string query = "SELECT Id, Nombre, User, Pass, Rol FROM usuarios WHERE User = @username LIMIT 1";

        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();

        var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter("@username", username));

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var usuario = new Usuario
            {
                IdUsuario = Convert.ToInt32(reader["Id"]),
                Username = reader["User"].ToString() ?? string.Empty,
                Password = reader["Pass"].ToString() ?? string.Empty,
                Rol = reader["Rol"].ToString() ?? string.Empty
            };

            return usuario;
        }

        return null;
    }
}
