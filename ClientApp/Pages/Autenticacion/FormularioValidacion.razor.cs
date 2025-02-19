using System.Web;
using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Autenticacion
{
    /// <summary>
    /// Componente Blazor para la validación de códigos de autenticación.
    /// </summary>
    public partial class FormularioValidacion
    {
        /// <summary>
        /// Botón de guardado que maneja la visualización de la carga.
        /// </summary>
        private Button saveButton = default!;
        
        /// <summary>
        /// DTO utilizado para la validación de autenticación.
        /// </summary>
        private AuthValidationDto authValidationDto = new AuthValidationDto();
        
        /// <summary>
        /// Servicio de autenticación inyectado.
        /// </summary>
        [Inject]
        public IServiceAutenticacion? servicioAutenticacion { get; set; }
        
        /// <summary>
        /// Administrador de navegación inyectado.
        /// </summary>
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        
        /// <summary>
        /// Evento para mostrar mensajes emergentes en la interfaz de usuario.
        /// </summary>
        [Parameter] public EventCallback<(ToastType toastType, string message)> OnCreateToastMessage { get; set; }
        
        /// <summary>
        /// Identificador del usuario a validar.
        /// </summary>
        [Parameter] public int? IdUsuario { get; set; }

        /// <summary>
        /// Método que realiza la validación del código de autenticación ingresado por el usuario.
        /// </summary>
        private async Task ValidarCodigo()
        {
            try
            {
                if (servicioAutenticacion != null)
                {
                    saveButton.ShowLoading("Verificando...");
                    authValidationDto.IdUsuario = IdUsuario ?? 0;
                    var result = await servicioAutenticacion.Acceder(authValidationDto);
                    
                    if (result.IsSuccess)
                    {
                        var urlAbsoluta = new Uri(navigationManager?.Uri ?? "");
                        var parametrosQuery = HttpUtility.ParseQueryString(urlAbsoluta.Query);
                        var UrlRetorno = parametrosQuery["returnUrl"];
                        
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
    }
}
