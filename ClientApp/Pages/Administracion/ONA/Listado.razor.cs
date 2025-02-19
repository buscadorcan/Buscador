using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;
using System.Text;

namespace ClientApp.Pages.Administracion.ONA
{
    public partial class Listado
    {
        private bool showModal = false; // Controla la visibilidad del modal
        private int? selectedIdONA;    // Almacena el ID del ONA seleccionado
        private List<OnaDto>? listaONAs; // Lista de registros ONAs
        private Button saveButton = default!;
        private Grid<OnaDto>? grid;
        [Inject]
        public IONAService? iONAservice { get; set; }

        [Inject]
        public Services.ToastService? toastService { get; set; }

        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        private bool isRolAdmin;

        // Método para cargar la lista de ONAs
        private async Task LoadONAs()
        {
            if (iONAservice != null)
            {
                var rolRelacionado = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                isRolAdmin = rolRelacionado == "KEY_USER_CAN";
                if (isRolAdmin)
                {
                    listaONAs = await iONAservice.GetONAsAsync() ?? new List<OnaDto>();
                }
                else
                {
                    int IdOna = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
                    listaONAs = await iONAservice.GetListByONAsAsync(IdOna) ?? new List<OnaDto>();
                }
            }
            // Ajusta la paginación si la lista está vacía o cambia
            if (listaONAs.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }
        //private List<LogMigracionDto> listasHevd = new();
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        private IEnumerable<OnaDto> PaginatedItems => listaONAs
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => listaONAs.Count > 0 ? (int)Math.Ceiling((double)listaONAs.Count / PageSize) : 1;

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
        // Proveedor de datos para el grid
        private async Task<GridDataProviderResult<OnaDto>> ONAsDataProvider(GridDataProviderRequest<OnaDto> request)
        {
            try
            {
                if (listaONAs is null && iONAservice != null)
                {
                    await LoadONAs(); // Carga los datos si aún no están cargados
                }

                return await Task.FromResult(request.ApplyTo(listaONAs ?? new List<OnaDto>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Abre el modal
        private void OpenDeleteModal(int idONA)
        {
            selectedIdONA = idONA;
            showModal = true;
        }

        // Cierra el modal
        private void CloseModal()
        {
            selectedIdONA = null;
            showModal = false;
        }

        // Confirmar eliminación del registro
        private async Task ConfirmDelete()
        {
            objEventTracking.NombrePagina = "Administración de ONA";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "ConfirmDelete";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Local) + ' ' + iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Apellido_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (selectedIdONA.HasValue && iONAservice != null)
            {
                var result = await iONAservice.DeleteONAAsync(selectedIdONA.Value);

                if (result)
                {
                    CloseModal(); // Cierra el modal
                    toastService?.CreateToastMessage(ToastType.Success, "Registro eliminado exitosamente.");
                    await LoadONAs(); // Actualiza la lista
                    await grid?.RefreshDataAsync(); //resfresca la grilla
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar el registro.");
                }
            }

        }

        // Método de inicialización
        protected override async Task OnInitializedAsync()
        {
            await LoadONAs(); // Carga la lista al iniciar el componente
        }

    }
}
