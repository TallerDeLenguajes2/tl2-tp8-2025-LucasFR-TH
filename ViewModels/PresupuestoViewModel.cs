using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace tl2_tp8_2025_LucasFR_TH.ViewModels;

public class PresupuestoViewModel : IValidatableObject
{
    public int idPresupuesto { get; set; }

    [Required(ErrorMessage = "El destinatario es requerido")]
    public string nombreDestinatario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de creación es requerida")]
    [DataType(DataType.DateTime)]
    public DateTime fechaCreacion { get; set; } = DateTime.Now;

    // Lista de productos añadidos al presupuesto (para bindear en POST)
    public List<AgregarProductoViewModel> Productos { get; set; } = new List<AgregarProductoViewModel>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (fechaCreacion > DateTime.Now)
        {
            yield return new ValidationResult("La fecha de creación no puede ser una fecha futura", new[] { nameof(fechaCreacion) });
        }
    }
}
