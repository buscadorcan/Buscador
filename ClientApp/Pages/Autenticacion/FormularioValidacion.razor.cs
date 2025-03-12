using System.Web;
using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
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
        /// Identificador del usuario a validar.
        /// </summary>
        [Parameter] public int? IdUsuario { get; set; }
        /// <summary>
        /// Evento que se dispara cuando cambia el estado del paso de autenticación.
        /// </summary>
        [Parameter] public EventCallback<AuthenticateResponseDto> OnStepChanged { get; set; }

        /// <summary>
        /// DTO utilizado para la validación de autenticación.
        /// </summary>
        private AuthValidationDto authValidationDto = new AuthValidationDto();

        /// <summary>
        /// DTO utilizado para la entrada de códigos.
        /// </summary>
        private InputCodeDto inputCodedto = new InputCodeDto();

        /// <summary>
        /// Referencia a los elementos de entrada de códigos.
        /// </summary>
        private ElementReference input1;

        /// <summary>
        /// Referencia a los elementos de entrada de códigos.
        /// </summary>
        private ElementReference input2;

        /// <summary>
        /// Referencia a los elementos de entrada de códigos.
        /// </summary>
        private ElementReference input3;

        /// <summary>
        /// Referencia a los elementos de entrada de códigos.
        /// </summary>
        private ElementReference input4;

        /// <summary>
        /// Referencia a los elementos de entrada de códigos.
        /// </summary>
        private ElementReference input5;

        /// <summary>
        /// Referencia a los elementos de entrada de códigos.
        /// </summary>
        private ElementReference input6;

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
                    authValidationDto.Codigo = inputCodedto.Codigo1 + inputCodedto.Codigo2 + inputCodedto.Codigo3 + inputCodedto.Codigo4 + inputCodedto.Codigo5 + inputCodedto.Codigo6;
                    var result = await servicioAutenticacion.Acceder(authValidationDto);
                    
                    if (true)
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

        /// <summary>
        /// Ir a la pagina login
        /// </summary>
        private async Task GoLoginPage() {
            await OnStepChanged.InvokeAsync(null);
        }

        /// <summary>
        /// Método que se ejecuta cuando se presiona una tecla en el campo de texto.
        /// </summary>
        private async Task MoveNext(ElementReference nextInput)
    {
        if (nextInput.Context != null) // Verifica si el elemento existe antes de hacer focus
        {
            await nextInput.FocusAsync();
        }
    }
    }
}
