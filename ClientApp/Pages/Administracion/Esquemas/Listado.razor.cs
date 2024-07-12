using BlazorBootstrap;
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
        private Grid<HomologacionEsquemaDto>? grid;
        public event Action? DataLoaded;
        private IEnumerable<HomologacionEsquemaDto>? listaHomologacionEsquemas;
        [Inject]
        private IHomologacionEsquemaService? iHomologacionEsquemaService { get; set; }
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        [Inject]
        public ICatalogosService? vwHomologacionRepository { get; set; }
        private List<HomologacionDto>? listaVwHomologacion;

        protected override async Task OnInitializedAsync()
        {
            if (vwHomologacionRepository != null)
            {
                listaVwHomologacion = await vwHomologacionRepository.GetHomologacionAsync<List<HomologacionDto>>("dimension");
            }

            DataLoaded += async () => {
                if (listaHomologacionEsquemas != null && JSRuntime != null) {
                    await Task.Delay(2000);
                    await JSRuntime.InvokeVoidAsync("initSortable", DotNetObjectReference.Create(this));
                }
            };
        }
        private async Task<GridDataProviderResult<HomologacionEsquemaDto>> EsquemasDataProvider(GridDataProviderRequest<HomologacionEsquemaDto> request)
        {
            if (iHomologacionEsquemaService != null)
            {
                listaHomologacionEsquemas = await iHomologacionEsquemaService.GetHomologacionEsquemasAsync();
            }

            DataLoaded?.Invoke();

            return await Task.FromResult(request.ApplyTo(listaHomologacionEsquemas ?? []));
        }
        [JSInvokable]
        public async Task OnDragEnd(string[] sortedIds)
        {
            if (iHomologacionEsquemaService != null)
            {
                for (int i = 0; i < sortedIds.Length; i += 1)
                {
                    HomologacionEsquemaDto? homo = listaHomologacionEsquemas?.FirstOrDefault(h => h.IdHomologacionEsquema == int.Parse(sortedIds[i]));
                    if (homo != null && homo.MostrarWebOrden != i + 1)
                    {
                        homo.MostrarWebOrden = i + 1;
                        await iHomologacionEsquemaService.RegistrarOActualizar(homo);
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
        private async Task OnDeleteClick(int IdHomologacionEsquema)
        {
            if (iHomologacionEsquemaService != null && listaHomologacionEsquemas != null && grid != null)
            {
                var respuesta = await iHomologacionEsquemaService.EliminarHomologacionEsquema(IdHomologacionEsquema);
                if (respuesta.registroCorrecto) {
                    listaHomologacionEsquemas = listaHomologacionEsquemas.Where(c => c.IdHomologacionEsquema != IdHomologacionEsquema);
                    await grid.RefreshDataAsync();
                }
            }
        }
        private async void showModal(int IdHomologacionEsquema)
        {
            if (listaHomologacionEsquemas != null)
            {
                var homo = listaHomologacionEsquemas.FirstOrDefault(c => c.IdHomologacionEsquema == IdHomologacionEsquema);
                var columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homo?.EsquemaJson ?? "[]");

                var parameters = new Dictionary<string, object>();
                parameters.Add("columnas", columnas ?? []);
                parameters.Add("listaVwHomologacion", listaVwHomologacion ?? []);
                await modal.ShowAsync<RowModal>(title: $"{homo?.MostrarWeb}", parameters: parameters);
            }
        }
    }
}