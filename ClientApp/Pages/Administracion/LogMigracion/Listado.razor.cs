using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;

namespace ClientApp.Pages.Administracion.LogMigracion
{
    /// <summary>
    /// Componente de listado de logs de migraci�n.
    /// Permite visualizar el historial de migraciones realizadas en el sistema.
    /// </summary>
    public partial class Listado
    {
        // Componente de la grilla para mostrar los registros de migraci�n
        private Grid<LogMigracionDto>? grid;
        /// <summary>
        /// Servicio de logs de migraci�n, utilizado para obtener registros hist�ricos.
        /// </summary>
        [Inject]
        private ILogMigracionService? iLogMigracionService { get; set; }
        // Lista que almacena los registros de logs de migraci�n
        private List<LogMigracionDto>? listasHevd = null;

        /// <summary>
        /// M�todo que proporciona los datos para la grilla de logs de migraci�n.
        /// Carga la lista de logs si a�n no han sido obtenidos.
        /// </summary>
        /// <param name="request">Par�metro que define la solicitud de datos de la grilla.</param>
        /// <returns>Devuelve un resultado con los datos de logs de migraci�n aplicando filtros y paginaci�n.</returns>
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