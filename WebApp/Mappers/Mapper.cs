using WebApp.Models;
using AutoMapper;
using SharedApp.Models.Dtos;

namespace WebApp.Mappers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<UsuarioDto, Usuario>();

            CreateMap<Models.Endpoint, EndpointDto>();

            CreateMap<UsuarioEndpointPermiso, UsuarioEndpointPermisoDto>();

            CreateMap<HomologacionEsquema, HomologacionEsquemaDto>();
            CreateMap<HomologacionEsquemaDto, HomologacionEsquema>();

            CreateMap<Homologacion, HomologacionDto>();
            CreateMap<HomologacionDto, Homologacion>();
            CreateMap<Conexion, ConexionDto>();
        }
    }
}