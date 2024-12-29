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

                    listaRoles = await iUsuariosService.GetRolesAsync();
                    listaOna = await iUsuariosService.GetOnaAsync();

                    var rolRelacionado = listaRoles.FirstOrDefault(rol => rol.IdHomologacionRol == usuario.IdHomologacionRol);

                    if (rolRelacionado != null)
                    {
                        usuario.Rol = rolRelacionado.Rol; 
                    
                    } else
                    {
                        var usuarioMaster = listaRoles
                   .Where(rol => rol.IdHomologacionRol == rol.IdHomologacionRol)  // Filtrar solo los roles "UsuarioMaster"
                   .OrderBy(rol => rol.IdHomologacionRol)     // Ordenar de forma ascendente por el campo IdHomologacionRol
                   .FirstOrDefault();

                        if (usuarioMaster != null)
                        {
                            usuario.Rol = usuarioMaster.Rol;
                        }
                    }

                    var razonSocial = listaOna.FirstOrDefault(ona => ona.IdONA == usuario.IdONA);

                    if (razonSocial != null)
                    {

                        usuario.RazonSocial = razonSocial.RazonSocial;
                    }
                    else
                    {
                        var KEY_ECU_SAE = listaOna
                   .Where(ona => ona.IdONA == ona.IdONA)  // Filtrar solo los roles "UsuarioMaster"
                   .OrderBy(ona => ona.IdONA)     // Ordenar de forma ascendente por el campo IdHomologacionRol
                   .FirstOrDefault();
                        if (KEY_ECU_SAE != null)
                        {
                            usuario.RazonSocial = KEY_ECU_SAE.RazonSocial;
                        }
                    }
                }
            }
            else
            {
                listaRoles = await iUsuariosService.GetRolesAsync();
                listaOna = await iUsuariosService.GetOnaAsync();

                if (listaRoles != null && listaRoles.Any())
                {
                    var usuarioMaster = listaRoles
                    .Where(rol => rol.IdHomologacionRol == rol.IdHomologacionRol)  // Filtrar solo los roles "UsuarioMaster"
                    .OrderBy(rol => rol.IdHomologacionRol)     // Ordenar de forma ascendente por el campo IdHomologacionRol
                    .FirstOrDefault();

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
                    var KEY_ECU_SAE = listaOna
                    .Where(ona => ona.IdONA == ona.IdONA)  // Filtrar solo los roles "UsuarioMaster"
                    .OrderBy(ona => ona.IdONA)     // Ordenar de forma ascendente por el campo IdHomologacionRol
                    .FirstOrDefault();
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
           
        }

        private void OnAutoCompleteRazonSocOnaChanged(string razonSocial, int idOna)
        {

            usuario.RazonSocial = razonSocial;
            usuario.IdONA = idOna;
        }
    }
}
