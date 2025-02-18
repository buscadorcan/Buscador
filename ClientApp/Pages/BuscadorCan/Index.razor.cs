using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Data;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class Index
    {
        [Inject]
        public ICatalogosService? iCatalogosService { get; set; }
        [Inject]
        public IBusquedaService? iBusquedaService { get; set; }

        private IndexGrilla? childComponentRef;
        private IndexCard? cardComponentRef;
        private List<VwFiltroDto>? listaEtiquetasFiltros = new List<VwFiltroDto>();
        private List<vwPanelONADto>? listaDatosPanel = new List<vwPanelONADto>();
        private List<List<vwFiltroDetalleDto>?> listadeOpciones = new List<List<vwFiltroDetalleDto>?>();
        private List<FiltrosBusquedaSeleccion> selectedValues = new List<FiltrosBusquedaSeleccion>();
        private int TotalEmpresa = 0;
        private BuscarRequest buscarRequest = new BuscarRequest();
        private bool modoBuscar = false;
        private List<Item> ListTypeSearch = new TypeSearch().ListTypeSearch;
        private List<FnPredictWordsDto> ListFnPredictWordsDto = new List<FnPredictWordsDto>();
        private FnPredictWordsDto? selectedWord;
        private List<Seleccion> Selecciones = new();
        private List<int> SelectedIds = new List<int>();
        private bool mostrarFiltrosAvanzados = false;
        private bool isExactSearch = false;
        private bool mostrarIndexCard = false; // Para alternar entre tarjeta de índice y grilla
        private string searchTerm = string.Empty;
        private string textoFiltrosAvanzados = "Filtros Avanzados";
        //private bool mostrarBuscador = false;
        private bool mostrarPublicidad = true;
        private bool esModoGrilla = true;
        private bool mostrarBuscador = false;
        private bool isCleaning = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (iCatalogosService != null)
                {
                    listaEtiquetasFiltros = await iCatalogosService.GetFiltrosAsync();

                    if (listaEtiquetasFiltros != null)
                    {
                        foreach (var opciones in listaEtiquetasFiltros)
                        {
                            listadeOpciones.Add(await iCatalogosService.GetFiltroDetalleAsync<List<vwFiltroDetalleDto>>("filters/data", opciones.CodigoHomologacion));
                            Console.WriteLine($"Lectura del codigo");
                        }

                    }
                    listaDatosPanel = new List<vwPanelONADto>();
                    TotalEmpresa = 0;
                    listaDatosPanel = await iCatalogosService.GetPanelOnaAsync();
                    TotalEmpresa = listaDatosPanel.Sum(x => x.NroOrg);
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            StateHasChanged();
        }

        private void CambiarSeleccion(string valor, int comboIndex, object isChecked)
        {
            bool seleccionado = bool.Parse(isChecked.ToString());
            // Obtén el CódigoHomologacion de listaEtiquetasFiltros
            var codigoHomologacion = listaEtiquetasFiltros?[comboIndex]?.CodigoHomologacion;

            if (string.IsNullOrWhiteSpace(codigoHomologacion))
            {
                Console.WriteLine($"No se encontró CódigoHomologacion para comboIndex {comboIndex}");
                return;
            }

            // Busca el filtro correspondiente en selectedValues
            var filtro = selectedValues.FirstOrDefault(f => f.CodigoHomologacion == codigoHomologacion);

            if (filtro == null)
            {
                // Si no existe el filtro, lo creamos
                filtro = new FiltrosBusquedaSeleccion
                {
                    CodigoHomologacion = codigoHomologacion,
                    Seleccion = new List<string>()
                };
                selectedValues.Add(filtro);
            }

            if (seleccionado)
            {
                // Agregar valor seleccionado
                if (!filtro.Seleccion.Contains(valor))
                {
                    filtro.Seleccion.Add(valor);
                }
            }
            else
            {
                // Quitar valor deseleccionado
                filtro.Seleccion.Remove(valor);

                // Si ya no hay selecciones, eliminamos el filtro
                if (!filtro.Seleccion.Any())
                {
                    selectedValues.Remove(filtro);
                }
            }

            Console.WriteLine($"Seleccionado: {string.Join(", ", filtro.Seleccion)} para {codigoHomologacion}");
        }
        private async Task BuscarPalabraRequest()
        {
            try
            {
                buscarRequest.TextoBuscar = searchTerm; // Asignar el término de búsqueda
                mostrarBuscador = true; // Habilitar la visualización de resultados

                if (esModoGrilla && childComponentRef != null) // Modo grilla
                {
                    childComponentRef.ModoBuscar = isExactSearch;
                    childComponentRef.selectedValues = selectedValues;
                    await childComponentRef.grid.ResetPageNumber();
                }
                else if (!esModoGrilla && cardComponentRef != null) // Modo tarjeta
                {
                    cardComponentRef.SearchTerm = searchTerm;
                    cardComponentRef.IsExactSearch = isExactSearch;
                    cardComponentRef.SelectedValues = selectedValues;
                    await cardComponentRef.BuscarPalabraRequest();
                }

                mostrarPublicidad = false; // Ocultar publicidad después de la búsqueda
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en BuscarPalabraRequest: {ex.Message}");
            }

            StateHasChanged(); // Forzar actualización de la UI
        }


        private async Task<AutoCompleteDataProviderResult<FnPredictWordsDto>> FnPredictWordsDtoDataProvider(AutoCompleteDataProviderRequest<FnPredictWordsDto> request)
        {
            if (request.Filter == null || string.IsNullOrWhiteSpace(request.Filter.Value))
            {
                return new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = [], TotalCount = 0 };
            }

            buscarRequest.TextoBuscar = request.Filter.Value;

            if (iBusquedaService != null)
            {
                var words = await iBusquedaService.FnPredictWords(request.Filter.Value);
                return new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = words, TotalCount = words.Count() };
            }

            return new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = [], TotalCount = 0 };
        }

        private void AlternarFiltrosAvanzados()
        {
            mostrarFiltrosAvanzados = !mostrarFiltrosAvanzados;
            mostrarPublicidad = false;
            // Cambiar el texto según el estado
            textoFiltrosAvanzados = mostrarFiltrosAvanzados
                ? "Ocultar Filtros Avanzados"
                : "Filtros Avanzados";
        }

        private async Task AlternarIndexCard()
        {
            mostrarIndexCard = true;
            esModoGrilla = false;

            // Esperar a que la UI se actualice y el componente se renderice
            await InvokeAsync(StateHasChanged);
            await Task.Delay(50); // Pequeño retraso para dar tiempo a Blazor a asignar la referencia

            if (cardComponentRef != null) // Ahora `cardComponentRef` debería estar disponible
            {
                    cardComponentRef.SearchTerm = searchTerm;
                    cardComponentRef.IsExactSearch = isExactSearch;
                    cardComponentRef.SelectedValues = selectedValues;
                    await cardComponentRef.BuscarPalabraRequest();
            }
            else
            {
                Console.WriteLine("cardComponentRef aún no está inicializado.");
            }
        }


        private async Task AlternarIndexGrilla()
        {
            mostrarIndexCard = false;
            esModoGrilla = true;

            if (childComponentRef != null) // Asegurar que el componente existe
            {
                // Si ya hay datos cargados, no se vuelve a buscar
                if (childComponentRef.grid != null)
                {
                    childComponentRef.ModoBuscar = isExactSearch;
                    childComponentRef.selectedValues = selectedValues;
                    await childComponentRef.grid.ResetPageNumber();
                }
            }
            StateHasChanged();
        }
        private class Seleccion
        {
            public int ComboIndex { get; set; }
            public string Texto { get; set; }
        }
        public class Opcion
        {
            public string MostrarWeb { get; set; }
        }

        private async Task OnSearchChanged(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Crear un FilterItem con los parámetros requeridos
                var filterItem = new FilterItem(
                    propertyName: "Word",                  // Nombre de la propiedad a filtrar
                    value: searchTerm,                     // Valor de búsqueda
                    @operator: FilterOperator.Contains,    // Operador de comparación
                    stringComparison: StringComparison.OrdinalIgnoreCase // Tipo de comparación
                );

                // Crear la solicitud de autocompletado con el filtro
                var request = new AutoCompleteDataProviderRequest<FnPredictWordsDto>
                {
                    Filter = filterItem
                };

                var result = await FnPredictWordsDtoDataProvider(request);
                ListFnPredictWordsDto = result.Data.ToList();
            }
        }

        private void ActualizarPanelONA(List<vwPanelONADto> panelOnaData)
        {
            listaDatosPanel = new List<vwPanelONADto>();
            TotalEmpresa = 0;
            listaDatosPanel = panelOnaData;
            TotalEmpresa = listaDatosPanel.Sum(x => x.NroOrg);
            StateHasChanged();
        }
        async Task LimpiarFiltros()
        {
            try
            {
                if (isCleaning) return; // Evita múltiples clics simultáneos
                isCleaning = true;
                StateHasChanged(); // Forzar actualización de la UI

                // 1️⃣ Recorrer cada filtro seleccionado y deseleccionarlo
                foreach (var filtro in selectedValues.ToList())
                {
                    foreach (var valor in filtro.Seleccion.ToList())
                    {
                        int comboIndex = listaEtiquetasFiltros.FindIndex(f => f.CodigoHomologacion == filtro.CodigoHomologacion);
                        if (comboIndex >= 0)
                        {
                            CambiarSeleccion(valor, comboIndex, false); // Desmarcar
                        }
                    }
                }

                // 2️⃣ Limpiar listas de filtros y opciones
                selectedValues.Clear();
                listadeOpciones.Clear();
                listaEtiquetasFiltros.Clear();

                // 3️⃣ Volver a cargar los filtros desde el backend
                if (iCatalogosService != null)
                {
                    listaEtiquetasFiltros = await iCatalogosService.GetFiltrosAsync();
                    if (listaEtiquetasFiltros != null)
                    {
                        foreach (var opciones in listaEtiquetasFiltros)
                        {
                            listadeOpciones.Add(await iCatalogosService.GetFiltroDetalleAsync<List<vwFiltroDetalleDto>>("filters/data", opciones.CodigoHomologacion));
                        }
                    }
                }

                // 4️⃣ Forzar la actualización de la UI
                StateHasChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al limpiar los filtros: {e.Message}");
            }
            finally
            {
                isCleaning = false; // Habilitar el botón nuevamente al finalizar
                StateHasChanged();
            }
        }


    }

    public class FiltrosBusquedaSeleccion
    {
        public string CodigoHomologacion { get; set; } = string.Empty; // Identificador único
        public List<string> Seleccion { get; set; } = new List<string>(); // Valores seleccionados

        public FiltrosBusquedaSeleccion()
        {
            Seleccion = new List<string>();
        }

    }



}