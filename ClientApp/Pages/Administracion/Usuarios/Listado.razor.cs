using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Pages.Administracion.Esquemas;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Usuarios
{
    public partial class Listado
    {
        private List<UsuarioDto>? listaUsuarios;
        private bool isRol17; // Variable para controlar la visibilidad del botón
        private bool isRol16;
        private bool showModal; // Controlar la visibilidad de la ventana modal  
        private string modalMessage;
        private int rolCargo;
        private int onaPais;
        [Inject]
        IUsuariosService? iUsuariosService { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        [Inject]
        private NavigationManager iNavigationManager { get; set; }

        private List<VwRolDto>? listaRoles;
        private List<OnaDto>? listaOna;

        protected override async Task OnInitializedAsync()
        {

            rolCargo = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Rol_Local);
            onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            listaUsuarios = await iUsuariosService.GetUsuariosAsync();

            listaRoles = await iUsuariosService.GetRolesAsync();
            listaOna = await iUsuariosService.GetOnaAsync();

            var rolRelacionado = listaRoles.FirstOrDefault(rol => rol.IdHomologacionRol == rolCargo);
            var onaRelacionado = listaOna.FirstOrDefault(ona => ona.IdONA == onaPais);

            // Obtener el rol del usuario desde LocalStorage  


            // Puedes establecer isRol17 si es necesario aquí, dependiendo de la lógica de tu aplicación  
            isRol17 = rolRelacionado.CodigoHomologacion == "KEY_USER_READ"; // Ajusta según la lógica de tu aplicación  
        }

        private async void EditarUsuario(UsuarioDto usuario)
        {

             rolCargo = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Rol_Local);
            onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            listaUsuarios = await iUsuariosService.GetUsuariosAsync();

            listaRoles = await iUsuariosService.GetRolesAsync();
            listaOna = await iUsuariosService.GetOnaAsync();

            var rolRelacionado = listaRoles.FirstOrDefault(rol => rol.IdHomologacionRol == rolCargo);
            var onaRelacionado = listaOna.FirstOrDefault(ona => ona.IdONA == onaPais);
            var Homolog = listaUsuarios.FirstOrDefault(usu => usu.IdUsuario == usuario.IdUsuario);
            var rolUsuario = listaRoles.FirstOrDefault(rol => rol.IdHomologacionRol == Homolog.IdHomologacionRol);

            if (rolRelacionado.CodigoHomologacion == "KEY_USER_ONA" && rolUsuario.CodigoHomologacion == "KEY_USER_CAN")
            {
                // No tiene permisos, mostrar la modal
                modalMessage = "No tiene permisos para editar este usuario.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }



            if (usuario.IdONA != onaPais && rolRelacionado.CodigoHomologacion == "KEY_USER_ONA")
            {
                modalMessage = "No tiene permisos para editar este usuario porque no pertenece a este País.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            if (usuario.IdONA == onaPais && rolRelacionado.CodigoHomologacion == "KEY_USER_CAN")
            {
                modalMessage = "No tiene permisos para editar este usuario porque no pertenece a este País.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            if (rolRelacionado.CodigoHomologacion == "KEY_USER_ONA"   && usuario.IdONA == onaPais && rolUsuario.CodigoHomologacion != "KEY_USER_CAN")
            {
                // Navegar al editar usuario
                iNavigationManager.NavigateTo($"/editar-usuario/{usuario.IdUsuario}");
            }

            if (rolRelacionado.CodigoHomologacion == "KEY_USER_CAN")
            {
                // Navegar al editar usuario
                iNavigationManager.NavigateTo($"/editar-usuario/{usuario.IdUsuario}");
            }
        }

        private void CerrarModal()
        {
            showModal = false;
        }

        private async Task<GridDataProviderResult<UsuarioDto>> UsuariosDataProvider(GridDataProviderRequest<UsuarioDto> request)
        {
            if (listaUsuarios is null && iUsuariosService != null)
            {
                listaUsuarios = await iUsuariosService.GetUsuariosAsync();
            }

            return await Task.FromResult(request.ApplyTo(listaUsuarios ?? new List<UsuarioDto>()));
        }
    }
}
