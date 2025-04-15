using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;

namespace ClientApp.Pages.Administracion.LogMigracion
{
    /// <summary>
    /// Componente de listado de logs de migración.
    /// Permite visualizar el historial de migraciones realizadas en el sistema.
    /// </summary>
    public partial class Listado
    {
        // Componente de la grilla para mostrar los registros de migración
        private Grid<LogMigracionDto>? grid;
        /// <summary>
        /// Servicio de logs de migración, utilizado para obtener registros históricos.
        /// </summary>
        [Inject]
        private ILogMigracionService? iLogMigracionService { get; set; }
        // Lista que almacena los registros de logs de migración
        private List<LogMigracionDto>? listasHevd = null;

        /// <summary>
        /// Método que proporciona los datos para la grilla de logs de migración.
        /// Carga la lista de logs si aún no han sido obtenidos.
        /// </summary>
        /// <param name="request">Parámetro que define la solicitud de datos de la grilla.</param>
        /// <returns>Devuelve un resultado con los datos de logs de migración aplicando filtros y paginación.</returns>
        private async Task<GridDataProviderResult<LogMigracionDto>> LogMigracionDtoDataProvider(GridDataProviderRequest<LogMigracionDto> request)
        {
            if (listasHevd == null && iLogMigracionService != null)
            {
                listasHevd = await iLogMigracionService.GetLogMigracionesAsync();
            }
            return await Task.FromResult(request.ApplyTo(listasHevd ?? []));
        }
    }
}