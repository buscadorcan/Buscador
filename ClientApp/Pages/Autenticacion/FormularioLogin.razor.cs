using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Autenticacion
{
    /// <summary>
    /// Componente de formulario de inicio de sesión en Blazor.
    /// Maneja la autenticación del usuario, el almacenamiento en localStorage y la gestión de intentos fallidos.
    /// </summary>
    public partial class FormularioLogin
    {
        /// <summary>
        /// Servicio de autenticación inyectado para validar las credenciales del usuario.
        /// </summary>
        [Inject] public IServiceAutenticacion? servicioAutenticacion { get; set; }

        /// <summary>
        /// Servicio de interoperabilidad con JavaScript para manipular el localStorage.
        /// </summary>
        [Inject] protected IJSRuntime? JSRuntime { get; set; }

        /// <summary>
        /// Evento que se dispara para mostrar mensajes tipo toast en la interfaz.
        /// </summary>
        [Parameter] public EventCallback<(ToastType toastType, string message)> OnCreateToastMessage { get; set; }

        /// <summary>
        /// Evento que se dispara cuando cambia el estado del paso de autenticación.
        /// </summary>
        [Parameter] public EventCallback<AuthenticateResponseDto> OnStepChanged { get; set; }

        /// <summary>
        /// Botón de guardar con funcionalidad de carga visual.
        /// </summary>
        private Button saveButton = default!;

        /// <summary>
        /// Objeto que contiene los datos de autenticación del usuario.
        /// </summary>
        private UsuarioAutenticacionDto usuarioAutenticacion = new UsuarioAutenticacionDto();

        /// <summary>
        /// Número máximo de intentos fallidos antes de bloquear el acceso temporalmente.
        /// </summary>
        private int Retry = 3;

        /// <summary>
        /// Tiempo (en minutos) antes de que los intentos fallidos se reinicien.
        /// </summary>
        private int Minutes = 20;

        /// <summary>
        /// Contador de intentos fallidos de autenticación.
        /// </summary>
        private int Counter = 0;

        /// <summary>
        /// Temporizador para restablecer el contador después de un tiempo determinado.
        /// </summary>
        private System.Timers.Timer? ResetTimer;

        /// <summary>
        /// Marca de tiempo de la última actualización del contador de intentos.
        /// </summary>
        private DateTime LastUpdated;

        /// <summary>
        /// Se ejecuta cuando el componente se inicializa y carga los valores previos desde el localStorage.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            var storedValue = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "counter");
            var lastUpdatedString = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "lastUpdated");

            if (!string.IsNullOrEmpty(storedValue))
            {
                Counter = int.Parse(storedValue);
            }

            if (!string.IsNullOrEmpty(lastUpdatedString) && DateTime.TryParse(lastUpdatedString, out DateTime lastUpdated))
            {
                LastUpdated = lastUpdated;
            }
            else
            {
                LastUpdated = DateTime.UtcNow;
            }

            if (Counter >= Retry)
            {
                StartResetTimer();
            }
        }

        /// <summary>
        /// Método para autenticar al usuario. Si la autenticación falla, incrementa el contador de intentos y bloquea si es necesario.
        /// </summary>
        private async Task AccesoUsuario()
        {
            try
            {
                if (servicioAutenticacion != null && Counter < Retry)
                {
                    saveButton.ShowLoading("Verificando...");
                    var result = await servicioAutenticacion.Autenticar(usuarioAutenticacion);
                    
                    if (result.IsSuccess)
                    {
                        await OnStepChanged.InvokeAsync(result.Result);
                    }
                    else
                    {
                        await IncrementCounter();
                        await OnCreateToastMessage.InvokeAsync((ToastType.Danger, $"{string.Join(";", result.ErrorMessages)}\nIntentos: {Counter}"));
                    }

                    saveButton.HideLoading();
                }
                else
                {
                    await ShowResetCountdown();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Incrementa el contador de intentos fallidos y lo almacena en localStorage.
        /// Si el contador alcanza el límite, inicia el temporizador de restablecimiento.
        /// </summary>
        private async Task IncrementCounter()
        {
            Counter++;
            LastUpdated = DateTime.UtcNow;

            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "counter", Counter.ToString());
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "lastUpdated", LastUpdated.ToString());

            if (Counter >= Retry)
            {
                StartResetTimer();
            }
        }

        /// <summary>
        /// Restablece el contador de intentos a cero y actualiza el localStorage.
        /// </summary>
        private async Task ResetCounter()
        {
            Counter = 0;
            LastUpdated = DateTime.UtcNow;
            
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "counter", Counter.ToString());
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "lastUpdated", LastUpdated.ToString());
        }

        /// <summary>
        /// Inicia un temporizador que revisa periódicamente si ha pasado el tiempo de espera para restablecer el contador.
        /// </summary>
        private void StartResetTimer()
        {
            ResetTimer = new System.Timers.Timer(60 * 1000); // 1 minuto
            ResetTimer.Elapsed += async (sender, e) =>
            {
                var elapsedMinutes = (DateTime.UtcNow - LastUpdated).TotalMinutes;
                if (elapsedMinutes >= Minutes)
                {
                    await ResetCounter();

                    if (ResetTimer != null)
                    {
                        ResetTimer.Stop();
                        ResetTimer.Dispose();
                        ResetTimer = null;
                    }
                }
            };
            ResetTimer.AutoReset = true;
            ResetTimer.Start();
        }

        /// <summary>
        /// Muestra un mensaje de advertencia indicando cuánto tiempo falta para que el contador se restablezca.
        /// </summary>
        private async Task ShowResetCountdown()
        {
            var elapsedTime = DateTime.UtcNow - LastUpdated;
            var remainingTime = TimeSpan.FromMinutes(Minutes) - elapsedTime;

            if (remainingTime.TotalSeconds > 0)
            {
                string timeLeft = $"{remainingTime.Minutes} min {remainingTime.Seconds} sec";
                await OnCreateToastMessage.InvokeAsync((ToastType.Warning, $"Bloqueado por los siguientes {timeLeft}"));
            }
        }
    }
}
