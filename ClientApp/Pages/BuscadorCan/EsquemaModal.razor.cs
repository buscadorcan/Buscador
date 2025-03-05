using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para el modal de esquema.
    /// </summary>
    public partial class EsquemaModal : ComponentBase
    {
        /// <summary>
        /// paneles de esquemas.
        /// </summary>
        Tabs tabs = default!;

        /// <summary>
        /// Servicio de búsqueda.
        /// </summary>
        [Inject] private IBusquedaService? servicio { get; set; }

        /// <summary>
        /// Resultado de datos de búsqueda.
        /// </summary>
        [Parameter] public BuscadorResultadoDataDto? resultData { get; set; } = default!;

        /// <summary>
        /// Lista de esquemas.
        /// </summary>
        private List<HomologacionEsquemaDto>? listaEsquemas = new List<HomologacionEsquemaDto>();

        /// <summary>
        /// Método de inicialización de datos.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (servicio != null) {
                    listaEsquemas = await servicio.FnHomologacionEsquemaTodoAsync(resultData?.VistaFK, resultData?.IdONA ?? 0);
                    Console.WriteLine($"Error en listaEsquemas:");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}