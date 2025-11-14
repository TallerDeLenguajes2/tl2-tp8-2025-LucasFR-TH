using espacioPresupuestos;
using System.Collections.Generic;

namespace tl2_tp8_2025_LucasFR_TH.Interfaces;

/// <summary>
/// Interfaz de abstracción para el repositorio de Presupuestos.
/// Define el contrato para todas las operaciones CRUD y acciones especiales.
/// </summary>
public interface IPresupuestoRepository
{
    /// <summary>
    /// Obtiene todos los presupuestos.
    /// </summary>
    /// <returns>Lista de presupuestos o lista vacía si no hay.</returns>
    List<Presupuesto> GetAll();

    /// <summary>
    /// Obtiene un presupuesto específico por su ID.
    /// </summary>
    /// <param name="id">ID del presupuesto a buscar.</param>
    /// <returns>Presupuesto si existe, null si no.</returns>
    Presupuesto GetById(int id);

    /// <summary>
    /// Crea un nuevo presupuesto en la base de datos.
    /// </summary>
    /// <param name="presupuesto">Objeto Presupuesto a crear.</param>
    /// <returns>Presupuesto creado con ID asignado.</returns>
    Presupuesto Create(Presupuesto presupuesto);

    /// <summary>
    /// Actualiza un presupuesto existente.
    /// </summary>
    /// <param name="id">ID del presupuesto a actualizar.</param>
    /// <param name="presupuesto">Objeto Presupuesto con los nuevos datos.</param>
    void Update(int id, Presupuesto presupuesto);

    /// <summary>
    /// Elimina un presupuesto de la base de datos.
    /// </summary>
    /// <param name="id">ID del presupuesto a eliminar.</param>
    void Delete(int id);

    /// <summary>
    /// Agrega un producto a un presupuesto existente.
    /// </summary>
    /// <param name="idPresupuesto">ID del presupuesto.</param>
    /// <param name="idProducto">ID del producto a agregar.</param>
    /// <param name="cantidad">Cantidad del producto.</param>
    void AddProducto(int idPresupuesto, int idProducto, int cantidad);
}
