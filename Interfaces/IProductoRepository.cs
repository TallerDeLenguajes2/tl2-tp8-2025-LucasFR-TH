using espacioProductos;
using System.Collections.Generic;

namespace tl2_tp8_2025_LucasFR_TH.Interfaces;

/// <summary>
/// Interfaz de abstracción para el repositorio de Productos.
/// Define el contrato para todas las operaciones CRUD.
/// </summary>
public interface IProductoRepository
{
    /// <summary>
    /// Obtiene todos los productos disponibles.
    /// </summary>
    /// <returns>Lista de productos o lista vacía si no hay.</returns>
    List<Producto> GetAll();

    /// <summary>
    /// Obtiene un producto específico por su ID.
    /// </summary>
    /// <param name="id">ID del producto a buscar.</param>
    /// <returns>Producto si existe, null si no.</returns>
    Producto GetById(int id);

    /// <summary>
    /// Crea un nuevo producto en la base de datos.
    /// </summary>
    /// <param name="producto">Objeto Producto a crear.</param>
    /// <returns>Producto creado con ID asignado.</returns>
    Producto Create(Producto producto);

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    /// <param name="id">ID del producto a actualizar.</param>
    /// <param name="producto">Objeto Producto con los nuevos datos.</param>
    void Update(int id, Producto producto);

    /// <summary>
    /// Elimina un producto de la base de datos.
    /// </summary>
    /// <param name="id">ID del producto a eliminar.</param>
    void Delete(int id);
}
