using BlazorBootstrap;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;

namespace ClientApp.Pages.Administracion.CambiarClave
{
    public partial class Formulario
    {
        [Inject] private IUsuariosService? iUsuariosService { get; set; }

        /// <summary>
        /// Administrador de navegación inyectado.
        /// </summary>
        [Inject]
        public NavigationManager? navigationManager { get; set; }

        /// <summary>
        /// Botón de guardado que maneja la visualización de la carga.
        /// </summary>
        private Button saveButton = default!;

        private UsuarioCambiarClaveDto usuario = new UsuarioCambiarClaveDto();

        /// <summary>
        /// Lista de mensajes emergentes que se mostrarán en la interfaz de usuario.
        /// </summary>
        private List<ToastMessage> messages = new List<ToastMessage>();

        private async Task OnCambiarClave()
        {
            try
            {
                if (iUsuariosService != null)
                {
                    saveButton.ShowLoading("Verificando...");
                    var result = await iUsuariosService.ActualizarClave(usuario);
                    
                    if (result.IsSuccess && result.Result)
                    {
                        CreateToastMessage((ToastType.Success, "Clave cambiada correctamente"));
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(3000);
                            navigationManager?.NavigateTo("/salir");
                        }); 
                    }
                    else
                    {
                        CreateToastMessage((ToastType.Danger, $"{string.Join(";", result.ErrorMessages)}"));
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
    }
}