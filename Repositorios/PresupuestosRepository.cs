namespace repositorioPresupuesto;

using espacioPresupuestos;
using espacioPresupuestoDetalle;
using espacioProductos;
using tl2_tp8_2025_LucasFR_TH.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

/// <summary>
/// Implementación de repositorio para Presupuestos.
/// Maneja todas las operaciones CRUD contra la base de datos.
/// </summary>
public class PresupuestosRepository : IPresupuestoRepository
{
    private string cadenaConexion = "Data Source = Db/Tienda.db";
    
    public List<Presupuesto> GetAll()
    {
        var query = @"SELECT p.*, pd.cantidad, pr.idProducto, pr.descripcion, pr.precio 
                     FROM presupuestos p 
                     LEFT JOIN presupuestoDetalle pd ON p.idPresupuesto = pd.idPresupuesto
                     LEFT JOIN productos pr ON pd.idProducto = pr.idProducto";
                     
        var presupuestos = new Dictionary<int, Presupuesto>();
        
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        
        var command = new SqliteCommand(query, connection);
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var idPresupuesto = Convert.ToInt32(reader["idPresupuesto"]);
            
            if (!presupuestos.ContainsKey(idPresupuesto))
            {
                presupuestos[idPresupuesto] = new Presupuesto
                {
                    idPresupuesto = idPresupuesto,
                    nombreDestinatario = reader["nombreDestinatario"].ToString(),
                    fechaCreacion = Convert.ToDateTime(reader["fechaCreacion"])
                };
            }
            
            if (!reader.IsDBNull(reader.GetOrdinal("idProducto")))
            {
                var producto = new Producto
                {
                    idProducto = Convert.ToInt32(reader["idProducto"]),
                    descripcion = reader["descripcion"].ToString(),
                    precio = Convert.ToInt32(reader["precio"])
                };
                
                var detalle = new PresupuestoDetalle
                {
                    producto = producto,
                    cantidad = Convert.ToInt32(reader["cantidad"])
                };
                
                presupuestos[idPresupuesto].detalle.Add(detalle);
            }
        }
        
        connection.Close();
        return new List<Presupuesto>(presupuestos.Values);
    }
    
    public Presupuesto GetById(int id)
    {
        var query = @"SELECT p.*, pd.cantidad, pr.idProducto, pr.descripcion, pr.precio 
                     FROM presupuestos p 
                     LEFT JOIN presupuestoDetalle pd ON p.idPresupuesto = pd.idPresupuesto
                     LEFT JOIN productos pr ON pd.idProducto = pr.idProducto
                     WHERE p.idPresupuesto = @id";
                     
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        
        var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter("@id", id));
        
        using var reader = command.ExecuteReader();
        Presupuesto presupuesto = null;
        
        while (reader.Read())
        {
            if (presupuesto == null)
            {
                presupuesto = new Presupuesto
                {
                    idPresupuesto = id,
                    nombreDestinatario = reader["nombreDestinatario"].ToString(),
                    fechaCreacion = Convert.ToDateTime(reader["fechaCreacion"])
                };
            }
            
            if (!reader.IsDBNull(reader.GetOrdinal("idProducto")))
            {
                var producto = new Producto
                {
                    idProducto = Convert.ToInt32(reader["idProducto"]),
                    descripcion = reader["descripcion"].ToString(),
                    precio = Convert.ToInt32(reader["precio"])
                };
                
                var detalle = new PresupuestoDetalle
                {
                    producto = producto,
                    cantidad = Convert.ToInt32(reader["cantidad"])
                };
                
                presupuesto.detalle.Add(detalle);
            }
        }
        
        connection.Close();
        return presupuesto;
    }
    
    public Presupuesto Create(Presupuesto presupuesto)
    {
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Insertar presupuesto
            var query = @"INSERT INTO presupuestos (nombreDestinatario, fechaCreacion) 
                         VALUES (@nombre, @fecha);
                         SELECT last_insert_rowid();";
                         
            var command = new SqliteCommand(query, connection, transaction);
            command.Parameters.Add(new SqliteParameter("@nombre", presupuesto.nombreDestinatario));
            command.Parameters.Add(new SqliteParameter("@fecha", presupuesto.fechaCreacion));
            
            presupuesto.idPresupuesto = Convert.ToInt32(command.ExecuteScalar());
            
            // Insertar detalles
            if (presupuesto.detalle != null)
            {
                foreach (var detalle in presupuesto.detalle)
                {
                    query = @"INSERT INTO presupuestoDetalle (idPresupuesto, idProducto, cantidad) 
                             VALUES (@idPresupuesto, @idProducto, @cantidad)";
                             
                    command = new SqliteCommand(query, connection, transaction);
                    command.Parameters.Add(new SqliteParameter("@idPresupuesto", presupuesto.idPresupuesto));
                    command.Parameters.Add(new SqliteParameter("@idProducto", detalle.producto.idProducto));
                    command.Parameters.Add(new SqliteParameter("@cantidad", detalle.cantidad));
                    
                    command.ExecuteNonQuery();
                }
            }
            
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            connection.Close();
        }
        
        return presupuesto;
    }
    
    public void AddProducto(int idPresupuesto, int idProducto, int cantidad)
    {
        var query = @"INSERT INTO presupuestoDetalle (idPresupuesto, idProducto, cantidad) 
                     VALUES (@idPresupuesto, @idProducto, @cantidad)";
                     
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        
        var command = new SqliteCommand(query, connection);
        command.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));
        command.Parameters.Add(new SqliteParameter("@idProducto", idProducto));
        command.Parameters.Add(new SqliteParameter("@cantidad", cantidad));
        
        command.ExecuteNonQuery();
        connection.Close();
    }
    
    public void Delete(int id)
    {
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Eliminar detalles
            var query = "DELETE FROM presupuestoDetalle WHERE idPresupuesto = @id";
            var command = new SqliteCommand(query, connection, transaction);
            command.Parameters.Add(new SqliteParameter("@id", id));
            command.ExecuteNonQuery();
            
            // Eliminar presupuesto
            query = "DELETE FROM presupuestos WHERE idPresupuesto = @id";
            command = new SqliteCommand(query, connection, transaction);
            command.Parameters.Add(new SqliteParameter("@id", id));
            command.ExecuteNonQuery();
            
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            connection.Close();
        }
    }

    public void Update(int id, Presupuesto presupuesto)
    {
        using var connection = new SqliteConnection(cadenaConexion);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Actualizar encabezado
            var query = "UPDATE presupuestos SET nombreDestinatario = @nombre, fechaCreacion = @fecha WHERE idPresupuesto = @id";
            var command = new SqliteCommand(query, connection, transaction);
            command.Parameters.Add(new SqliteParameter("@nombre", presupuesto.nombreDestinatario));
            command.Parameters.Add(new SqliteParameter("@fecha", presupuesto.fechaCreacion));
            command.Parameters.Add(new SqliteParameter("@id", id));
            command.ExecuteNonQuery();

            // Eliminar detalles antiguos
            query = "DELETE FROM presupuestoDetalle WHERE idPresupuesto = @id";
            command = new SqliteCommand(query, connection, transaction);
            command.Parameters.Add(new SqliteParameter("@id", id));
            command.ExecuteNonQuery();

            // Insertar nuevos detalles
            if (presupuesto.detalle != null)
            {
                foreach (var detalle in presupuesto.detalle)
                {
                    query = @"INSERT INTO presupuestoDetalle (idPresupuesto, idProducto, cantidad) 
                             VALUES (@idPresupuesto, @idProducto, @cantidad)";
                    command = new SqliteCommand(query, connection, transaction);
                    command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));
                    command.Parameters.Add(new SqliteParameter("@idProducto", detalle.producto.idProducto));
                    command.Parameters.Add(new SqliteParameter("@cantidad", detalle.cantidad));
                    command.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            connection.Close();
        }
    }
}