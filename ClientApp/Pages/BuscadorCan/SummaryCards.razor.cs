using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para las tarjetas de resumen.
    /// </summary>
    public partial class SummaryCards : ComponentBase
    {
        /// <summary>
        /// Servicio de catálogos.
        /// </summary>
        [Inject] public ICatalogosService? iCatalogosService { get; set; }

        /// <summary>
        /// PANEL DE ONAS.
        /// </summary>
        [Parameter] public List<vwPanelONADto>? PanelONA { get; set; }

        private int TotalOrganismos => PanelONA?.Sum(x => x.NroOrg) ?? 0;

        /// <summary>
        /// Evento para el cambio de panel de ONA.
        /// </summary>
        [Parameter] public EventCallback<List<vwPanelONADto>> HandlePanelONAChange { get; set; }

        /// <summary>
        /// Inicializacion de valores
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            if (iCatalogosService != null)
            {
                await HandlePanelONAChange.InvokeAsync(await iCatalogosService.GetPanelOnaAsync());
            }
        }
    }
}
