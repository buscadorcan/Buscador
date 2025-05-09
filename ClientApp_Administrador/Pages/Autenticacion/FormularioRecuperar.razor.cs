using BlazorBootstrap;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;

namespace ClientAppAdministrador.Pages.Autenticacion
{
    /// <summary>
    /// Componente Blazor para la validación de códigos de autenticación.
    /// </summary>
    public partial class FormularioRecuperar : ComponentBase
    {
        /// <summary>
        /// Botón de guardado que maneja la visualización de la carga.
        /// </summary>
        private Button saveButton = default!;
        
        /// <summary>
        /// Servicio de autenticación inyectado.
        /// </summary>
        [Inject] public IServiceAutenticacion servicioAutenticacion { get; set; } = default!;
        
        /// <summary>
        /// Administrador de navegación inyectado.
        /// </summary>
        [Inject] public NavigationManager navigationManager { get; set; } = default!;
        
        /// <summary>
        /// Evento para mostrar mensajes emergentes en la interfaz de usuario.
        /// </summary>
        [Parameter] public EventCallback<(ToastType toastType, string message)> OnCreateToastMessage { get; set; }
        
        /// <summary>
        /// Intercambiar formulartio entre login / recuperar
        /// </summary>
        [Parameter] public EventCallback<int> OnOptionChange { get; set; }

        /// <summary>
        /// DTO utilizado para la validación.
        /// </summary>
        private UsuarioRecuperacionDto usuarioRecuperacion = new UsuarioRecuperacionDto();

        /// <summary>
        /// Método que realiza la validación del email de autenticación ingresado por el usuario.
        /// </summary>
        private async Task RecuperarClave()
        {
            try
            {
                if (servicioAutenticacion != null)
                {
                    saveButton.ShowLoading("Verificando...");
                    var result = await servicioAutenticacion.Recuperar<object>(usuarioRecuperacion);
                    if (result.IsSuccess)
                    {
                       await GoLoginPage(); 
                    }
                    else
                    {
                        await OnCreateToastMessage.InvokeAsync((ToastType.Danger, $"{string.Join(";", result.ErrorMessages)}"));
                    }
                    
                    saveButton.HideLoading();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Método que redirige al usuario a la página de inicio de sesión.
        /// </summary>
        private async Task GoLoginPage() {
            await OnOptionChange.InvokeAsync(1);
        }
    }
}
