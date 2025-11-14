namespace repositorioProducto;
using espacioProductos;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using Microsoft.Data.Sqlite;

/// <summary>
/// Implementación de repositorio para Productos.
/// Maneja todas las operaciones CRUD contra la base de datos.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private string cadenaConexion = "Data Source = Db/Tienda.db";

    public List<Producto> GetAll()
    {
        string query = "SELECT * FROM productos";
        List<Producto> productos = [];
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();

        var command = new SqliteCommand(query, connection);

        using (SqliteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var producto = new Producto
                {
                    idProducto = Convert.ToInt32(reader["idProducto"]),
                    descripcion = reader["Descripcion"].ToString(),
                    precio = Convert.ToInt32(reader["Precio"])
                };
                productos.Add(producto);
            }
        }
        connection.Close();
        return productos;
    }

    public Producto Create(Producto producto)
    {
        var query = "INSERT INTO productos (descripcion, precio) VALUES (@descripcion, @precio); SELECT last_insert_rowid();";
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        
        var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter("@descripcion", producto.descripcion));
        command.Parameters.Add(new SqliteParameter("@precio", producto.precio));
        
        producto.idProducto = Convert.ToInt32(command.ExecuteScalar());
        connection.Close();
        
        return producto;
    }

    public Producto GetById(int id)
    {
        var query = "SELECT * FROM productos WHERE idProducto = @id";
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        
        var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter("@id", id));

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            var producto = new Producto
            {
                idProducto = Convert.ToInt32(reader["idProducto"]),
                descripcion = reader["Descripcion"].ToString(),
                precio = Convert.ToInt32(reader["Precio"])
            };
            connection.Close();
            return producto;
        }
        
        connection.Close();
        return null;
    }

    public void Update(int id, Producto producto)
    {
        var query = "UPDATE productos SET descripcion = @descripcion, precio = @precio WHERE idProducto = @id";
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        
        var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter("@descripcion", producto.descripcion));
        command.Parameters.Add(new SqliteParameter("@precio", producto.precio));
        command.Parameters.Add(new SqliteParameter("@id", id));
        
        command.ExecuteNonQuery();
        connection.Close();
    }

    public void Delete(int id)
    {
        var query = "DELETE FROM productos WHERE idProducto = @id";
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        
        var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter("@id", id));
        
        command.ExecuteNonQuery();
        connection.Close();
    }
}
