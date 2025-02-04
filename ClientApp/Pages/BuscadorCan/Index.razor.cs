using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
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
        private bool mostrarBuscador = false;
        private bool mostrarPublicidad = true;
        private bool esModoGrilla = true;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (iCatalogosService != null) {
                    listaEtiquetasFiltros = await iCatalogosService.GetFiltrosAsync();

                    if (listaEtiquetasFiltros != null)
                    {
                        foreach(var opciones in listaEtiquetasFiltros)
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

             
            } catch (Exception e) {
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
        private void OnAutoCompleteChanged(ChangeEventArgs e)
        {
            var selectedWord = e.Value?.ToString();
            if (!string.IsNullOrWhiteSpace(selectedWord))
            {
                buscarRequest.TextoBuscar = selectedWord;
            }
        }

        private void AgregarSeleccion(string valor, int? idHomologacion, int comboIndex)
        {
            if (!string.IsNullOrWhiteSpace(valor) && idHomologacion.HasValue)
            {
                // Agregar la selección sin verificar duplicados, ya que debería permitir múltiples opciones del mismo select
                Selecciones.Add(new Seleccion
                {
                    Texto = valor,
                    ComboIndex = comboIndex
                });
            }
        }

        private void QuitarSeleccion(int comboIndex, string texto)
        {
            Selecciones.RemoveAll(s => s.ComboIndex == comboIndex && s.Texto == texto);
        }

        private void SeleccionarTodos(int comboIndex, object isChecked)
        {
            bool seleccionarTodo = bool.Parse(isChecked.ToString());

            // Verificar si listadeOpciones y el índice son válidos
            if (listadeOpciones != null && comboIndex >= 0 && comboIndex < listadeOpciones.Count)
            {
                var opciones = listadeOpciones[comboIndex];
                if (seleccionarTodo)
                {
                    // Seleccionar todas las opciones del combo
                    foreach (var opcion in opciones)
                    {
                        if (!Selecciones.Any(s => s.ComboIndex == comboIndex && s.Texto == opcion.MostrarWeb))
                        {
                            Selecciones.Add(new Seleccion { ComboIndex = comboIndex, Texto = opcion.MostrarWeb });
                        }
                    }
                }
                else
                {
                    // Quitar todas las opciones del combo
                    Selecciones.RemoveAll(s => s.ComboIndex == comboIndex);
                }
            }
        }
        private string ObtenerSeleccionesComoJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(Selecciones);
        }

        private void AlternarFiltrosAvanzados()
        {
            mostrarFiltrosAvanzados = !mostrarFiltrosAvanzados;

            // Cambiar el texto según el estado
            textoFiltrosAvanzados = mostrarFiltrosAvanzados
                ? "Ocultar Filtros Avanzados"
                : "Filtros Avanzados";
        }

        private void AlternarIndexCard()
        {
            mostrarIndexCard = !mostrarIndexCard;
            esModoGrilla = !mostrarIndexCard; // Actualiza la bandera para definir si la búsqueda va a grilla o tarjetas
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