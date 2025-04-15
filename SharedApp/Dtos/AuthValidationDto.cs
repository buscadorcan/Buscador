using System.ComponentModel.DataAnnotations;

namespace SharedApp.Dtos
{
    /// <summary>
    /// DTO para la validación de autenticación.
    /// </summary>
    public class AuthValidationDto
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        [Required]
        public int IdUsuario { get; set; }

        /// <summary>
        /// id rol
        /// </summary>
        [Required]
        public int IdHomologacionRol { get; set; }

        /// <summary>
        /// Código de autenticación.
        /// </summary>
        [Required]
        public string Codigo { get; set;} = "";
    }

    /// <summary>
    /// DTO para la validación de autenticación.
    /// </summary>
    public class InputCodeDto
    {
        /// <summary>
        /// Código de autenticación 1
        /// </summary>
        [Required] public string Codigo1 { get; set; } = "";

        /// <summary>
        /// Código de autenticación 2
        /// </summary>
        [Required] public string Codigo2 { get; set; } = "";

        /// <summary>
        /// Código de autenticación 3
        /// </summary>
        [Required] public string Codigo3 { get; set; } = "";

        /// <summary>
        /// Código de autenticación 4
        /// </summary>
        [Required] public string Codigo4 { get; set; } = "";

        /// <summary>
        /// Código de autenticación 5
        /// </summary>
        [Required] public string Codigo5 { get; set; } = "";

        /// <summary>
        /// Código de autenticación 6
        /// </summary>
        [Required] public string Codigo6 { get; set; } = "";
    }
}