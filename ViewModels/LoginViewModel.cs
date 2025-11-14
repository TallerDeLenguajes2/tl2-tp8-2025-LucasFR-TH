using System.ComponentModel.DataAnnotations;

namespace tl2_tp8_2025_LucasFR_TH.ViewModels;

/// <summary>
/// ViewModel para el login de usuarios.
/// Contiene las credenciales necesarias para autenticación.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Nombre de usuario (username).
    /// </summary>
    [Display(Name = "Usuario")]
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El usuario no puede superar 50 caracteres.")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario.
    /// </summary>
    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Opción para recordar al usuario (para futuros desarrollos).
    /// </summary>
    [Display(Name = "Recuérdame")]
    public bool RememberMe { get; set; } = false;
}
