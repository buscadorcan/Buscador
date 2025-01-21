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
        //[Inject] 
        //protected IJSRuntime? JSRuntime { get; set; }
        //protected string ApiKey = "AIzaSyC7NUCEvrqrrQDDDRLK2q0HSqswPxtBVAk";
        private IndexGrilla? childComponentRef;
        private IndexCard? cardComponentRef;
        private List<VwFiltroDto>? listaEtiquetasFiltros = new List<VwFiltroDto>();
        private List<List<FnFiltroDetalleDto>?> listadeOpciones = new List<List<FnFiltroDetalleDto>?>();
        private List<FiltrosBusquedaSeleccion> selectedValues = new List<FiltrosBusquedaSeleccion>();
        private BuscarRequest buscarRequest = new BuscarRequest();
        private bool modoBuscar = false;
        private List<Item> ListTypeSearch = new TypeSearch().ListTypeSearch;
        private List<FnPredictWordsDto> ListFnPredictWordsDto = new List<FnPredictWordsDto>();
        private List<Seleccion> Selecciones = new();
        private List<int> SelectedIds = new List<int>();
        private bool mostrarFiltrosAvanzados = false;
        private bool isExactSearch = false;
        private bool mostrarIndexCard = false; // Para alternar entre tarjeta de índice y grilla
        private string searchTerm = string.Empty;
        private string textoFiltrosAvanzados = "Filtros Avanzados";
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
                            listadeOpciones.Add(await iCatalogosService.GetFiltroDetalleAsync<List<FnFiltroDetalleDto>>("filters/data", opciones.CodigoHomologacion));
                            
                        }
                        
                    }
                }

                // Si no hay datos del servicio, usar datos simulados
                if (listaEtiquetasFiltros == null || !listaEtiquetasFiltros.Any())
                {
                    CargarDatosSimulados();
                }
            } catch (Exception e) {
                Console.WriteLine(e);
                CargarDatosSimulados();
            }
        }

        private void CargarDatosSimulados()
        {
            // Crear etiquetas de filtro simuladas
            listaEtiquetasFiltros = new List<VwFiltroDto>
            {
                new VwFiltroDto { IdHomologacion = 1, MostrarWeb = "Filtro 1" },
                new VwFiltroDto { IdHomologacion = 2, MostrarWeb = "Filtro 2" },
                new VwFiltroDto { IdHomologacion = 3, MostrarWeb = "Filtro 3" }
            };

            // Crear opciones simuladas para cada filtro
            listadeOpciones = new List<List<FnFiltroDetalleDto>?>
            {
                new List<FnFiltroDetalleDto>
                {
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 1.1" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 1.2" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 1.3" }
                },
                new List<FnFiltroDetalleDto>
                {
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 2.1" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 2.2" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 2.3" }
                },
                new List<FnFiltroDetalleDto>
                {
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 3.1" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 3.2" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 3.3" }
                }
            };
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
            buscarRequest.TextoBuscar = searchTerm;
            if (childComponentRef != null)
            {
                // Sincroniza el estado del filtro con el componente hijo (IndexGrilla)
                childComponentRef.ModoBuscar = isExactSearch ? true : false;
                childComponentRef.selectedValues = selectedValues;
                 

                // Reinicia la grilla para aplicar los nuevos filtros
                await childComponentRef.grid.ResetPageNumber();
            }
        }
        private async Task<AutoCompleteDataProviderResult<FnPredictWordsDto>> FnPredictWordsDtoDataProvider(AutoCompleteDataProviderRequest<FnPredictWordsDto> request)
        {
            buscarRequest.TextoBuscar = request.Filter.Value;
            if (iBusquedaService != null)
            {
                var words = await iBusquedaService.FnPredictWords(request.Filter.Value);
                return await Task.FromResult(new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = words, TotalCount = words.Count() });
            }

            return await Task.FromResult(new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = [], TotalCount = 0 });
        }
        private void OnAutoCompleteChanged(FnPredictWordsDto _fnPredictWordsDto)
        {
            if (_fnPredictWordsDto?.Word != null)
            {
                buscarRequest.TextoBuscar = _fnPredictWordsDto.Word;
            } else {
                selectedValues = new List<FiltrosBusquedaSeleccion>();
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

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        // Llama a la inicialización del mapa desde JavaScript
        //        await JSRuntime.InvokeVoidAsync("initMap", ApiKey);
        //    }
        //}

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