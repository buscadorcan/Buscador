using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Services;
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
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        //[Inject]
        //protected IJSRuntime? JSRuntime { get; set; }
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        private IEnumerable<HomologacionDto> PaginatedItems => listaHomologacions
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => listaHomologacions.Count > 0 ? (int)Math.Ceiling((double)listaHomologacions.Count / PageSize) : 1;

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
            if (iCatalogosService != null)
            {
                listaHomologacions = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("grupos");
            }
            // Ajusta la paginación si la lista está vacía o cambia
            if (listaHomologacions.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }

            //DataLoaded += async () =>
            //{
            //    if (listaHomologacions != null && JSRuntime != null)
            //    {
            //        await Task.Delay(2000);
            //        await JSRuntime.InvokeVoidAsync("initSortable", DotNetObjectReference.Create(this));
            //    }
            //};
        }
        private async Task<GridDataProviderResult<HomologacionDto>> HomologacionDataProvider(GridDataProviderRequest<HomologacionDto> request)
        {
            if (iCatalogosService != null)
            {
                listaHomologacions = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("grupos");
            }

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
            if (listaHomologacions != null)
            {
                // Actualiza el orden en la lista local
                var ordenados = new List<HomologacionDto>();
                for (int i = 0; i < sortedIds.Length; i++)
                {
                    HomologacionDto? homo = listaHomologacions.FirstOrDefault(h => h.IdHomologacion == int.Parse(sortedIds[i]));
                    if (homo != null)
                    {
                        homo.MostrarWebOrden = i + 1; // Actualiza el orden en memoria
                        ordenados.Add(homo);
                        if (iHomologacionService != null)
                        {
                            await iHomologacionService.RegistrarOActualizar(homo); // Actualiza en el backend
                        }
                    }
                }

                // Reemplaza la lista original con la lista ordenada
                listaHomologacions = ordenados;

                // Refresca el grid
                if (grid != null)
                {
                    await grid.RefreshDataAsync();
                }
            }
        }

    }
}