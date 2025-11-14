using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace tl2_tp8_2025_LucasFR_TH.ViewModels;

public class AgregarProductoViewModel
{
    [Required]
    public int productoId { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    public int cantidad { get; set; }

    // Para poblar el dropdown en el GET
    public SelectList? ListaProductos { get; set; }
}
