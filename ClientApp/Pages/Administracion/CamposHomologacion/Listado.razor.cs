using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Pages.Administracion.Esquemas;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.CamposHomologacion
{
    public partial class Listado
    {
        // Elemento seleccionado en la lista de homologaciones
        private HomologacionDto? homologacionSelected;
        // Componente de la grilla para mostrar los datos
        private Grid<HomologacionDto>? grid;
        // Lista de homologaciones obtenidas
        private List<HomologacionDto>? listaHomologacions = new List<HomologacionDto>();
        // Servicio inyectado para acceder a los catálogos
        [Inject]
        private ICatalogosService? iCatalogosService { get; set; }
        // Servicio inyectado para gestionar homologaciones
        [Inject]
        private IHomologacionService? iHomologacionService { get; set; }
        // Lista de visualización de homologaciones
        private List<HomologacionDto>? listaVwHomologacion;
        // Evento que se activa cuando los datos se han cargado
        public event Action? DataLoaded;
        // ID de la homologación seleccionada para operaciones
        private int? selectedIdHomologacion;    // Almacena el ID de la homologación seleccionado
        // Control de visibilidad del modal
        private bool showModal; // Controlar la visibilidad de la ventana modal  
        // Bandera para determinar si se está agregando un nuevo elemento
        private bool IsAdd;
        // Mensaje del modal
        private string modalMessage;
        // Servicio de notificaciones Toast
        [Inject]
        public Services.ToastService? toastService { get; set; }
        // Servicio de interoperabilidad con JavaScript
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        // Servicio de búsqueda
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();
        // Servicio de almacenamiento local en el navegador
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }

        /// <summary>
        /// Método asincrónico que inicializa la lista de campos de homologación.
        /// </summary>
        /// <returns>Devuelve la lista de homologaciones al iniciar la aplicación.</returns>
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.NombrePagina = "/campos-homologacion";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "campos-homologacion";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (iCatalogosService != null)
            {
                listaVwHomologacion = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("grupos");

                DataLoaded += async () => {
                    if (!(listaHomologacions is null) && JSRuntime != null) {
                        await Task.Delay(2000);
                        await JSRuntime.InvokeVoidAsync("initSortable", DotNetObjectReference.Create(this));
                    }
                };
            }
        }

        /// <summary>
        /// Método que obtiene la lista de homologaciones y aplica los criterios de filtrado.
        /// </summary>
        /// <returns>Lista de homologaciones aplicando los filtros.</returns>
        private async Task<GridDataProviderResult<HomologacionDto>> HomologacionDataProvider(GridDataProviderRequest<HomologacionDto> request)
        {
            if (homologacionSelected != null)
            {
                IsAdd = homologacionSelected.CodigoHomologacion == "KEY_DIM_ESQUEMA";
                //listaHomologacions = await iHomologacionService.GetHomologacionsAsync();
                listaHomologacions = await iHomologacionService.GetHomologacionsSelectAsync(homologacionSelected.CodigoHomologacion);
            }
            //IsAdd = true;
            DataLoaded?.Invoke();

            return await Task.FromResult(request.ApplyTo(listaHomologacions ?? []));
        }

        /// <summary>
        /// Método que maneja el cambio de selección en un elemento de autocompletado.
        /// </summary>
        /// <param name="e">Evento de cambio.</param>
        private async Task OnAutoCompleteChangedHandler(ChangeEventArgs e)
        {
            
            // Obtén el ID seleccionado desde el <select>
            var selectedId = Convert.ToInt32(e.Value);

            // Busca el elemento correspondiente en la lista
            var selectedHomologacion = listaVwHomologacion?.FirstOrDefault(h => h.IdHomologacion == selectedId);

            // Si se encuentra, actualiza la selección
            if (selectedHomologacion != null)
            {
                homologacionSelected = selectedHomologacion;
                IsAdd = homologacionSelected.CodigoHomologacion == "KEY_DIM_ESQUEMA";
                // Refresca la grilla si es necesario
                if (grid != null)
                {
                    await grid.RefreshDataAsync();
                }
            }
        }

        /// <summary>
        /// Método invocable desde JavaScript para actualizar el orden de los elementos en la lista al arrastrar.
        /// </summary>
        /// <param name="sortedIds">Lista de identificadores ordenados.</param>

        [JSInvokable]
        public async Task OnDragEnd(string[] sortedIds)
        {
            for (int i = 0; i < sortedIds.Length; i += 1)
            {
                HomologacionDto homo = listaHomologacions.FirstOrDefault(h => h.IdHomologacion == int.Parse(sortedIds[i]));
                if (homo != null && homo.MostrarWebOrden != i + 1)
                {
                    homo.MostrarWebOrden = i + 1;
                    await iHomologacionService.RegistrarOActualizar(homo);
                }
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Método que abre el modal de confirmación de eliminación.
        /// </summary>
        /// <param name="idHomologacion">ID de la homologación a eliminar.</param>

        private void OpenDeleteModal(int idHomologacion)
        {
            selectedIdHomologacion = idHomologacion;
            showModal = true;
        }

        /// <summary>
        /// Método que cierra el modal de confirmación de eliminación.
        /// </summary>
        private void CloseModal()
        {
            selectedIdHomologacion = null;
            showModal = false;
        }

        /// <summary>
        /// Método que confirma la eliminación de una homologación.
        /// </summary>
        private async Task ConfirmDelete()
        {
            objEventTracking.NombrePagina = "/campos-homologacion";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "btnEliminar";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (selectedIdHomologacion.HasValue && iHomologacionService != null)
            {
                var result = await iHomologacionService.DeleteHomologacionAsync(selectedIdHomologacion.Value);
                if (result)
                {
                    CloseModal(); // Cierra el modal
                    toastService?.CreateToastMessage(ToastType.Success, "Registro eliminado exitosamente.");
                    await LoadHomologacion(); // Actualiza la lista
                    await grid?.RefreshDataAsync(); //resfresca la grilla
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar el registro.");
                }
            }

        }

        /// <summary>
        /// Método que carga la lista de homologaciones.
        /// </summary>
        private async Task LoadHomologacion() 
        {
            if (iHomologacionService != null)
            {
                listaHomologacions = await iHomologacionService.GetHomologacionsAsync();
            }
        }
    }
}