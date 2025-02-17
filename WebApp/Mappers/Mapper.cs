using WebApp.Models;
using AutoMapper;
using SharedApp.Models.Dtos;

namespace WebApp.Mappers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<VwGrilla, VwGrillaDto>();
            CreateMap<VwFiltro, VwFiltroDto>();
            CreateMap<vwFiltroDetalle, vwFiltroDetalleDto>();
            CreateMap<VwDimension, VwDimensionDto>();
            CreateMap<Homologacion, GruposDto>();
            CreateMap<VwRol, VwRolDto>();
            CreateMap<VwPais, VwPaisDto>();
            CreateMap<VwMenu, VwMenuDto>();
            CreateMap<vwPanelONA, vwPanelONADto>();
            CreateMap<VwHomologacionGrupo, VwHomologacionGrupoDto>();
            CreateMap<VwHomologacion, VwHomologacionDto>();
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<UsuarioDto, Usuario>();
            CreateMap<vwONA, vwONADto>();
            CreateMap<vwEsquemaOrganiza, vwEsquemaOrganizaDto>();

            //Usuario
            CreateMap<VwAcreditacionOna, VwAcreditacionOnaDto>();
            CreateMap<VwAcreditacionEsquema, VwAcreditacionEsquemaDto>();
            CreateMap<VwEstadoEsquema, VwEstadoEsquemaDto>();
            CreateMap<VwOecPais, VwOecPaisDto>();
            CreateMap<VwEsquemaPais, VwEsquemaPaisDto>();
            CreateMap<VwOecFecha, VwOecFechaDto>();

            //Read
            CreateMap<VwProfesionalCalificado, VwProfesionalCalificadoDto>();
            CreateMap<VwProfesionalOna, VwProfesionalOnaDto>();
            CreateMap<VwProfesionalEsquema, VwProfesionalEsquemaDto>();
            CreateMap<VwProfesionalFecha, VwProfesionalFechaDto>();
            CreateMap<VwCalificaUbicacion, VwCalificaUbicacionDto>();

            //Can
            CreateMap<VwBusquedaFecha, VwBusquedaFechaDto>();
            CreateMap<VwBusquedaFiltro, VwBusquedaFiltroDto>();
            CreateMap<VwBusquedaUbicacion, VwBusquedaUbicacionDto>();
            CreateMap<VwActualizacionONA, VwActualizacionONADto>();

            //Ona
            CreateMap<VwOrganismoRegistrado, VwOrganismoRegistradoDto>();
            CreateMap<VwOrganizacionEsquema, VwOrganizacionEsquemaDto>();
            CreateMap<VwOrganismoActividad, VwOrganismoActividadDto>();

            // CreateMap<UsuarioEndpointPermiso, UsuarioEndpointPermisoDto>();

            CreateMap<Esquema, EsquemaDto>();
            CreateMap<EsquemaDto, Esquema>();

            CreateMap<Homologacion, HomologacionDto>();
            CreateMap<HomologacionDto, Homologacion>();

            CreateMap<ONAConexion, ONAConexionDto>();
            CreateMap<ONAConexionDto, ONAConexion>();

            CreateMap<ONA, OnaDto>();
            CreateMap<OnaDto, ONA>();

            //CreateMap<MigracionExcel, MigracionExcelDto>();
            //CreateMap<MigracionExcelDto, MigracionExcel>();

            CreateMap<LogMigracion, LogMigracionDto>();
            CreateMap<LogMigracionDto, LogMigracion>();

            CreateMap<EsquemaVista, EsquemaVistaValidacionDto>();
            CreateMap<EsquemaVistaValidacionDto, EsquemaVista>();

            CreateMap<EsquemaVistaColumna, EsquemaVistaColumnaDto>();
            CreateMap<EsquemaVistaColumnaDto, EsquemaVistaColumna>();

            //Thesaurus
            CreateMap<Thesaurus, ThesaurusDto>();
            CreateMap<Expansion, ExpansionDto>();
            CreateMap<ExpansionDto, Expansion>();
            CreateMap<Replacement, ReplacementDto>();
        }
    }
}
