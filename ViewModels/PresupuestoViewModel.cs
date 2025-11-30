// ViewModels/PresupuestoViewModel.cs: Objeto para manejar la creación y edición de Presupuestos.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tl2_tp8_2025_LucasFR_TH.ViewModels
{
    public class PresupuestoViewModel
    {
        public int IdPresupuesto { get; set; }

        // Validación: Requerido
        /// NombreDestinatario: Debe ser obligatorio ([Required]).
        [Display(Name = "Nombre o Email del Destinatario")]
        [Required(ErrorMessage = "El nombre o email es obligatorio.")]
        /// Opcional/Alternativa (Si se decide guardar Email): Si se decide guardar un email en lugar del nombre, se debe validar el formato del email (usar el atributo [EmailAddress]).
        // [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    public string NombreDestinatario { get; set; } = string.Empty;

        // Validación: Requerido y tipo de dato
        /// FechaCreacion: Debe ser requerida ([Required]) y se debe implementar la lógica de validación para asegurar que la fecha sea menor o igual a la fecha actual (no puede ser una fecha futura).
        [Display(Name = "Fecha de Creación")]
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        // Lista de productos añadidos al presupuesto (para bindear en POST)
        public List<AgregarProductoViewModel> Productos { get; set; } = new List<AgregarProductoViewModel>();
    }
}
