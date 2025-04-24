using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;
using SharedApp.Dtos;

namespace Core.Service
{
     public class CatalogosService : ICatalogosService
    {
        private readonly ICatalogosRepository _catalogosRepository;

        public CatalogosService(ICatalogosRepository catalogosRepository)
        {
            this._catalogosRepository = catalogosRepository;
        }
        public VwHomologacionGrupo? FindVwHGByCode(string codigoHomologacion)
        {
            return _catalogosRepository.FindVwHGByCode(codigoHomologacion);
        }

        public VwRol FindVwRolByHId(int idHomologacionRol)
        {
            return _catalogosRepository.FindVwRolByHId(idHomologacionRol);
        }

        public List<vwFiltroDetalle> ObtenerFiltroDetalles(string codigo)
        {
            return _catalogosRepository.ObtenerFiltroDetalles(codigo);
        }

        public List<vw_FiltrosAnidadosDto> ObtenerFiltrosAnidados(List<FiltrosBusquedaSeleccionDto> filtrosSeleccionados)
        {
            return _catalogosRepository.ObtenerFiltrosAnidados(filtrosSeleccionados);
        }

        public List<vw_FiltrosAnidadosDto> ObtenerFiltrosAnidadosAll()
        {
           return _catalogosRepository.ObtenerFiltrosAnidadosAll();
        }

        public List<Homologacion> ObtenerGrupos()
        {
           return _catalogosRepository.ObtenerGrupos();
        }

        public List<ONA> ObtenerOna()
        {
           return _catalogosRepository.ObtenerOna();
        }

        public List<VwDimension> ObtenerVwDimension()
        {
           return _catalogosRepository.ObtenerVwDimension();
        }

        public List<vwEsquemaOrganiza> ObtenervwEsquemaOrganiza()
        {
           return _catalogosRepository.ObtenervwEsquemaOrganiza();
        }

        public List<VwFiltro> ObtenerVwFiltro()
        {
           return _catalogosRepository.ObtenerVwFiltro();
        }

        public List<VwGrilla> ObtenerVwGrilla()
        {
           return _catalogosRepository.ObtenerVwGrilla();
        }

        public List<VwHomologacionGrupo> ObtenerVwHomologacionGrupo()
        {
           return _catalogosRepository.ObtenerVwHomologacionGrupo();
        }

        public List<VwMenu> ObtenerVwMenu()
        {
           return _catalogosRepository.ObtenerVwMenu();
        }

        public List<vwONA> ObtenervwOna()
        {
           return _catalogosRepository.ObtenervwOna();
        }

        public List<vwPanelONA> ObtenerVwPanelOna()
        {
           return _catalogosRepository.ObtenerVwPanelOna();
        }

        public List<VwRol> ObtenerVwRol()
        {
           return _catalogosRepository.ObtenerVwRol();
        }
    }
}
