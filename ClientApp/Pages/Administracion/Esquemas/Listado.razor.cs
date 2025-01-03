using BlazorBootstrap;
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
        private IEnumerable<EsquemaDto>? listaEsquemas;
        [Inject]
        private IEsquemaService? iEsquemaService { get; set; }
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        [Inject]
        public IHomologacionService? HomologacionService { get; set; }
        private List<HomologacionDto>? listaVwHomologacion;

        protected override async Task OnInitializedAsync()
        {
            if (HomologacionService != null)
            {
                listaVwHomologacion = await HomologacionService.GetHomologacionsAsync(1);
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
    }
}