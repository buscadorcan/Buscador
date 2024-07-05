using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using System.Web;
using BlazorBootstrap;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Autenticacion
{
    public partial class Acceder
    {
        private Button saveButton = default!;
        private UsuarioAutenticacionDto usuarioAutenticacion = new UsuarioAutenticacionDto();
        public string? UrlRetorno { get; set; }
        public string? alertMessage { get; set; }
        [Inject]
        public IServiceAutenticacion? servicioAutenticacion { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        private async Task AccesoUsuario()
        {
            try
            {
                if (servicioAutenticacion != null)
                {
                    saveButton.ShowLoading("Verificando...");
                    var result = await servicioAutenticacion.Acceder(usuarioAutenticacion);
                    Console.WriteLine(result);
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
                        alertMessage = string.Join(";", result.ErrorMessages);
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