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
    public partial class FormularioLogin : ComponentBase
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
        /// Servicio para manipular los reintentos.
        /// </summary>
        [Inject] protected ILoginRetryValidatorService? loginRetryValidatorService { get; set; }

        /// <summary>
        /// Evento que se dispara para mostrar mensajes tipo toast en la interfaz.
        /// </summary>
        [Parameter] public EventCallback<(ToastType toastType, string message)> OnCreateToastMessage { get; set; }

        /// <summary>
        /// Evento que se dispara cuando cambia el estado del paso de autenticación.
        /// </summary>
        [Parameter] public EventCallback<AuthenticateResponseDto> OnStepChanged { get; set; }

        /// <summary>
        /// Intercambiar formulartio entre login / recuperar
        /// </summary>
        [Parameter] public EventCallback<int> OnOptionChange { get; set; }

        /// <summary>
        /// Botón de guardar con funcionalidad de carga visual.
        /// </summary>
        private Button saveButton = default!;

        /// <summary>
        /// Objeto que contiene los datos de autenticación del usuario.
        /// </summary>
        private UsuarioAutenticacionDto usuarioAutenticacion = new UsuarioAutenticacionDto();
        /// <summary>
        /// Método para autenticar al usuario. Si la autenticación falla, incrementa el contador de intentos y bloquea si es necesario.
        /// </summary>
        private async Task AccesoUsuario()
        {
            try
            {
                var loginRetryValidator = loginRetryValidatorService.LoginThrottleService(usuarioAutenticacion.Email);
                if (servicioAutenticacion != null && loginRetryValidator.IsSuccess)
                {
                    saveButton.ShowLoading("Verificando...");
                    var result = await servicioAutenticacion.Autenticar(usuarioAutenticacion);
                    
                    if (result.IsSuccess)
                    {
                        await OnStepChanged.InvokeAsync(result.Result);
                        loginRetryValidatorService.RemoveAttemptByEmail(usuarioAutenticacion.Email);

                    }
                    else
                    {
                        await OnCreateToastMessage.InvokeAsync((ToastType.Danger, $"{string.Join(";", result.ErrorMessages)}\nIntentos: {loginRetryValidator.Value}"));
                    }

                    saveButton.HideLoading();
                } else {
                    await OnCreateToastMessage.InvokeAsync((ToastType.Danger, loginRetryValidator.ErrorMessage ?? ""));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.CompletedTask;
        }
        private async Task Recovery() {
            await OnOptionChange.InvokeAsync(2);
        }
    }
}
