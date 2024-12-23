using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using ClientApp.Models;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente Blazor que muestra los resultados de búsqueda en una grilla, interactuando con un servicio de búsqueda
    /// para obtener los datos basados en los parámetros de búsqueda proporcionados.
    /// </summary>
    public partial class ResultGrid
    {
        /// <summary>
        /// Parámetro que representa la solicitud de búsqueda, que contiene los filtros y el texto a buscar.
        /// </summary>
        [Parameter]
        public SolicitudBusqueda solicitudBusqueda { get; set; } = default!;

        /// <summary>
        /// Servicio de búsqueda inyectado para interactuar con los endpoints de búsqueda.
        /// </summary>
        [Inject]
        public IBusquedaService servicio { get; set; } = default!;

        /// <summary>
        /// Servicio de API inyectado para realizar llamadas a los endpoints.
        /// </summary>
        [Inject]
        public IApiService _apiService { get; set; } = default!;

        /// <summary>
        /// Servicio de notificaciones tipo Toast inyectado para mostrar mensajes al usuario.
        /// </summary>
        [Inject]
        protected ToastService ToastService { get; set; } = default!;

        private Modal modal = default!;
        public Grid<ResultDataPaBuscarPalabraDto>? grid;
        private List<HomologacionDto> listaEtiquetasGrilla = default!;
        private int totalCount = 0;

        /// <summary>
        /// Método que se ejecuta cuando el componente se inicializa. 
        /// Realiza una llamada al servicio API para obtener los esquemas para la grilla.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                listaEtiquetasGrilla = await _apiService.GetAsync<List<HomologacionDto>>("grid/schema");
            }
            catch (Exception e)
            {
                ToastService.Notify(new(ToastType.Danger, $"Error fetching filters: {e.Message}."));
            }
        }

        /// <summary>
        /// Método que obtiene los resultados de búsqueda paginados utilizando los parámetros de búsqueda de la solicitud.
        /// </summary>
        /// <param name="PageNumber">Número de página para la paginación.</param>
        /// <param name="PageSize">Número de resultados por página.</param>
        /// <returns>Una lista de resultados de búsqueda.</returns>
        private async Task<List<ResultDataPaBuscarPalabraDto>> BuscarEsquemas(int PageNumber, int PageSize)
        {
            var listBuscadorResultadoDataDto = new List<ResultDataPaBuscarPalabraDto>();
            try
            {
                if (servicio != null)
                {
                    // Prepara los filtros para la búsqueda
                    var filtros = new
                    {
                        ExactaBuscar = solicitudBusqueda.UseExactMatch,
                        TextoBuscar = solicitudBusqueda.TextoBusqueda ?? "",
                        FiltroPais = GetFiltrosPorId(2),
                        FiltroOna = GetFiltrosPorId(3),
                        FiltroNorma = GetFiltrosPorId(5),
                        FiltroEsquema = GetFiltrosPorId(4),
                        FiltroEstado = GetFiltrosPorId(6),
                        FiltroRecomocimiento = GetFiltrosPorId(7)
                    };

                    // Realiza la búsqueda
                    var result = await servicio.PsBuscarPalabraAsync(JsonConvert.SerializeObject(filtros), PageNumber, PageSize);

                    if (result?.Data != null)
                    {
                        listBuscadorResultadoDataDto = result.Data;
                    }

                    if (PageNumber == 1)
                    {
                        totalCount = result?.TotalCount ?? 0;
                    }
                }
            }
            catch (Exception e)
            {
                ToastService.Notify(new(ToastType.Danger, $"Error fetching filters: {e.Message}."));
            }

            return listBuscadorResultadoDataDto;
        }

        /// <summary>
        /// Método que obtiene los filtros según un ID específico.
        /// </summary>
        /// <param name="id">ID del filtro para obtener los datos correspondientes.</param>
        /// <returns>Una lista de filtros aplicados.</returns>
        private List<string> GetFiltrosPorId(int id)
        {
            return solicitudBusqueda.Filtros?.FirstOrDefault(c => c.Id == id)?
                .Seleccion?.Where(c => c != null).ToList() ?? new List<string>();
        }

        /// <summary>
        /// Método que proporciona los datos de búsqueda para la grilla. Se invoca al cambiar la página o tamaño de la grilla.
        /// </summary>
        /// <param name="request">La solicitud de la grilla que contiene la página y tamaño de página.</param>
        /// <returns>El resultado con los datos y el número total de registros.</returns>
        private async Task<GridDataProviderResult<ResultDataPaBuscarPalabraDto>> ResultadoBusquedaDataProvider(GridDataProviderRequest<ResultDataPaBuscarPalabraDto> request)
        {
            var data = await BuscarEsquemas(request.PageNumber, request.PageSize);
            return new GridDataProviderResult<ResultDataPaBuscarPalabraDto>
            {
                Data = data,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Muestra un modal con información detallada sobre un resultado de búsqueda seleccionado.
        /// </summary>
        /// <param name="resultData">Los datos del resultado seleccionado para mostrar en el modal.</param>
        private async void ShowModal(ResultDataPaBuscarPalabraDto resultData)
        {
            var parameters = new Dictionary<string, object>
            {
                { "resultData", resultData }
            };

            await modal.ShowAsync<EsquemaModal>("Información Detallada", parameters: parameters);
        }
    }
}
