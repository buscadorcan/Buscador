using BlazorBootstrap;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using Infractruture.Services;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedApp.Dtos;

namespace ClientApp.Pages.Administracion.Esquemas
{
    /// <summary>
    /// Página de listado de esquemas de homologación.
    /// Permite la gestión de esquemas, incluyendo la visualización, ordenamiento y eliminación.
    /// </summary>
    public partial class Listado
    {
        // Modal para la visualización de detalles
        private Modal modal = default!;
        // Grilla para mostrar la lista de esquemas
        private Grid<EsquemaDto>? grid;
        // Evento que se dispara cuando los datos han sido cargados
        public event Action? DataLoaded;
        // Control de visibilidad del modal de eliminación
        private bool deleteshowModal;
        // Mensaje del modal
        private string modalMessage;
        // ID del esquema seleccionado para eliminación
        private int? selectedIdEsquema;
        // Servicio de notificaciones Toast
        [Inject]
        public Infractruture.Services.ToastService? toastService { get; set; }
        // Lista de esquemas disponibles
        private IEnumerable<EsquemaDto>? listaEsquemas;
        // Servicio de esquemas inyectado
        [Inject]
        private IEsquemaService? iEsquemaService { get; set; }
        // Servicio de interoperabilidad con JavaScript
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        // Servicio de homologaciones inyectado
        [Inject]
        public IHomologacionService? HomologacionService { get; set; }
        // Servicio de homologaciones inyectado
        private List<HomologacionDto>? listaVwHomologacion;
        // Servicio de búsqueda inyectado
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Servicio de almacenamiento local
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();


        /// <summary>
        /// Método asincrónico que inicializa la lista de esquemas y la configuración de JavaScript.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/esquemas";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "esquemas";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (HomologacionService != null)
            {
                listaVwHomologacion = await HomologacionService.GetHomologacionsAsync();
            }

            DataLoaded += async () => {
                if (listaEsquemas != null && JSRuntime != null) {
                    await Task.Delay(2000);
                    await JSRuntime.InvokeVoidAsync("initSortable", DotNetObjectReference.Create(this));
                }
            };
        }

        /// <summary>
        /// Método que obtiene la lista de esquemas y aplica la paginación.
        /// </summary>
        /// <returns>Lista de esquemas.</returns>
        private async Task<GridDataProviderResult<EsquemaDto>> EsquemasDataProvider(GridDataProviderRequest<EsquemaDto> request)
        {
            if (iEsquemaService != null)
            {
                listaEsquemas = await iEsquemaService.GetListEsquemasAsync();
            }

            DataLoaded?.Invoke();

            return await Task.FromResult(request.ApplyTo(listaEsquemas ?? []));
        }

        /// <summary>
        /// Método invocable desde JavaScript para actualizar el orden de los esquemas.
        /// </summary>
        /// <param name="sortedIds">Lista ordenada de IDs de esquemas.</param>
        [JSInvokable]
        public async Task OnDragEnd(string[] sortedIds)
        {
            if (iEsquemaService != null)
            {
                for (int i = 0; i < sortedIds.Length; i += 1)
                {
                    EsquemaDto? homo = listaEsquemas?.FirstOrDefault(h => h.IdEsquema == int.Parse(sortedIds[i]));
                    if (homo != null && homo.MostrarWebOrden != i + 1)
                    {
                        homo.MostrarWebOrden = i + 1;
                        await iEsquemaService.RegistrarEsquemaActualizar(homo);
                    }
                }
            }
            if (grid != null)
            {
                await grid.RefreshDataAsync();
            }
            else
            {
                await Task.CompletedTask;
            }
        }

        /// <summary>
        /// Método que muestra el modal con detalles del esquema seleccionado.
        /// </summary>
        /// <param name="IdEsquema">ID del esquema seleccionado.</param>

        private async void showModal(int IdEsquema)
        {
            if (listaEsquemas != null)
            {
                var homo = listaEsquemas.FirstOrDefault(c => c.IdEsquema == IdEsquema);
                var columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homo?.EsquemaJson ?? "[]");

                var parameters = new Dictionary<string, object>();
                parameters.Add("columnas", columnas ?? []);
                parameters.Add("listaVwHomologacion", listaVwHomologacion ?? []);
                await modal.ShowAsync<RowModal>(title: $"{homo?.MostrarWeb}", parameters: parameters);
            }
        }

        /// <summary>
        /// Método que abre el modal de confirmación de eliminación.
        /// </summary>
        /// <param name="IdEsquema">ID del esquema a eliminar.</param>
        private void OpenDeleteModal(int idOna)
        {
            selectedIdEsquema = idOna;
            deleteshowModal = true;
        }

        /// <summary>
        /// Método que cierra el modal de eliminación.
        /// </summary>
        private void CloseModal()
        {
            selectedIdEsquema = null;
            deleteshowModal = false;
        }

        /// <summary>
        /// Método que confirma la eliminación de un esquema.
        /// </summary>
        private async Task ConfirmDelete()
        {
            objEventTracking.CodigoHomologacionMenu = "/esquemas";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "btnEliminar";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (selectedIdEsquema.HasValue && iEsquemaService != null)
            {
                int idEsquema = selectedIdEsquema.Value;
                var respuesta = await iEsquemaService.DeleteEsquemaAsync(selectedIdEsquema.Value);
                if (respuesta)
                {
                    CloseModal();
                    listaEsquemas = listaEsquemas.Where(c => c.IdEsquema != idEsquema);
                    await LoadEsquemas();
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar el registro.");
                }
            }
        }

        /// <summary>
        /// Método que carga la lista de esquemas desde la base de datos.
        /// </summary>
        private async Task LoadEsquemas()
        {
            if (iEsquemaService != null)
            {
                listaEsquemas = await iEsquemaService.GetListEsquemasAsync();
            }
            if (grid != null)
            {
                await grid.RefreshDataAsync();
            }
        }
    }
}