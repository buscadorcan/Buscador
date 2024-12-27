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
        private List<VwRolDto>? listaRoles;
        private List<OnaDto>? listaOna;

        [Inject]
        public IUsuariosService? iUsuariosService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }

        private List<UsuarioDto>? listaUsuarios;

        protected override async Task OnInitializedAsync()
        {
            if (Id > 0 && iUsuariosService != null)
            {
                usuario = await iUsuariosService.GetUsuarioAsync(Id.Value);
                if (usuario != null)
                {
                    usuario.Clave = null;
                }
            }
            else
            {
                listaRoles = await iUsuariosService.GetRolesAsync();
                listaOna = await iUsuariosService.GetOnaAsync();

                if (listaRoles != null && listaRoles.Any())
                {
                    var usuarioMaster = listaRoles.FirstOrDefault(rol => rol.Rol == "UsuarioMaster");
                    if (usuarioMaster != null)
                    {
                        usuario.Rol = usuarioMaster.Rol;
                    }
                }
                else
                {
                    listaRoles = new List<VwRolDto>(); 
                }

                if (listaOna != null && listaOna.Any())
                {
                    var KEY_ECU_SAE = listaOna.FirstOrDefault(ona => ona.RazonSocial == "KEY_ECU_SAE");
                    if (KEY_ECU_SAE != null)
                    {
                        usuario.RazonSocial = KEY_ECU_SAE.RazonSocial;
                    }
                }
                else
                {
                    listaOna = new List<OnaDto>();
                }


                if (usuario == null)
                {
                    usuario = new UsuarioDto(); 
                }

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
        private async Task OnAutoCompleteChanged(string rol, int idRol) {

            usuario.Rol = rol;
            usuario.IdHomologacionRol = idRol;  
            //int idrol;
            //if (rol == "UsuarioMaster")
            //{
            //    idrol = 15;
            //    usuario.Rol = rol;
            //    usuario.IdHomologacionRol = idrol;
            //}
            //else if (rol == "UsuarioOna")
            //{
            //    idrol = 16;
            //    usuario.Rol = rol;
            //    usuario.IdHomologacionRol = idrol;
            //}
            //else if (rol == "UsuarioRead")
            //{
            //    idrol = 17;
            //    usuario.Rol = rol;
            //    usuario.IdHomologacionRol = idrol;
            //}

        }

        private void OnAutoCompleteRazonSocOnaChanged(string razonSocial, int idOna)
        {

            usuario.RazonSocial = razonSocial;
            usuario.IdONA = idOna;

            //int idONA;
            //if (rol == "Ecuador")
            //{
            //    idONA = 1;
            //    usuario.IdONA = idONA;
            //}
            //else if (rol == "Colombia")
            //{
            //    idONA = 2;
            //    usuario.IdONA = idONA;
            //}
            //else if (rol == "Peru")
            //{
            //    idONA = 3;
            //    usuario.IdONA = idONA;
            //}
            //else if (rol == "Bolivia")
            //{
            //    idONA = 4;
            //    usuario.IdONA = idONA;
            //}
        }
    }
}
