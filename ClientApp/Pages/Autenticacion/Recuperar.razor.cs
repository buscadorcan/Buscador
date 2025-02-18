using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using BlazorBootstrap;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Autenticacion
{
    public partial class Recuperar
    {
        private Button saveButton = default!;
        private UsuarioRecuperacionDto usuarioRecuperacion = new UsuarioRecuperacionDto();
        List<ToastMessage> messages = new List<ToastMessage>();
        [Inject]
        public IServiceAutenticacion? servicioAutenticacion { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
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
                        navigationManager?.NavigateTo("/acceder");
                    }
                    else
                    {
                        messages.Add(CreateToastMessage(ToastType.Danger, $"{string.Join(";", result.ErrorMessages)}"));
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
        private ToastMessage CreateToastMessage(ToastType toastType, string message)
        => new ToastMessage
            {
                Type = toastType,
                Title = "ONA",
                Message = message,
            };
    }
}