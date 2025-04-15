using SharedApp.Dtos;

namespace ClientApp.Services.IService
{
    public interface IEventService
    {
        Task<List<VwEventUserAllDto>> GetListEventUserAllAsync();

        Task<List<EventUserDto>> GetEventAsync(string report, DateOnly fini, DateOnly ffin);

        Task<bool> DeleteEventAllAsync(string report, DateOnly fini, DateOnly ffin);

        Task<bool> DeleteEventByIdAsync(string report, int codigoEvento);

        Task<List<VwEventTrackingSessionDto>> GetEventSessionAsync();

        Task<List<PaginasMasVisitadaDto>> GetEventPagMasVistAsync();

        Task<List<FiltrosMasUsadoDto>> GetEventFiltroMasUsadAsync();
    }
}
