namespace Infractruture.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de validación de reintentos de login.
    /// Esta interfaz proporciona métodos para gestionar el contador de intentos de login fallidos
    /// y las restricciones asociadas a los intentos repetidos.
    /// </summary>
    public interface ILoginRetryValidatorService
    {
        /// <summary>
        /// Valida el número de intentos de login realizados para un correo dado.
        /// Si el número de intentos supera el límite permitido, devuelve el tiempo restante
        /// antes de que el usuario pueda intentar nuevamente.
        /// 
        /// Si el tiempo de espera ha pasado (más de 20 minutos desde el último intento fallido),
        /// el contador de intentos se restablece a cero.
        /// 
        /// Si el número de intentos es menor al límite (3 intentos), se incrementa el contador y se
        /// devuelve el número de intentos realizados.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario para verificar los intentos.</param>
        /// <returns>Un resultado con el número de intentos realizados o un mensaje de error si está bloqueado.</returns>
        Result<int> LoginThrottleService(string email);

        /// <summary>
        /// Elimina el intento de login asociado a un correo electrónico específico.
        /// Este método elimina el intento de login fallido de la lista y actualiza el almacenamiento local.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario cuyo intento debe ser eliminado.</param>
        void RemoveAttemptByEmail(string email);
    }
}
