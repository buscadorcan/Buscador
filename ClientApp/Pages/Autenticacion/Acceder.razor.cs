using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using System.Web;
using BlazorBootstrap;
using SharedApp.Models.Dtos;
using Newtonsoft.Json;
using Microsoft.JSInterop;

namespace ClientApp.Pages.Autenticacion
{
    public partial class Acceder
    {
        private Button saveButton = default!;
        private UsuarioAutenticacionDto usuarioAutenticacion = new UsuarioAutenticacionDto();
        public string? UrlRetorno { get; set; }
        List<ToastMessage> messages = new List<ToastMessage>();
        [Inject]
        public IServiceAutenticacion? servicioAutenticacion { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        private int Retry = 3;
        private int Minutes = 20;
        private int Counter = 0;
        private System.Timers.Timer? ResetTimer;
        private DateTime LastUpdated;
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

            if (Counter >= Retry) {
                StartResetTimer();
            }
        }
        private async Task AccesoUsuario()
        {
            try
            {
                Console.WriteLine($"Counter: {Counter}");
                if (servicioAutenticacion != null && Counter < Retry)
                {
                    saveButton.ShowLoading("Verificando...");
                    var result = await servicioAutenticacion.Acceder(usuarioAutenticacion);
                    Console.WriteLine(JsonConvert.SerializeObject(result));
                    if (result.IsSuccess)
                    {
                        var urlAbsoluta = new Uri(navigationManager?.Uri ?? "");
                        var parametrosQuery = HttpUtility.ParseQueryString(urlAbsoluta.Query);
                        UrlRetorno = parametrosQuery["returnUrl"];
                        if (string.IsNullOrEmpty(UrlRetorno))
                        {
                            navigationManager?.NavigateTo("/administracion");
                        }
                        else
                        {
                            navigationManager?.NavigateTo("/" + UrlRetorno);
                        }
                    }
                    else
                    {
                        await IncrementCounter();
                        messages.Add(CreateToastMessage(ToastType.Danger, $"{string.Join(";", result.ErrorMessages)}\nIntentos: {Counter}"));
                    }
                    saveButton.HideLoading();
                } else {
                    ShowResetCountdown();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await Task.CompletedTask;
        }
        private ToastMessage CreateToastMessage(ToastType toastType, string message)
        => new ToastMessage
            {
                Type = toastType,
                Title = "ONA",
                Message = message,
            };

        /// <summary>
        /// Increments the counter, updates the last updated timestamp, and stores the values in localStorage.
        /// If the counter reaches 3, it starts the reset timer.
        /// </summary>
        private async Task IncrementCounter()
        {
            Counter++;
            LastUpdated = DateTime.UtcNow;

            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "counter", Counter.ToString());
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "lastUpdated", LastUpdated.ToString());

            if (Counter >= Retry) {
                StartResetTimer();   
            }
        }

        /// <summary>
        /// Resets the counter to zero and updates the last updated timestamp.
        /// The new values are stored in localStorage.
        /// </summary>        
        private async Task ResetCounter()
        {
            Counter = 0;
            LastUpdated = DateTime.UtcNow;

            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "counter", Counter.ToString());
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "lastUpdated", LastUpdated.ToString());
        }

        /// <summary>
        /// Starts a timer that checks every minute if 2 minutes have passed since the last update.
        /// If the elapsed time exceeds 2 minutes, the counter is reset.
        /// </summary>
        private void StartResetTimer()
        {
            ResetTimer = new System.Timers.Timer(60 * 1000);
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
        /// Displays a warning toast message showing the remaining time before the counter resets.
        /// </summary>
        private void ShowResetCountdown()
        {
            var elapsedTime = DateTime.UtcNow - LastUpdated;
            var remainingTime = TimeSpan.FromMinutes(Minutes) - elapsedTime;

            if (remainingTime.TotalSeconds > 0)
            {
                string timeLeft = $"{remainingTime.Minutes} min {remainingTime.Seconds} sec";
                messages.Add(CreateToastMessage(ToastType.Warning, $"Bloqueado por los siguientes {timeLeft}"));
            }
        }
    }
}