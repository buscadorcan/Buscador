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
      CreateMap<VwDimension, VwDimensionDto>();
      CreateMap<Homologacion, GruposDto>();
      CreateMap<VwRol, VwRolDto>();
      CreateMap<VwPais, VwPaisDto>();
      CreateMap<VwMenu, VwMenuDto>();


      CreateMap<Usuario, UsuarioDto>(); 
      CreateMap<UsuarioDto, Usuario>();

      // CreateMap<UsuarioEndpointPermiso, UsuarioEndpointPermisoDto>();

      CreateMap<Esquema, EsquemaDto>();
      CreateMap<EsquemaDto, Esquema>();

      CreateMap<Homologacion, HomologacionDto>();
      CreateMap<HomologacionDto, Homologacion>();

      CreateMap<ONAConexion, ONAConexionDto>();
      CreateMap<ONAConexionDto, ONAConexion>();

      CreateMap<ONA, OnaDto>();
      CreateMap<OnaDto, ONA>();

      CreateMap<MigracionExcel, MigracionExcelDto>();
      CreateMap<MigracionExcelDto, MigracionExcel>();

      CreateMap<LogMigracion, LogMigracionDto>();
      CreateMap<LogMigracionDto, LogMigracion>();
    }
  }
}
