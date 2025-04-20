using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Clase InputFilters
    /// </summary>
    public partial class InputFilters : ComponentBase, IDisposable
    {
        /// <summary>
        /// Variable para consultar datos de catalogos
        /// </summary>
        [Inject] public ICatalogosService? iCatalogosService { get; set; }

        /// <summary>
        /// Evento que se dispara para mantener al componente padre informado de los cambios en los filtros.
        /// </summary>
        [Parameter] public EventCallback<List<FiltrosBusquedaSeleccionDto>> onFilterChange { get; set; }

        /// <summary>
        /// Evento que se dispara para mantener al componente padre informado de la visibilidad de la grilla.
        /// </summary>
        [Parameter] public EventCallback<bool> isGridVisibleChanged { get; set; }

        /// <summary>
        /// Propiedad para mostrar la grilla
        /// </summary>
        [Parameter] public bool isGridVisible { get; set; }

        /// <summary>
        /// Propiedad para mostrar el filtro
        /// </summary>
        private bool isFilterVisible = false;

        /// <summary>
        /// Método para habilitar / deshabilitar el botón de limpiar
        /// </summary>
        private bool isCleaning = false;

        /// <summary>
        /// Lista de opciones de filtros
        /// </summary>
        private List<List<vwFiltroDetalleDto>?> listadeOpciones = new List<List<vwFiltroDetalleDto>?>();

        /// <summary>
        /// Lista de etiquetas de filtros
        /// </summary>
        private List<VwFiltroDto>? listaEtiquetasFiltros = new List<VwFiltroDto>();

        /// <summary>
        /// Método para limpiar los checkboxes sin afectar la lista de opciones.
        /// </summary>
        [Inject] public IJSRuntime JS { get; set; }

        /// <summary>
        /// Lista de valores seleccionados
        /// </summary>
        private List<FiltrosBusquedaSeleccionDto> selectedValues = new();

        /// <summary>
        /// Referencia al objeto para llamadas desde JS
        /// </summary>
        private DotNetObjectReference<InputFilters>? _objRef;

        /// <summary>
        /// Inicializador de datos
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
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

            StateHasChanged();
        }

        /// <summary>
        /// Método para agregar / quitar selección del filtro
        /// </summary>
        private void CambiarSeleccion(string valor, int comboIndex, object isChecked)
        {
            bool seleccionado = bool.Parse(isChecked.ToString());

            var codigoHomologacion = listaEtiquetasFiltros?[comboIndex]?.CodigoHomologacion;
            if (string.IsNullOrWhiteSpace(codigoHomologacion)) return;

            var filtro = selectedValues.FirstOrDefault(f => f.CodigoHomologacion == codigoHomologacion);

            if (filtro == null)
            {
                filtro = new FiltrosBusquedaSeleccionDto
                {
                    CodigoHomologacion = codigoHomologacion,
                    Seleccion = new List<string>()
                };
                selectedValues.Add(filtro);
            }

            if (seleccionado)
            {
                if (!filtro.Seleccion.Contains(valor))
                {
                    filtro.Seleccion.Add(valor);
                }
            }
            else
            {
                filtro.Seleccion.Remove(valor);

                if (!filtro.Seleccion.Any())
                {
                    selectedValues.Remove(filtro);
                }
            }

            _ = onFilterChange.InvokeAsync(selectedValues);
            StateHasChanged(); // 🔥 Forzar la actualización del estado visual del botón
        }

        /// <summary>
        /// Método para limpiar los filtros
        /// </summary>
        async Task LimpiarFiltros()
        {
            try
            {
                if (isCleaning) return; // Evita múltiples clics simultáneos
                isCleaning = true;

                // Llamar a la función JavaScript para desmarcar todos los checkboxes
                await JS.InvokeVoidAsync("desmarcarTodosLosCheckboxes");

                // Limpiar la lista de seleccionados sin modificar las opciones
                selectedValues.Clear();
                _ = onFilterChange.InvokeAsync(selectedValues);

                StateHasChanged(); // Forzar la actualización visual
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al limpiar los filtros: {e.Message}");
            }
            finally
            {
                isCleaning = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Método para seleccionar o deseleccionar todos los valores de un combo
        /// </summary>
        private void CambiarSeleccionTodos(int comboIndex, object isChecked)
        {
            bool seleccionarTodos = bool.Parse(isChecked.ToString());

            var codigoHomologacion = listaEtiquetasFiltros?[comboIndex]?.CodigoHomologacion;
            if (string.IsNullOrWhiteSpace(codigoHomologacion)) return;

            var opciones = listadeOpciones[comboIndex];
            if (opciones == null) return;

            var filtroExistente = selectedValues.FirstOrDefault(f => f.CodigoHomologacion == codigoHomologacion);
            if (filtroExistente == null)
            {
                filtroExistente = new FiltrosBusquedaSeleccionDto
                {
                    CodigoHomologacion = codigoHomologacion,
                    Seleccion = new List<string>()
                };
                selectedValues.Add(filtroExistente);
            }

            if (seleccionarTodos)
            {
                // Agrega todos si no están ya
                filtroExistente.Seleccion = opciones.Select(o => o.MostrarWeb).Distinct().ToList();
            }
            else
            {
                // Elimina el filtro completo
                selectedValues.RemoveAll(f => f.CodigoHomologacion == codigoHomologacion);
            }

            _ = onFilterChange.InvokeAsync(selectedValues);
            StateHasChanged();
        }

        /// <summary>
        /// Método invocable desde JavaScript para recibir filtros seleccionados
        /// </summary>
        [JSInvokable]
        public async Task RecibirSeleccionados(List<SeleccionadoDto> seleccionados)
        {
            try
            {
                if (seleccionados == null || !seleccionados.Any()) return;

                // Aquí se procesan los filtros seleccionados.
                selectedValues.Clear();
                foreach (var seleccionado in seleccionados)
                {
                    var filtro = selectedValues.FirstOrDefault(f => f.CodigoHomologacion == seleccionado.Combo);
                    if (filtro == null)
                    {
                        filtro = new FiltrosBusquedaSeleccionDto
                        {
                            CodigoHomologacion = seleccionado.Combo,
                            Seleccion = new List<string>()
                        };
                        selectedValues.Add(filtro);
                    }

                    filtro.Seleccion.Add(seleccionado.Valor);
                }

                // Aquí puedes llamar al servicio de backend para obtener los filtros actualizados
                if (iCatalogosService != null)
                {
                    var nuevosDatos = await iCatalogosService.GetFiltrosAnidadosAsync(selectedValues);
                    ActualizarOpcionesDesdeBackend(nuevosDatos);
                }

                await onFilterChange.InvokeAsync(selectedValues);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar los filtros: {ex.Message}");
            }
        }

        /// <summary>
        /// Método para actualizar los combos desde los datos anidados del backend
        /// </summary>
        private void ActualizarOpcionesDesdeBackend(Dictionary<string, List<vw_FiltrosAnidadosDto>> respuesta)
        {
            for (int i = 0; i < listaEtiquetasFiltros?.Count; i++)
            {
                var codigo = listaEtiquetasFiltros[i].CodigoHomologacion;
                if (respuesta.ContainsKey(codigo))
                {
                    var opcionesActualizadas = respuesta[codigo]
                        .Select(dto => new vwFiltroDetalleDto
                        {
                            MostrarWeb = codigo switch
                            {
                                "KEY_FIL_ONA" => dto.KEY_FIL_ONA,
                                "KEY_FIL_PAI" => dto.KEY_FIL_PAI,
                                "KEY_FIL_EST" => dto.KEY_FIL_EST,
                                "KEY_FIL_ESO" => dto.KEY_FIL_ESO,
                                "KEY_FIL_NOR" => dto.KEY_FIL_NOR,
                                "KEY_FIL_REC" => dto.KEY_FIL_REC,
                                _ => null
                            }
                        })
                        .Where(o => !string.IsNullOrWhiteSpace(o.MostrarWeb))
                        .ToList();

                    listadeOpciones[i] = opcionesActualizadas;
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _objRef = DotNetObjectReference.Create(this);
                await JS.InvokeVoidAsync("registrarInstanciaInputFilters", _objRef);
            }
        }


        /// <summary>
        /// Liberar recursos
        /// </summary>
        public void Dispose()
        {
            _objRef?.Dispose();
        }
    }
}
