using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para la página de búsqueda de CAN.
    /// </summary>
    public partial class Index : ComponentBase
    {
        /// <summary>
        /// Servicio para obtener catálogos.
        /// </summary>
        [Inject] public ICatalogosService? iCatalogosService { get; set; }

        /// <summary>
        /// servicio de busqueda
        /// </summary>
        [Inject] public IBusquedaService? iservicio { get; set; }

        /// <summary>
        /// servicio de busqueda
        /// </summary>
        [Inject] public IONAService? iOnaService { get; set; }

        /// <summary>
        /// Gets or sets the total items.
        /// </summary>
        private int TotalItems = 0;

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        private int TotalPages = 0;

        /// <summary>
        /// Gets or sets the display pages.
        /// </summary>
        private int DisplayPages = 5;

        /// <summary>
        /// Gets or sets the active page number.
        /// </summary>
        private int ActivePageNumber = 1;

        /// <summary>
        /// Lista de valores seleccionados
        /// </summary>
        private List<FiltrosBusquedaSeleccion> selectedFilter = new List<FiltrosBusquedaSeleccion>();

        /// <summary>
        /// Lista de urls devuerltos por el servicio
        /// </summary>
        private Dictionary<int, string> iconUrls = new();

        /// <summary>
        /// Variable para almacenar la información de la ONA.
        /// </summary>
        private OnaDto? OnaDto;

        /// <summary>
        /// Lista de resultados de la búsqueda.
        /// </summary>
        private List<BuscadorResultadoDataDto> listBuscadorResultadoDataDto = new List<BuscadorResultadoDataDto>();

        /// <summary>
        /// Método para mostrar el resultados
        /// </summary>
        private bool isGridVisible = true;

        /// <summary>
        /// Texto de busqueda
        /// </summary>
        private string searchText = string.Empty;

        /// <summary>
        /// Variable para indicar si la busqueda es exacta
        /// </summary>
        private bool isExactSearch = false;

        /// <summary>
        /// Gets or sets informations ONA.
        /// </summary>
        private List<vwPanelONADto>? PanelONA = new List<vwPanelONADto>();

        /// <summary>
        /// Listado de etiquetas de la grilla.
        /// </summary>
        private List<VwGrillaDto>? listaEtiquetasGrilla;

        /// <summary>
        /// Listado de etiquetas de la grilla.
        /// </summary>
        private List<VwGrillaDto>? listaEtiquetasCards;

        [Inject]
        private IBusquedaService iBusquedaService { get; set; }

        private EventTrackingDto objEventTracking { get; set; } = new();

        private object filtros {get; set;} 

        /// <summary>
        /// Método de inicialización del componente.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (iCatalogosService != null)
                {
                    listaEtiquetasGrilla = await iCatalogosService.GetHomologacionAsync<List<VwGrillaDto>>("grid/schema");
                    var ordenPersonalizado = new Dictionary<int, int>
                    {
                        { 84, 1 },
                        { 78, 2 },
                        { 82, 3 }, // Eliminado el duplicado
                        { 83, 4 },
                        { 90, 5 },
                        { 93, 6 },
                        { 81, 7 },
                        { 92, 8 },
                        { 91, 9 }
                    };

                    listaEtiquetasCards = listaEtiquetasGrilla
                                 ?.OrderBy(x => ordenPersonalizado.ContainsKey(x.IdHomologacion) ? ordenPersonalizado[x.IdHomologacion] : int.MaxValue)
                                 .ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void HandlePanelONAChange (List<vwPanelONADto> newPanelONA)
        {
            var nroOrg = newPanelONA.Sum(x => x.NroOrg);
            newPanelONA.Insert(0, new vwPanelONADto { NroOrg = nroOrg, Pais = "Total" });
            PanelONA = newPanelONA;
        }

        /// <summary>
        /// Método para manejar el cambio de filtros.
        /// </summary>
        private void HandleFilterChange(List<FiltrosBusquedaSeleccion> newFilter)
        {
            selectedFilter = newFilter;
        }

        /// <summary>
        /// Servicio de catálogos inyectado.
        /// </summary>
        /// <param name="_searchText"></param>
        /// <param name="_isExactSearch"></param>
        /// <returns></returns>
        private async Task onClickSearch(string _searchText, bool _isExactSearch)
        {
            searchText = _searchText;
            isExactSearch = _isExactSearch;
            ActivePageNumber = 1;
            await BuscarEsquemas(_searchText, _isExactSearch);

            // Registra los Eventos
            registeEvent("BuscarEsquema", "Buscar", "KEY_USER_SEARCH", JsonConvert.SerializeObject(filtros));

        }

        /// <summary>
        /// Método para manejar el cambio de página.
        /// </summary>
        private async Task ActivePageNumberChanged(int pageNumber)
        {
            ActivePageNumber = pageNumber;
            await BuscarEsquemas(searchText, isExactSearch);
        }

        /// <summary>
        /// Método para manejar el cambio de cantidad de items por páginas a mostrar.
        /// </summary>
        private async Task DisplayPagesChanged(int displayPages)
        {
            ActivePageNumber = 1;
            DisplayPages = displayPages;
            await BuscarEsquemas(searchText, isExactSearch);
        }

        /// <summary>
        /// Método para manejar el cambio de visibilidad de la grilla.
        /// </summary>
        private async Task isGridVisibleChanged(bool isVisible)
        {
            isGridVisible = isVisible;
        }

        /// <summary>
        /// Método para realizar la busqueda correspondiente
        /// </summary>
        private async Task<List<BuscadorResultadoDataDto>> BuscarEsquemas(string searchText, bool isExactSearch)
        {
            try
            {
                if (iservicio != null)
                {
                    filtros = new
                    {
                        ExactaBuscar = isExactSearch,
                        TextoBuscar = searchText ?? "",
                        FiltroPais = selectedFilter?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_PAI")?.Seleccion ?? new List<string>(),
                        FiltroOna = selectedFilter?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ONA")?.Seleccion ?? new List<string>(),
                        FiltroNorma = selectedFilter?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_NOR")?.Seleccion ?? new List<string>(),
                        FiltroEsquema = selectedFilter?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ESQ")?.Seleccion ?? new List<string>(),
                        FiltroEstado = selectedFilter?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_EST")?.Seleccion ?? new List<string>(),
                        FiltroRecomocimiento = selectedFilter?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_REC")?.Seleccion ?? new List<string>()
                    };

                    var result = await iservicio.PsBuscarPalabraAsync(JsonConvert.SerializeObject(filtros), ActivePageNumber, DisplayPages);

                    if (!(result.Data is null))
                    {
                        listBuscadorResultadoDataDto = result.Data;

                        // Prepara las URLs de los íconos
                        foreach (var item in listBuscadorResultadoDataDto)
                        {
                            if (item.IdONA.HasValue && !iconUrls.ContainsKey(item.IdONA.Value))
                            {
                                // Obtener la URL correcta del ícono desde el backend
                                var iconUrl = await getIconUrl(item);

                                // Concatenar la URL base con la ruta relativa si es necesario
                                iconUrls[item.IdONA.Value] = $"{Inicializar.UrlBaseApi.TrimEnd('/')}/{iconUrl.TrimStart('/')}";
                            }
                        }
                    }

                    if (ActivePageNumber == 1 && result.PanelONA != null)
                    {
                        TotalItems = result.TotalCount;
                        TotalPages = (int)Math.Ceiling((double)TotalItems / DisplayPages);
                        HandlePanelONAChange(result.PanelONA);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error en BuscarEsquemas: {e.Message}");
            }

            return listBuscadorResultadoDataDto;
        }

        /// <summary>
        /// Método obtener el icono correspondiente a la ONA.
        /// </summary>
        private async Task<string> getIconUrl(BuscadorResultadoDataDto resultData)
        {
            try
            {
                var idOna = resultData.IdONA;
                OnaDto = await iOnaService?.GetONAsAsync(idOna ?? 0);

                if (!string.IsNullOrEmpty(OnaDto.UrlIcono))
                {
                    var deserialized = JsonConvert.DeserializeObject<Dictionary<string, string>>(OnaDto.UrlIcono);

                    if (deserialized != null && deserialized.ContainsKey("filePath"))
                    {
                        return deserialized["filePath"];
                    }
                }

                return "https://via.placeholder.com/16";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return "https://via.placeholder.com/16";
            }
        }

        private async void registeEvent(string nombreAccion, 
                                        string nombreControl, 
                                        string codigoHomologacion, 
                                        string filtros)
        {
            objEventTracking.CodigoHomologacionMenu = "/";
            objEventTracking.NombreAccion = nombreAccion;
            objEventTracking.NombreControl = nombreControl;
            objEventTracking.idUsuario = 0;
            objEventTracking.CodigoHomologacionRol = codigoHomologacion;
            objEventTracking.ParametroJson = filtros; ;
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);
        }
    }

    /// <summary>
    /// Clase para manejar los filtros de búsqueda seleccionados.
    /// </summary>
    public class FiltrosBusquedaSeleccion
    {
        /// <summary>
        /// Código de homologación del filtro.
        /// </summary>
        public string CodigoHomologacion { get; set; } = string.Empty;

        /// <summary>
        /// Lista de valores seleccionados.
        /// </summary>
        public List<string> Seleccion { get; set; } = new List<string>();

        /// <summary>
        /// Inicializador de la clase.
        /// </summary>
        public FiltrosBusquedaSeleccion()
        {
            Seleccion = new List<string>();
        }
    }
}