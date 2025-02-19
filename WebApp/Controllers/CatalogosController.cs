using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using WebApp.Models;

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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwGrilla: Obtiene el esquema de la grilla.
         */
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
                //return HandleException(e, nameof(ObtenerVwGrilla));
                return StatusCode(500, new { mensaje = "Error interno", detalle = e.Message });
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwFiltro: Obtiene el esquema de los filtros.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerFiltroDetalles: Obtiene los detalles de un filtro específico.
         */
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


        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwDimension: Obtiene el esquema de las dimensiones.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerGrupos: Obtiene el esquema de los grupos. Requiere autorización.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwRol: Obtiene el esquema de roles. Requiere autorización.
         */
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


        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerOna: Obtiene el esquema de ONAs. Requiere autorización.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwMenu: Obtiene los datos para el menú. Requiere autorización.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerPanelOna: Obtiene el esquema de roles del panel ONA. Requiere autorización.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerEsquemaOrganiza: Obtiene el esquema organizacional.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwOna: Obtiene los datos de ONAs.
         */
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
                return HandleException(e, nameof(ObtenerPanelOna));
            }
        }

    }
}
