using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Clase InputFilters
    /// </summary>
    public partial class InputFilters : ComponentBase
    {
        /// <summary>
        /// Variable para consultar datos de catalogos
        /// </summary>
        [Inject] public ICatalogosService? iCatalogosService { get; set; }

        /// <summary>
        /// Evento que se dispara para mantener al componente padre informado de los cambios en los filtros.
        /// </summary>
        [Parameter] public EventCallback<List<FiltrosBusquedaSeleccion>> onFilterChange { get; set; }

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
        /// Lista de valores seleccionados
        /// </summary>
        private List<FiltrosBusquedaSeleccion> selectedValues = new List<FiltrosBusquedaSeleccion>();

        /// <summary>
        /// Inicializador de datos
        /// </summary>
        /// <returns></returns>
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
        /// Método para agregar / quitar seleccion el filtro
        /// </summary>
        private void CambiarSeleccion(string valor, int comboIndex, object isChecked)
        {
            bool seleccionado = bool.Parse(isChecked.ToString());
            // Obtén el CódigoHomologacion de listaEtiquetasFiltros
            var codigoHomologacion = listaEtiquetasFiltros?[comboIndex]?.CodigoHomologacion;

            if (string.IsNullOrWhiteSpace(codigoHomologacion))
            {
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

            _ = onFilterChange.InvokeAsync(selectedValues);
        }

        /// <summary>
        /// Método para limpiar los filtros
        /// </summary>
        /// <returns></returns>
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
                _ = onFilterChange.InvokeAsync(selectedValues);

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
}