using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Pages.Administracion.Esquemas;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Usuarios
{
    public partial class Listado
    {
        private List<UsuarioDto>? listaUsuarios;
        private bool isRolRead; // Variable para controlar la visibilidad del botón
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

        private Button saveButton = default!;
        private Grid<UsuarioDto>? grid;
        private int? selectedIdUsuario;    // Almacena el ID del usuario seleccionado
        [Inject]
        public Services.ToastService? toastService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        private IEnumerable<UsuarioDto> PaginatedItems => listaUsuarios
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => listaUsuarios.Count > 0 ? (int)Math.Ceiling((double)listaUsuarios.Count / PageSize) : 1;

        private bool CanGoPrevious => CurrentPage > 1;
        private bool CanGoNext => CurrentPage < TotalPages;

        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }
        protected override async Task OnInitializedAsync()
        {

            rolCargo = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Rol_Local);
            onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            listaUsuarios = await iUsuariosService.GetUsuariosAsync();

            //listaRoles = await iUsuariosService.GetRolesAsync();
            listaOna = await iUsuariosService.GetOnaAsync();

            //var rolRelacionado = listaRoles.FirstOrDefault(rol => rol.IdHomologacionRol == rolCargo);
            var rolRelacionado = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            var onaRelacionado = listaOna.FirstOrDefault(ona => ona.IdONA == onaPais);
 
            isRolRead = rolRelacionado == "KEY_USER_READ";

            // Ajusta la paginación si la lista está vacía o cambia
            if (listaUsuarios.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }
        private async void EditarUsuario(UsuarioDto usuario)
        {

            rolCargo = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Rol_Local);
            onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            listaUsuarios = await iUsuariosService.GetUsuariosAsync();

            listaRoles = await iUsuariosService.GetRolesAsync();
            listaOna = await iUsuariosService.GetOnaAsync();

            //var rolRelacionado = listaRoles.FirstOrDefault(rol => rol.IdHomologacionRol == rolCargo);
            var rolRelacionado = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            var onaRelacionado = listaOna.FirstOrDefault(ona => ona.IdONA == onaPais);
            var Homolog = listaUsuarios.FirstOrDefault(usu => usu.IdUsuario == usuario.IdUsuario);
            var rolUsuario = listaRoles.FirstOrDefault(rol => rol.IdHomologacionRol == Homolog.IdHomologacionRol);

            if (rolRelacionado == "KEY_USER_ONA" && rolUsuario.CodigoHomologacion == "KEY_USER_CAN")
            {
                // No tiene permisos, mostrar la modal
                modalMessage = "No tiene permisos para editar este usuario.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            if (usuario.IdONA != onaPais && rolRelacionado == "KEY_USER_ONA")
            {
                modalMessage = "No tiene permisos para editar este usuario porque no pertenece a este País.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            if (usuario.IdONA == onaPais && rolRelacionado == "KEY_USER_CAN")
            {
                modalMessage = "No tiene permisos para editar este usuario porque no pertenece a este País.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            if (rolRelacionado == "KEY_USER_ONA"   && usuario.IdONA == onaPais && rolUsuario.CodigoHomologacion != "KEY_USER_CAN")
            {
                // Navegar al editar usuario
                iNavigationManager.NavigateTo($"/editar-usuario/{usuario.IdUsuario}");
            }

            if (rolRelacionado == "KEY_USER_CAN")
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
        // Abre el modal
        private void OpenDeleteModal(int idUsuario)
        {
            selectedIdUsuario = idUsuario;
            showModal = true;
        }

        // Cierra el modal
        private void CloseModal()
        {
            selectedIdUsuario = null;
            showModal = false;
        }

        // Confirmar eliminación del registro
        private async Task ConfirmDelete()
        {
            objEventTracking.NombrePagina = "Administración de Usuarios";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "ConfirmDelete";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Local) + ' ' + iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Apellido_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);
            if (selectedIdUsuario.HasValue && iUsuariosService != null)
            {
                var result = await iUsuariosService.DeleteUsuarioAsync(selectedIdUsuario.Value);
                if (result)
                {
                    CloseModal(); // Cierra el modal
                    toastService?.CreateToastMessage(ToastType.Success, "Registro eliminado exitosamente.");
                    await LoadUsuarios(); // Actualiza la lista
                    //await grid?.RefreshDataAsync(); //resfresca la grilla
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar el registro.");
                }
            }

        }
        // Método para cargar la lista de Usuarios
        private async Task LoadUsuarios()
        {
            if (iUsuariosService != null)
            {
                listaUsuarios = await iUsuariosService.GetUsuariosAsync();
            }
        }
    }
}
