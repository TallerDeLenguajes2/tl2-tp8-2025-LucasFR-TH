using System.ComponentModel.DataAnnotations;

namespace tl2_tp8_2025_LucasFR_TH.ViewModels;

public class ProductoViewModel
{
    public int idProducto { get; set; }

    // Descripción: opcional, máximo 250 caracteres
    [StringLength(250, ErrorMessage = "La descripción no puede tener más de 250 caracteres")]
    public string? descripcion { get; set; }

    // Precio: requerido y debe ser mayor a 0
    [Required(ErrorMessage = "El precio es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public int precio { get; set; }
}
