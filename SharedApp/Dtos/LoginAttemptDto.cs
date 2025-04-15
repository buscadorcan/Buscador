namespace SharedApp.Dtos
{
    public class LoginAttemptDto
    {
        /// <summary>
        /// Email del usuarion que se esta persistiendo
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Contador de intentos fallidos de autenticación.
        /// </summary>
        public int Counter { get; set; } = 0;

        /// <summary>
        /// Marca de tiempo de la última actualización del contador de intentos.
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}