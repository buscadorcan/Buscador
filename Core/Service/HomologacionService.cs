using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Service
{
    public class HomologacionService : IHomologacionService
    {
        private readonly IHomologacionRepository _homologacionRepository;

        public HomologacionService(IHomologacionRepository homologacionRepository)
        {
            this._homologacionRepository = homologacionRepository;
        }

        public bool Create(Homologacion data)
        {
            return _homologacionRepository.Create(data);
        }

        public List<Homologacion> FindByAll()
        {
            return _homologacionRepository.FindByAll();
        }

        public Homologacion? FindById(int id)
        {
            return _homologacionRepository.FindById(id);
        }

        public List<Homologacion> FindByIds(int[] ids)
        {
            return _homologacionRepository.FindByIds(ids);
        }

        public ICollection<Homologacion> FindByParent()
        {
            return _homologacionRepository.FindByParent();
        }

        public List<VwHomologacion> ObtenerVwHomologacionPorCodigo(string codigoHomologacion)
        {
            return _homologacionRepository.ObtenerVwHomologacionPorCodigo(codigoHomologacion);
        }

        public bool Update(Homologacion data)
        {
            return _homologacionRepository.Update(data);
        }
    }
}
