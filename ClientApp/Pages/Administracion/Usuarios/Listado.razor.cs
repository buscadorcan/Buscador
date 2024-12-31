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
        private int rol;
        private int onaPais;
        [Inject]
        IUsuariosService? iUsuariosService { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        [Inject]
        private NavigationManager iNavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Obtener el rol del usuario desde LocalStorage  
            rol = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Rol_Local);
            onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            listaUsuarios = await iUsuariosService.GetUsuariosAsync();

            // Puedes establecer isRol17 si es necesario aquí, dependiendo de la lógica de tu aplicación  
            isRol17 = rol == 17; // Ajusta según la lógica de tu aplicación  
        }

        private void EditarUsuario(UsuarioDto usuario)
        {
            if (rol == 16 && usuario.Rol == "UsuarioMaster")
            {
                // No tiene permisos, mostrar la modal
                modalMessage = "No tiene permisos para editar este usuario.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            if (usuario.IdONA != onaPais)
            {
                modalMessage = "No tiene permisos para editar este usuario porque no pertenece a este País.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            else
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
