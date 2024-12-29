using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Usuarios
{
    public partial class Formulario
    {
        private Button saveButton = default!;
        private UsuarioDto usuario = new UsuarioDto();
        [Inject]
        public IUsuariosService? iUsuariosService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Id > 0 && iUsuariosService != null) {
                usuario = await iUsuariosService.GetUsuarioAsync(Id.Value);
                usuario.Clave = null;
            } else {
                //usuario.Rol = "USER";
                usuario.Estado = "A";
            }
        }
        private async Task RegistrarUsuario()
        {
            saveButton.ShowLoading("Guardando...");

            if (iUsuariosService != null)
            {
                var result = await iUsuariosService.RegistrarOActualizar(usuario);
                if (result.registroCorrecto)
                {
                    toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                    navigationManager?.NavigateTo("/usuarios");
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al registrar en el servidor");
                }
            }

            saveButton.HideLoading();
        }
        private void OnAutoCompleteChanged(string rol) {
           var usuario = rol;
        }
    }
}
