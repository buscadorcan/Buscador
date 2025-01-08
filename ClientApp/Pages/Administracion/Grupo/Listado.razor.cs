using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Grupo
{
    public partial class Listado
    {
        private Grid<HomologacionDto>? grid;
        private List<HomologacionDto>? listaHomologacions = new List<HomologacionDto>();
        [Inject]
        private ICatalogosService? iCatalogosService { get; set; }
        [Inject]
        private IHomologacionService? iHomologacionService { get; set; }
        public event Action? DataLoaded;
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        protected override async Task OnInitializedAsync()
        {
            DataLoaded += async () => {
                if (!(listaHomologacions is null) && JSRuntime != null) {
                    await Task.Delay(2000);
                    await JSRuntime.InvokeVoidAsync("initSortable", DotNetObjectReference.Create(this));
                }
            };
        }
        private async Task<GridDataProviderResult<HomologacionDto>> HomologacionDataProvider(GridDataProviderRequest<HomologacionDto> request)
        {
            if (iCatalogosService != null)
                listaHomologacions = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("grupos");

            DataLoaded?.Invoke();

            return await Task.FromResult(request.ApplyTo(listaHomologacions ?? []));
        }
        private async Task OnDeleteClick(int IdHomologacion)
        {
            if (iHomologacionService != null)
            {
                var respuesta = await iHomologacionService.EliminarHomologacion(IdHomologacion);
                if (respuesta.registroCorrecto && grid != null) {
                    await grid.RefreshDataAsync();
                }
            }
        }
        [JSInvokable]
        public async Task OnDragEnd(string[] sortedIds)
        {
            for (int i = 0; i < sortedIds.Length; i += 1)
            {
                HomologacionDto? homo = listaHomologacions?.FirstOrDefault(h => h.IdHomologacion == int.Parse(sortedIds[i] ?? ""));
                if (homo != null && homo.MostrarWebOrden != i + 1)
                {
                    homo.MostrarWebOrden = i + 1;
                    if (iHomologacionService != null)
                        await iHomologacionService.RegistrarOActualizar(homo);
                }
            }
            await Task.CompletedTask;
        }
    }
}