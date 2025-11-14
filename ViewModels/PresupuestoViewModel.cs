
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tl2_tp8_2025_LucasFR_TH.ViewModels
{
    public class PresupuestoViewModel
    {
        public int IdPresupuesto { get; set; }

        // Validación: Requerido
        [Display(Name = "Nombre o Email del Destinatario")]
        [Required(ErrorMessage = "El nombre o email es obligatorio.")]
        // [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string NombreDestinatario { get; set; }

        // Validación: Requerido y tipo de dato
        [Display(Name = "Fecha de Creación")]
        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        // Lista de productos añadidos al presupuesto (para bindear en POST)
        public List<AgregarProductoViewModel> Productos { get; set; } = new List<AgregarProductoViewModel>();
    }
}
