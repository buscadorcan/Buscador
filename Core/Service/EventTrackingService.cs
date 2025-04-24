using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;
using SharedApp.Dtos;

namespace Core.Service
{
    public class EventTrackingService : IEventTrackingService
    {
        private readonly IEventTrackingRepository _eventTrackingRepository;

        public EventTrackingService(IEventTrackingRepository eventTrackingRepository)
        {
            this._eventTrackingRepository = eventTrackingRepository;
        }

        public bool Create(paAddEventTrackingDto data)
        {
            return _eventTrackingRepository.Create(data);
        }

        public bool DeleteEventAll(DateOnly fini, DateOnly ffin)
        {
            return _eventTrackingRepository.DeleteEventAll(fini, ffin);
        }

        public bool DeleteEventById(int id)
        {
            return _eventTrackingRepository.DeleteEventById(id);
        }

        public Menus? FindDataById(int idHRol, int idHMenu)
        {
            return _eventTrackingRepository.FindDataById(idHRol, idHMenu);
        }

        public string GetCodeByUser(string nombreUsuario, string CodigoHomologacionRol, string CodigoHomologacionMenu)
        {
            return _eventTrackingRepository.GetCodeByUser(nombreUsuario, CodigoHomologacionRol, CodigoHomologacionMenu);
        }

        public List<EventUser> GetEventAll(string report, DateOnly fini, DateOnly ffin)
        {
            return _eventTrackingRepository.GetEventAll(report, fini, ffin);
        }

        public List<FiltrosMasUsadoDto> GetEventFiltroMasUsa()
        {
            return _eventTrackingRepository.GetEventFiltroMasUsa();
        }

        public List<PaginasMasVisitadaDto> GetEventPagMasVisit()
        {
            return _eventTrackingRepository.GetEventPagMasVisit();
        }

        public List<VwEventTrackingSessionDto> GetEventSession()
        {
            return _eventTrackingRepository.GetEventSession();
        }

        public List<VwEventUserAll> GetEventUserAll()
        {
            return _eventTrackingRepository.GetEventUserAll();
        }
    }
}
