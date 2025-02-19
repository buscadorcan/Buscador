using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Esquemas
{
    public partial class Listado
    {
        private Modal modal = default!;
        private Grid<EsquemaDto>? grid;
        public event Action? DataLoaded;
        private bool deleteshowModal; 
        private string modalMessage;
        private int? selectedIdEsquema;
        [Inject]
        public Services.ToastService? toastService { get; set; }
        private IEnumerable<EsquemaDto>? listaEsquemas;
        [Inject]
        private IEsquemaService? iEsquemaService { get; set; }
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        [Inject]
        public IHomologacionService? HomologacionService { get; set; }
        private List<HomologacionDto>? listaVwHomologacion;
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
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
        private async Task<GridDataProviderResult<EsquemaDto>> EsquemasDataProvider(GridDataProviderRequest<EsquemaDto> request)
        {
            if (iEsquemaService != null)
            {
                listaEsquemas = await iEsquemaService.GetListEsquemasAsync();
            }

            DataLoaded?.Invoke();

            return await Task.FromResult(request.ApplyTo(listaEsquemas ?? []));
        }
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
        private async Task OnDeleteClick(int IdEsquema)
        {
            if (iEsquemaService != null && listaEsquemas != null && grid != null)
            {
                var respuesta = await iEsquemaService.DeleteEsquemaAsync(IdEsquema);
                if (respuesta) {
                    listaEsquemas = listaEsquemas.Where(c => c.IdEsquema != IdEsquema);
                    await grid.RefreshDataAsync();
                }
            }
        }
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

        private void OpenDeleteModal(int idOna)
        {
            selectedIdEsquema = idOna;
            deleteshowModal = true;
        }

        // Cierra el modal
        private void CloseModal()
        {
            selectedIdEsquema = null;
            deleteshowModal = false;
        }

        // Confirmar eliminación del registro
        private async Task ConfirmDelete()
        {
            objEventTracking.NombrePagina = "Administación de Homologación Esquemas";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "ConfirmDelete";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Local) + ' ' + iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Apellido_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
            objEventTracking.ParametroJson = "";
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