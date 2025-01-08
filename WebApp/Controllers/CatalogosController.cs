using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controlador para la gestión de catálogos, filtros, dimensiones y roles.
    /// </summary>
    [Route("api/catalogos")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class CatalogosController(
      ICatalogosRepository vhRepo,
      IMapper mapper
    ) : BaseController
    {
        private readonly ICatalogosRepository _vhRepo = vhRepo;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Obtiene el esquema de la grilla.
        /// </summary>
        /// <returns>Una lista con el esquema de la grilla.</returns>
        [HttpGet("grid/schema")]
        public IActionResult ObtenerVwGrilla()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwGrillaDto>>
                {
                    Result = _vhRepo.ObtenerVwGrilla().Select(item => _mapper.Map<VwGrillaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerVwGrilla));
            }
        }

        /// <summary>
        /// Obtiene el esquema de los filtros.
        /// </summary>
        /// <returns>Una lista con el esquema de los filtros.</returns>
        [HttpGet("filters/schema")]
        public IActionResult ObtenerVwFiltro()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwFiltroDto>>
                {
                    Result = _vhRepo.ObtenerVwFiltro().Select(item => _mapper.Map<VwFiltroDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerVwFiltro));
            }
        }

        /// <summary>
        /// Obtiene los detalles de un filtro específico.
        /// </summary>
        /// <param name="id">ID del filtro.</param>
        /// <returns>Una lista con los detalles del filtro.</returns>
        [HttpGet("filters/data/{id:int}", Name = "ObtenerFiltroDetalles")]
        public IActionResult ObtenerFiltroDetalles(int id)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnFiltroDetalleDto>>
                {
                    Result = _vhRepo.ObtenerFiltroDetalles(id)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerFiltroDetalles));
            }
        }

        /// <summary>
        /// Obtiene el esquema de las dimensiones.
        /// </summary>
        /// <returns>Una lista con el esquema de las dimensiones.</returns>
        [HttpGet("dimensions/schema")]
        public IActionResult ObtenerVwDimension()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwDimensionDto>>
                {
                    Result = _vhRepo.ObtenerVwDimension().Select(item => _mapper.Map<VwDimensionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerVwDimension));
            }
        }

        /// <summary>
        /// Obtiene el esquema de los grupos. Requiere autorización.
        /// </summary>
        /// <returns>Una lista con el esquema de los grupos.</returns>
        [Authorize]
        [HttpGet("grupos")]
        public IActionResult ObtenerGrupos()
        {
            try
            {
                return Ok(new RespuestasAPI<List<GruposDto>>
                {
                    Result = _vhRepo.ObtenerGrupos().Select(item => _mapper.Map<GruposDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerGrupos));
            }
        }

        /// <summary>
        /// Obtiene el esquema de roles. Requiere autorización.
        /// </summary>
        /// <returns>Una lista con el esquema de roles.</returns>
        //[Authorize]
        [HttpGet("roles")]
        public IActionResult ObtenerVwRol()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwRolDto>>
                {
                    Result = _vhRepo.ObtenerVwRol().Select(item => _mapper.Map<VwRolDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerVwRol));
            }
        }


        /// <summary>
        /// Obtiene el esquema de roles. Requiere autorización.
        /// </summary>
        /// <returns>Una lista con el esquema de roles.</returns>
        [Authorize]

        [HttpGet("onas")]
        public IActionResult ObtenerOna()
        {
            try
            {
                return Ok(new RespuestasAPI<List<OnaDto>>
                {
                    Result = _vhRepo.ObtenerOna().Select(item => _mapper.Map<OnaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerOna));
            }
        }

        /// <summary>
        /// Obtiene los datos para el menu. Requiere autorización.
        /// </summary>
        /// <returns>Una lista con el esquema de menus.</returns>
        [Authorize]
        [HttpGet("menu")]
        public IActionResult ObtenerVwMenu()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwMenuDto>>
                {
                    Result = _vhRepo.ObtenerVwMenu().Select(item => _mapper.Map<VwMenuDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerVwRol));
            }
        }
    }
}
