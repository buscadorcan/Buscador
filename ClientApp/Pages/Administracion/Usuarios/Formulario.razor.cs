using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
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
        private bool isRol16; // Variable para controlar la visibilidad del botón

        [Inject]
        public IUsuariosService? iUsuariosService { get; set; }

        [Inject]
        public NavigationManager? navigationManager { get; set; }

        [Parameter]
        public int? Id { get; set; }

        [Inject]
        public Services.ToastService? toastService { get; set; }

        private List<UsuarioDto>? listaUsuarios;

        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }

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
                    var rol = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Rol_Local);
                    var rolCombox = listaRoles.FirstOrDefault(role => role.IdHomologacionRol == rol);
                    isRol16 = rolCombox.CodigoHomologacion == "KEY_USER_ONA";

                    if (rolRelacionado != null)
                    {
                        usuario.Rol = rolRelacionado.Rol;
                        if (isRol16)
                        {
                            listaRoles = listaRoles.Where(rol => rol.CodigoHomologacion == "KEY_USER_ONA" || rol.CodigoHomologacion == "KEY_USER_READ").ToList();
                        }     
                    }
                    else
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



                    // RAZON SOCIAL
                    var razonSocial = listaOna.FirstOrDefault(ona => ona.IdONA == usuario.IdONA);

                    if (isRol16)
                    {
                        var onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
                        listaOna = listaOna.Where(onas => onas.IdONA == onaPais).ToList();

                        if (razonSocial != null)
                        {
                            usuario.RazonSocial = razonSocial.RazonSocial;
                        }
                    }
                    else
                    {
                        
                        var KEY_ECU_SAE = listaOna
                            .Where(ona => ona.IdONA == ona.IdONA)  // Filtrar solo los roles "UsuarioMaster"
                            .OrderBy(ona => ona.IdONA)     // Ordenar de forma ascendente por el campo IdHomologacionRol
                            .FirstOrDefault();

                        if (KEY_ECU_SAE != null)
                        {
                            usuario.RazonSocial = razonSocial.RazonSocial;
                        }
                    }
                }
            }
            else
            {
                listaRoles = await iUsuariosService.GetRolesAsync();
                listaOna = await iUsuariosService.GetOnaAsync();

                var rol = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Rol_Local);
                var rolCombox = listaRoles.FirstOrDefault(role => role.IdHomologacionRol == rol);
                isRol16 = rolCombox.CodigoHomologacion == "KEY_USER_ONA";

                if (listaRoles != null && listaRoles.Any())
                {
                    // Filtrar los roles cuando isRol16 es verdadero
                    if (isRol16)
                    {
                        listaRoles = listaRoles.Where(rol => rol.CodigoHomologacion == "KEY_USER_ONA" || rol.CodigoHomologacion == "KEY_USER_READ").ToList();

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
                        listaRoles = await iUsuariosService.GetRolesAsync();
                        var usuarioMaster = listaRoles
                            .Where(rol => rol.IdHomologacionRol == rol.IdHomologacionRol)  // Filtrar solo los roles "UsuarioMaster"
                            .OrderBy(rol => rol.IdHomologacionRol)     // Ordenar de forma ascendente por el campo IdHomologacionRol
                            .FirstOrDefault();

                        if (usuarioMaster != null)
                        {
                            usuario.Rol = usuarioMaster.Rol;
                        }
                    }
                }
                else
                {
                    listaRoles = new List<VwRolDto>();
                }

                if (listaOna != null && listaOna.Any())
                {
                    if (isRol16)
                    {
                        var onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
                        listaOna = listaOna.Where(onas => onas.IdONA == onaPais).ToList();

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
                        listaOna = await iUsuariosService.GetOnaAsync();
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
            //listaUsuarios = await iUsuariosService.GetUsuariosAsync();
            listaRoles = await iUsuariosService.GetRolesAsync();
            listaOna = await iUsuariosService.GetOnaAsync();

            var rolRelacionado = listaRoles.FirstOrDefault(rol => rol.Rol == usuario.Rol);
            var onaRelacionado = listaOna.FirstOrDefault(rol => rol.RazonSocial == usuario.RazonSocial);

            if (usuario.IdHomologacionRol == 0)
            {
                usuario.IdHomologacionRol = rolRelacionado.IdHomologacionRol;
            }

            if (usuario.IdONA == 0)
            {
                usuario.IdONA = onaRelacionado.IdONA;
            }

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

        private async Task OnAutoCompleteChanged(string rol, int idRol)
        {
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
