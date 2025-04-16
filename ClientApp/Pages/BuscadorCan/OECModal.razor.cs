using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para el modal de OEC.
    /// </summary>
    public partial class OECModal : ComponentBase
    {
        /// <summary>
        /// Servicio de catálogos.
        /// </summary>
        [Inject] private ICatalogosService iCatalogoService { get; set; } = default!;

        /// <summary>
        /// Resultado de la búsqueda.
        /// </summary>
        [Parameter] public BuscadorResultadoDataDto ResultData { get; set; } = default!;

        /// <summary>
        /// OEC seleccionado.
        /// </summary>
        private vwEsquemaOrganizaDto? onaSeleccionado;

        /// <summary>
        /// Indicador de carga.
        /// </summary>
        private bool loading = true;

        /// <summary>
        /// Método inicialización de datos
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var listaOnas = await iCatalogoService.GetvwEsquemaOrganizaAsync();
                onaSeleccionado = listaOnas.FirstOrDefault(ona => ona.IdEsquemaData == ResultData.IdEsquemaData);

            
                if (!string.IsNullOrEmpty(onaSeleccionado?.ONAUrlIcono) && onaSeleccionado.ONAUrlIcono.Contains("filePath"))
                {
                    var iconoJson = System.Text.Json.JsonDocument.Parse(onaSeleccionado.ONAUrlIcono);
                    onaSeleccionado.ONAUrlIcono = iconoJson.RootElement.GetProperty("filePath").GetString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el OEC: {ex.Message}");
            }
            finally
            {
                loading = false;
            }
        }
    }
}
