/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/CatalogosController: Controlador para catalogos
using Microsoft.AspNetCore.Mvc;
using SharedApp.Response;
using AutoMapper;
using DataAccess.Interfaces;
using SharedApp.Dtos;
using DataAccess.Models;

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
        /// ObtenerVwGrilla
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de VwGrillaDto que representa la estructura de la grilla.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet("grid/schema")]
        public IActionResult ObtenerVwGrilla()
        {
            try
            {
                if (_vhRepo == null)
                {
                    return StatusCode(500, new { mensaje = "El repositorio está NULL" });
                }
                return Ok(new RespuestasAPI<List<VwGrillaDto>>
                {
                    Result = _vhRepo.ObtenerVwGrilla().Select(item => _mapper.Map<VwGrillaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = e.Message });
            }
        }

        /// <summary>
        /// ObtenerVwFiltro
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de VwFiltroDto que representa la estructura de los filtros.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
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
        /// ObtenerFiltroDetalles
        /// </summary>
        /// <param name="codigo">Código del filtro para obtener sus detalles.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de vwFiltroDetalleDto que representa los detalles del filtro.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet("filters/data/{codigo}")]
        public IActionResult ObtenerFiltroDetalles(string codigo)
        {
            try
            {
                return Ok(new RespuestasAPI<List<vwFiltroDetalleDto>>
                {
                    Result = _vhRepo.ObtenerFiltroDetalles(codigo).Select(item => _mapper.Map<vwFiltroDetalleDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerFiltroDetalles));
            }
        }

        /// <summary>
        /// ObtenerVwDimension
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de VwDimensionDto que representa la estructura de las dimensiones.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
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
        /// ObtenerGrupos
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de VwHomologacionGrupoDto que representa los grupos en la aplicación.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet("grupos")]
        public IActionResult ObtenerGrupos()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwHomologacionGrupoDto>>
                {
                    Result = _vhRepo.ObtenerVwHomologacionGrupo().Select(item => _mapper.Map<VwHomologacionGrupoDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerGrupos));
            }
        }


        /// <summary>
        /// ObtenerVwRol
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de VwRolDto que representa los roles.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
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
        /// ObtenerOna
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de OnaDto que representa los ONAs.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
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
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/ObtenerVwMenu: Obtiene los datos para el menú. Requiere autorización.
        /// Este método devuelve la estructura de datos utilizada para representar los elementos del menú de la aplicación.
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de VwMenuDto que representa los elementos del menú.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
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
                return HandleException(e, nameof(ObtenerVwMenu));
            }
        }

        /// <summary>
        /// ObtenerPanelOna
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de vwPanelONA que representa la configuración del panel ONA.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet("panel")]
        public IActionResult ObtenerPanelOna()
        {
            try
            {
                return Ok(new RespuestasAPI<List<vwPanelONA>>
                {
                    Result = _vhRepo.ObtenerVwPanelOna().Select(item => _mapper.Map<vwPanelONA>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerPanelOna));
            }
        }

        /// <summary>
        /// ObtenerEsquemaOrganiza
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de vwEsquemaOrganiza que representa la estructura organizacional.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet("EsquemaOrganiza")]
        public IActionResult ObtenerEsquemaOrganiza()
        {
            try
            {
                return Ok(new RespuestasAPI<List<vwEsquemaOrganiza>>
                {
                    Result = _vhRepo.ObtenervwEsquemaOrganiza().Select(item => _mapper.Map<vwEsquemaOrganiza>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerEsquemaOrganiza));
            }
        }

        /// <summary>
        /// ObtenerVwOna
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de vwONADto que representa los ONAs registrados.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet("vwona")]
        public IActionResult ObtenerVwOna()
        {
            try
            {
                return Ok(new RespuestasAPI<List<vwONADto>>
                {
                    Result = _vhRepo.ObtenervwOna().Select(item => _mapper.Map<vwONADto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerVwOna));
            }
        }


    }
}
