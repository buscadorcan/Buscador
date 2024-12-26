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
                usuario.IdHomologacionRol = 18;
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

            int idrol;
            if (rol == "ADMIN")
            {
                idrol = 18;
                usuario.IdHomologacionRol = idrol;
            }
            else if (rol == "USER")
            {
                idrol = 17;
                usuario.IdHomologacionRol = idrol;
            }
        }

        private void OnAutoCompletePaisOnaChanged(string rol)
        {

            int idONA;
            if (rol == "Ecuador")
            {
                idONA = 1;
                usuario.IdONA = idONA;
            }
            else if (rol == "Colombia")
            {
                idONA = 2;
                usuario.IdONA = idONA;
            }
            else if (rol == "Peru")
            {
                idONA = 3;
                usuario.IdONA = idONA;
            }
            else if (rol == "Bolivia")
            {
                idONA = 4;
                usuario.IdONA = idONA;
            }
        }
    }
}
