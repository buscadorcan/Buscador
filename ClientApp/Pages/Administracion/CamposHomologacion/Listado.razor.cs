using BlazorBootstrap;
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
        private HomologacionDto? homologacionSelected;
        private Grid<HomologacionDto>? grid;
        private List<HomologacionDto>? listaHomologacions = new List<HomologacionDto>();
        [Inject]
        private ICatalogosService? iCatalogosService { get; set; }
        [Inject]
        private IHomologacionService? iHomologacionService { get; set; }
        private List<HomologacionDto>? listaVwHomologacion;
        public event Action? DataLoaded;

        private int? selectedIdHomologacion;    // Almacena el ID de la homologación seleccionado
        private bool showModal; // Controlar la visibilidad de la ventana modal  
        private string modalMessage;
        [Inject]
        public Services.ToastService? toastService { get; set; }

        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        protected override async Task OnInitializedAsync()
        {
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
        private async Task<GridDataProviderResult<HomologacionDto>> HomologacionDataProvider(GridDataProviderRequest<HomologacionDto> request)
        {
            if (homologacionSelected != null)
            {
                listaHomologacions = await iHomologacionService.GetHomologacionsAsync();
            }

            DataLoaded?.Invoke();

            return await Task.FromResult(request.ApplyTo(listaHomologacions ?? []));
        }
        //private async Task OnDeleteClick(int IdHomologacion)
        //{
        //    var respuesta = await iHomologacionService.EliminarHomologacion(IdHomologacion);
        //    if (respuesta.registroCorrecto) {
        //        await grid.RefreshDataAsync();
        //    }
        //}
        private async void OnAutoCompleteChanged(HomologacionDto _vwHomologacionSelected)
        {
            homologacionSelected = _vwHomologacionSelected;
            await grid.RefreshDataAsync();
        }
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
        private string getNameDefault() {
            return homologacionSelected?.MostrarWeb ?? "Seleccione Grupo de Homologación";
        }
        // Abre el modal
        private void OpenDeleteModal(int idHomologacion)
        {
            selectedIdHomologacion = idHomologacion;
            showModal = true;
        }

        // Cierra el modal
        private void CloseModal()
        {
            selectedIdHomologacion = null;
            showModal = false;
        }

        // Confirmar eliminación del registro
        private async Task ConfirmDelete()
        {
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
        private async Task LoadHomologacion() 
        {
            if (iHomologacionService != null)
            {
                listaHomologacions = await iHomologacionService.GetHomologacionsAsync();
            }
        }
    }
}