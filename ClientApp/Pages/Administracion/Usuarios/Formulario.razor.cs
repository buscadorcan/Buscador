using BlazorBootstrap;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using Infractruture.Services;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;
using System.Data;

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
        public Infractruture.Services.ToastService? toastService { get; set; }
        private List<UsuarioDto>? listaUsuarios;
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            if (Id > 0 && iUsuariosService != null)
            {
                objEventTracking.CodigoHomologacionMenu = "/editar-usuario";
                objEventTracking.NombreAccion = "OnInitializedAsync";
                objEventTracking.NombreControl = "editar-usuario";
                objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                usuario = await iUsuariosService.GetUsuarioAsync(Id.Value);

                if (usuario != null)
                {
                    usuario.Clave = null;

                    listaRoles = await iUsuariosService.GetRolesAsync();
                    listaOna = await iUsuariosService.GetOnaAsync();

                    var rolRelacionado = listaRoles.FirstOrDefault(rol => rol.IdHomologacionRol == usuario.IdHomologacionRol);
                    //var rol = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Rol_Local);
                    //var rolCombox = listaRoles.FirstOrDefault(role => role.IdHomologacionRol == rol);

                    var rol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                    var rolCombox = listaRoles.FirstOrDefault(role => role.CodigoHomologacion == rol);
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
                objEventTracking.CodigoHomologacionMenu = "/nuevo-usuario";
                objEventTracking.NombreAccion = "OnInitializedAsync";
                objEventTracking.NombreControl = "nuevo-usuario";
                objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                listaRoles = await iUsuariosService.GetRolesAsync();
                listaOna = await iUsuariosService.GetOnaAsync();

                //var rol = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Rol_Local);
                //var rolCombox = listaRoles.FirstOrDefault(role => role.IdHomologacionRol == rol);

                var rol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                var rolCombox = listaRoles.FirstOrDefault(role => role.CodigoHomologacion == rol);
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
            objEventTracking.CodigoHomologacionMenu = "/editar-usuario";
            objEventTracking.NombreAccion = "RegistrarUsuario";
            objEventTracking.NombreControl = "btnGuardar";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

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
                if (usuario.IdUsuario <= 0)
                {
                    if (String.IsNullOrEmpty(usuario.Clave))
                    {
                        toastService?.CreateToastMessage(ToastType.Info, "Debe ingresar la clave");
                        navigationManager?.NavigateTo("/nuevo-usuario");
                        saveButton.HideLoading();
                        return;
                    }
                }
                var result = await iUsuariosService.RegistrarOActualizar(usuario);

                if (result.registroCorrecto)
                {
                    toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                    navigationManager?.NavigateTo("/usuarios");
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al registrar en el servidor");
                    navigationManager?.NavigateTo("/nuevo-usuario");
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
        private string emailValidationMessage = string.Empty;
        private CancellationTokenSource debounceToken = new();

        private async Task ValidateEmail(ChangeEventArgs e)
        {
            usuario.Email = e.Value.ToString();

            // Cancelar validaciones anteriores (debounce)
            debounceToken.Cancel();
            debounceToken = new CancellationTokenSource();

            try
            {
                // Esperar unos milisegundos antes de validar (para debounce)
                if (Id == null || Id == 0)
                {
                    await Task.Delay(500, debounceToken.Token);

                    if (iUsuariosService != null && !string.IsNullOrWhiteSpace(usuario.Email))
                    {
                        var isUnique = await iUsuariosService.ValidarEmailUnico(usuario.Email);
                        if (!isUnique)
                        {
                            emailValidationMessage = "El email ya está registrado.";
                        }
                        else
                        {
                            emailValidationMessage = string.Empty;
                        }
                    }
                }
                else
                {
                    emailValidationMessage = string.Empty;
                }
                
            }
            catch (TaskCanceledException)
            {
                // La validación anterior fue cancelada, no hacer nada
            }
        }

    }
}
