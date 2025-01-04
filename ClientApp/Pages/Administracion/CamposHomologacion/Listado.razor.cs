using BlazorBootstrap;
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
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        protected override async Task OnInitializedAsync()
        {
            if (iCatalogosService != null)
            {
                listaVwHomologacion = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("grupo");

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
        private async Task OnDeleteClick(int IdHomologacion)
        {
            var respuesta = await iHomologacionService.EliminarHomologacion(IdHomologacion);
            if (respuesta.registroCorrecto) {
                await grid.RefreshDataAsync();
            }
        }
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
    }
}