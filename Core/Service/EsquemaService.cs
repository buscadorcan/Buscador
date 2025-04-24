using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Service
{
    public class EsquemaService : IEsquemaService
    {
        private readonly IEsquemaRepository _esquemaRepository;

        public EsquemaService(IEsquemaRepository esquemaRepository)
        {
            this._esquemaRepository = esquemaRepository;
        }
        public bool Create(Esquema data)
        {
           return _esquemaRepository.Create(data);
        }

        public bool CreateEsquemaValidacion(EsquemaVista data)
        {
            return _esquemaRepository.CreateEsquemaValidacion(data);
        }

        public bool EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(int id)
        {
            return _esquemaRepository.EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(id);
        }

        public List<Esquema> FindAll()
        {
           return _esquemaRepository.FindAll();
        }

        public List<Esquema> FindAllWithViews()
        {
           return _esquemaRepository.FindAllWithViews();
        }

        public Esquema? FindById(int Id)
        {
           return _esquemaRepository.FindById(Id);
        }

        public Esquema? FindByViewName(string esquemaVista)
        {
           return _esquemaRepository.FindByViewName(esquemaVista);
        }

        public Task<Esquema?> FindByViewNameAsync(string esquemaVista)
        {
            return _esquemaRepository.FindByViewNameAsync(esquemaVista);
        }

        public EsquemaVistaColumna? GetEsquemaVistaColumnaByIdEquemaVistaAsync(int idOna, int idEsquema)
        {
            return _esquemaRepository.GetEsquemaVistaColumnaByIdEquemaVistaAsync(idOna, idEsquema);
        }

        public List<Esquema> GetListaEsquemaByOna(int idONA)
        {
           return _esquemaRepository.GetListaEsquemaByOna(idONA);
        }

        public bool GuardarListaEsquemaVistaColumna(List<EsquemaVistaColumna> listaEsquemaVistaColumna, int? idOna, int? intidEsquema)
        {
            return _esquemaRepository.GuardarListaEsquemaVistaColumna(listaEsquemaVistaColumna, idOna, intidEsquema);
        }

        public bool Update(Esquema data)
        {
           return _esquemaRepository.Update(data);
        }

        public bool UpdateEsquemaValidacion(EsquemaVista data)
        {
            return _esquemaRepository.UpdateEsquemaValidacion(data);
        }
    }
}
