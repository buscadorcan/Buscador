using System.Text.Json;
using Infractruture.Interfaces;
using Microsoft.JSInterop;
using SharedApp.Dtos;

namespace Infractruture.Services
{
    /// <summary>
    /// Servicio para la validación de reintentos de login de los usuarios.
    /// Esta clase gestiona los intentos fallidos de login, el control de los reintentos y el tiempo de espera 
    /// basado en un límite de intentos (máximo 3 intentos) y un tiempo de espera de 20 minutos entre intentos.
    /// Utiliza `localStorage` para persistir los datos y mantener el estado a través de recargas de página.
    /// </summary>
    public class LoginRetryValidatorService : ILoginRetryValidatorService
    {
        private List<LoginAttemptDto> _attemptsList = new List<LoginAttemptDto>();
        private const int MaxAttempts = 3;  // Número máximo de intentos fallidos permitidos.
        private const int TimeoutMinutes = 20;  // Tiempo de espera después de 3 intentos fallidos (en minutos).
        private readonly IJSRuntime _jSRuntime;  // Servicio para interactuar con localStorage en el navegador.

        /// <summary>
        /// Constructor que inicializa el servicio de validación de reintentos de login.
        /// </summary>
        /// <param name="jSRuntime">Instancia de `IJSRuntime` para interactuar con el navegador y su `localStorage`.</param>
        public LoginRetryValidatorService(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        /// <summary>
        /// Método que se llama cuando el componente se inicializa. Carga los intentos de login desde `localStorage`.
        /// </summary>
        /// <returns>Una tarea asincrónica.</returns>
        public async Task OnInitializedAsync()
        {
            await SetListByStore();
        }

        /// <summary>
        /// Carga los intentos de login fallidos almacenados en `localStorage` en la lista interna.
        /// </summary>
        /// <returns>Una tarea asincrónica.</returns>
        public async Task SetListByStore()
        {
            var storedValue = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "LoginAttemptDto");

            if (!string.IsNullOrEmpty(storedValue))
            {
                _attemptsList = JsonSerializer.Deserialize<List<LoginAttemptDto>>(storedValue) ?? new List<LoginAttemptDto>();
            }
        }

        /// <summary>
        /// Valida el número de intentos de login realizados para un correo dado y verifica si el usuario está bloqueado.
        /// Si el tiempo de espera de 20 minutos ha pasado desde el último intento fallido, resetea el contador.
        /// Si no ha pasado el tiempo, aumenta el contador de intentos y lo guarda en `localStorage`.
        /// 
        /// Si el número de intentos es mayor o igual al máximo permitido (3), devuelve el tiempo restante hasta que el usuario 
        /// pueda intentar nuevamente.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario para verificar los intentos.</param>
        /// <returns>Un objeto `Result<int>` que contiene el número de intentos realizados o un mensaje de error si está bloqueado.</returns>
        public Result<int> LoginThrottleService(string email)
        {
            var attempt = _attemptsList.FirstOrDefault(a => a.Email == email) ?? CreateNewAttempt(email);

            if (HasTimeoutPassed(attempt.LastUpdated))
            {
                ResetAttempt(attempt);
                SaveToLocalStorage();
                return Result<int>.Success(0);
            }

            if (attempt.Counter < MaxAttempts)
            {
                IncrementAttempt(attempt);
                SaveToLocalStorage();
                return Result<int>.Success(attempt.Counter);
            }
            else
            {
                return GetRemainingTimeMessage(attempt.LastUpdated);
            }
        }

        /// <summary>
        /// Elimina el intento de login asociado a un correo electrónico específico.
        /// Elimina el intento de login fallido de la lista y actualiza `localStorage`.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario cuyo intento debe ser eliminado.</param>
        public void RemoveAttemptByEmail(string email)
        {
            var attempt = _attemptsList.FirstOrDefault(a => a.Email == email);

            if (attempt != null)
            {
                _attemptsList.Remove(attempt);
                SaveToLocalStorage();
            }
        }

        /// <summary>
        /// Crea un nuevo intento de login para un correo dado si no existe.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario para el que se crea el intento.</param>
        /// <returns>Un objeto `LoginAttemptDto` representando el nuevo intento.</returns>
        private LoginAttemptDto CreateNewAttempt(string email)
        {
            var newAttempt = new LoginAttemptDto { Email = email };
            _attemptsList.Add(newAttempt);
            SaveToLocalStorage();
            return newAttempt;
        }

        /// <summary>
        /// Verifica si ha pasado el tiempo de espera de 20 minutos desde el último intento fallido.
        /// </summary>
        /// <param name="lastUpdated">La marca de tiempo del último intento.</param>
        /// <returns>True si ha pasado más de 20 minutos, de lo contrario, False.</returns>
        private bool HasTimeoutPassed(DateTime lastUpdated)
        {
            return (DateTime.UtcNow - lastUpdated).TotalMinutes > TimeoutMinutes;
        }

        /// <summary>
        /// Resetea el contador de intentos fallidos y actualiza la marca de tiempo al tiempo actual.
        /// </summary>
        /// <param name="attempt">El objeto `LoginAttemptDto` que se debe resetear.</param>
        private void ResetAttempt(LoginAttemptDto attempt)
        {
            attempt.Counter = 0;
            attempt.LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Incrementa el contador de intentos fallidos y actualiza la marca de tiempo al tiempo actual.
        /// </summary>
        /// <param name="attempt">El objeto `LoginAttemptDto` cuyo contador se incrementará.</param>
        private void IncrementAttempt(LoginAttemptDto attempt)
        {
            attempt.Counter++;
            attempt.LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Obtiene el mensaje que indica el tiempo restante hasta que el usuario pueda volver a intentar.
        /// Si el tiempo restante es mayor a 0, se devuelve el mensaje con el tiempo en minutos y segundos.
        /// </summary>
        /// <param name="lastUpdated">La marca de tiempo del último intento fallido.</param>
        /// <returns>Un objeto `Result<int>` con el mensaje de error si está bloqueado o el número de intentos.</returns>
        private Result<int> GetRemainingTimeMessage(DateTime lastUpdated)
        {
            var elapsedTime = DateTime.UtcNow - lastUpdated;
            var remainingTime = TimeSpan.FromMinutes(TimeoutMinutes) - elapsedTime;

            if (remainingTime.TotalSeconds > 0)
            {
                string timeLeft = $"{remainingTime.Minutes} min {remainingTime.Seconds} sec";
                return Result<int>.Failure($"Bloqueado por los siguientes {timeLeft}");
            }

            return Result<int>.Success(0);
        }

        /// <summary>
        /// Guarda la lista actualizada de intentos fallidos en `localStorage`.
        /// </summary>
        /// <returns>Una tarea asincrónica que representa la operación de guardar en `localStorage`.</returns>
        private async void SaveToLocalStorage()
        {
            var serializedData = JsonSerializer.Serialize(_attemptsList);
            await _jSRuntime.InvokeVoidAsync("localStorage.setItem", "LoginAttemptDto", serializedData);
        }
    }
}
