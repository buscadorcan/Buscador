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
        //private List<List<vwFiltroDetalleDto>?> listadeOpciones = new List<List<vwFiltroDetalleDto>?>();
        private List<List<vwFiltroDetalleDto>?> listadeOpciones = new();



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
        /// llenaado de todos los datos anidados completos
        /// </summary>
        private List<vw_FiltrosAnidadosDto> datosAnidadosCompletos = new();


        /// <summary>
        /// Inicializador de datos
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            if (iCatalogosService != null)
            {
                listaEtiquetasFiltros = await iCatalogosService.GetFiltrosAsync();
                datosAnidadosCompletos = await iCatalogosService.ObtenerFiltrosAnidadosAllAsync();

                if (listaEtiquetasFiltros != null && datosAnidadosCompletos != null)
                {
                    foreach (var opciones in listaEtiquetasFiltros)
                    {
                        string codigo = opciones.CodigoHomologacion;

                        var valores = datosAnidadosCompletos
                            .Select(d =>
                            {
                                var prop = typeof(vw_FiltrosAnidadosDto).GetProperty(codigo);
                                return prop != null ? prop.GetValue(d)?.ToString() : null;
                            })
                            .Where(valor => !string.IsNullOrEmpty(valor))
                            .Distinct(StringComparer.OrdinalIgnoreCase) // ✔️ <-- ¡esto es lo clave!
                            .OrderBy(v => v, StringComparer.OrdinalIgnoreCase) // ✔️ también aplica para orden
                            .ToList();
                            

                        var listaPorFiltro = valores
                            .Select(valor => new vwFiltroDetalleDto
                            {
                                IdHomologacion = 1,
                                MostrarWeb = valor,
                                CodigoHomologacion = codigo
                            })
                            .ToList();

                        listadeOpciones.Add(listaPorFiltro);
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
                await JS.InvokeVoidAsync("cerrarDropdowns");

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

            if (seleccionarTodos)
            {
                var seleccion = opciones.Select(o => o.MostrarWeb).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                var filtroExistente = selectedValues.FirstOrDefault(f => f.CodigoHomologacion == codigoHomologacion);
                if (filtroExistente == null)
                {
                    filtroExistente = new FiltrosBusquedaSeleccionDto
                    {
                        CodigoHomologacion = codigoHomologacion,
                        Seleccion = seleccion
                    };
                    selectedValues.Add(filtroExistente);
                }
                else
                {
                    filtroExistente.Seleccion = seleccion;
                }
            }
            else
            {
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
                if (seleccionados == null || !seleccionados.Any())
                {
                    await CargarFiltrosIniciales();
                    return;
                }

                // Filtramos datosAnidadosCompletos en base a los seleccionados
                var filtrado = datosAnidadosCompletos.Where(item =>
                {
                    bool coincide = true;

                    foreach (var filtro in selectedValues)
                    {
                        var prop = typeof(vw_FiltrosAnidadosDto).GetProperty(filtro.CodigoHomologacion);
                        if (prop == null) continue;

                        var valorProp = prop.GetValue(item)?.ToString()?.Trim();

                        if (!filtro.Seleccion.Any(sel =>
                            string.Equals(valorProp?.ToUpperInvariant(), sel.Trim().ToUpperInvariant())))
                        {
                            coincide = false;
                            break;
                        }
                    }

                    return coincide;
                }).ToList();

                // Limpiamos y reconstruimos listadeOpciones desde el nuevo conjunto filtrado
                listadeOpciones.Clear();

                foreach (var etiqueta in listaEtiquetasFiltros)
                {
                    var prop = typeof(vw_FiltrosAnidadosDto).GetProperty(etiqueta.CodigoHomologacion);
                    if (prop == null)
                    {
                        listadeOpciones.Add(new List<vwFiltroDetalleDto>());
                        continue;
                    }

                    var opciones = filtrado
                        .Select(f => prop.GetValue(f)?.ToString()?.Trim())
                        .Where(val => !string.IsNullOrWhiteSpace(val))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(val => val, StringComparer.OrdinalIgnoreCase)
                        .Select(val => new vwFiltroDetalleDto
                        {
                            CodigoHomologacion = etiqueta.CodigoHomologacion,
                            MostrarWeb = val,
                            IdHomologacion = 1
                        })
                        .ToList();

                    listadeOpciones.Add(opciones);
                }

                // Actualizamos selectedValues (por si quieres seguir mostrándolos)
                selectedValues = seleccionados
                    .GroupBy(s => s.Combo)
                    .Select(g => new FiltrosBusquedaSeleccionDto
                    {
                        CodigoHomologacion = g.Key,
                        Seleccion = g.Select(x => x.Valor).ToList()
                    })
                    .ToList();

                // Notificamos al componente padre (si aplica)
                if (onFilterChange.HasDelegate)
                {
                    await onFilterChange.InvokeAsync(selectedValues);
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error simplificado en RecibirSeleccionados: {ex.Message}");
            }
        }

    

        private async Task CargarFiltrosIniciales()
        {
          
            listadeOpciones.Clear();

            if (listaEtiquetasFiltros != null && datosAnidadosCompletos != null)
            {
                foreach (var opciones in listaEtiquetasFiltros)
                {
                    string codigo = opciones.CodigoHomologacion;

                    var valores = datosAnidadosCompletos
                        .Select(d =>
                        {
                            var prop = typeof(vw_FiltrosAnidadosDto).GetProperty(codigo);
                            return prop != null ? prop.GetValue(d)?.ToString()?.Trim() : null;
                        })
                        .Where(valor => !string.IsNullOrWhiteSpace(valor))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    var listaPorFiltro = valores
                        .Select(valor => new vwFiltroDetalleDto
                        {
                            IdHomologacion = 1,
                            MostrarWeb = valor,
                            CodigoHomologacion = codigo
                        })
                        .ToList();

                    listadeOpciones.Add(listaPorFiltro);
                }
            }

            selectedValues.Clear(); // Limpiar selección
            await onFilterChange.InvokeAsync(selectedValues); // Informar al padre
            StateHasChanged(); // Refrescar UI
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
