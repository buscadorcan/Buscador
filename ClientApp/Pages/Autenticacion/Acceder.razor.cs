using BlazorBootstrap;
using SharedApp.Models.Dtos;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Pages.Autenticacion
{
    /// <summary>
    /// Componente parcial para la página de autenticación.
    /// Gestiona mensajes emergentes (toasts) y maneja la autenticación del usuario.
    /// </summary>
    public partial class Acceder
    {
        /// <summary>
        /// Administrador de navegación inyectado.
        /// </summary>
        [Inject] NavigationManager? _navigationManager { get; set; }

        /// <summary>
        /// Lista de mensajes emergentes que se mostrarán en la interfaz de usuario.
        /// </summary>
        private List<ToastMessage> messages = new List<ToastMessage>();

        /// <summary>
        /// Objeto que almacena la respuesta de autenticación del usuario.
        /// Puede ser nulo hasta que el usuario se autentique correctamente.
        /// </summary>
        private AuthenticateResponseDto? authenticateResponseDto = default;

        /// <summary>
        /// Objeto que almacena la opcióon de formulario usando en la autenticación.
        /// </summary>
        private int opcion = 1;

        /// <summary>
        /// Crea y agrega un mensaje emergente (toast) a la lista de mensajes.
        /// </summary>
        /// <param name="data">Tupla que contiene el tipo de mensaje (toast) y el contenido del mensaje.</param>
        private void CreateToastMessage((ToastType toastType, string message) data)
        {
            messages.Add(new ToastMessage
            {
                Type = data.toastType,
                Title = "ONA",
                Message = data.message,
            });
        }

        /// <summary>
        /// Maneja el cambio de paso en el proceso de autenticación,
        /// actualizando el estado del objeto de respuesta de autenticación.
        /// </summary>
        /// <param name="_authenticateResponseDto">Objeto de autenticación actualizado.</param>
        private void HandleStepChange(AuthenticateResponseDto? _authenticateResponseDto)
        {
            authenticateResponseDto = _authenticateResponseDto;
        }

        /// <summary>
        /// Método que maneja la navegación hacia una ruta específica cuando se hace clic en un componente.
        /// </summary>
        private void GoToSearchPage()
        {
            _navigationManager.NavigateTo("/");
        }

        /// <summary>
        /// Cambia de formulario entre login / recuperar clave
        /// </summary>
        /// <param name="_opcion">opción seleccionada</param>
        private void HandleOptionChange(int _opcion)
        {
            opcion = _opcion;
        }
    }
}
