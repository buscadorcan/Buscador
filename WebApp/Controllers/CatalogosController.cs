/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/CatalogosController: Controlador para catalogos
using Microsoft.AspNetCore.Mvc;
using SharedApp.Response;
using AutoMapper;
using DataAccess.Interfaces;
using SharedApp.Dtos;
using DataAccess.Models;
using Core.Interfaces;

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
      ICatalogosService iCatalogosService,
      IMapper mapper
    ) : BaseController
    {
        private readonly ICatalogosService _iCatalogosService = iCatalogosService;
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
                if (_iCatalogosService == null)
                {
                    return StatusCode(500, new { mensaje = "El repositorio está NULL" });
                }
                return Ok(new RespuestasAPI<List<VwGrillaDto>>
                {
                    Result = _iCatalogosService.ObtenerVwGrilla().Select(item => _mapper.Map<VwGrillaDto>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenerVwFiltro().Select(item => _mapper.Map<VwFiltroDto>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenerFiltroDetalles(codigo).Select(item => _mapper.Map<vwFiltroDetalleDto>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenerVwDimension().Select(item => _mapper.Map<VwDimensionDto>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenerVwHomologacionGrupo().Select(item => _mapper.Map<VwHomologacionGrupoDto>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenerVwRol().Select(item => _mapper.Map<VwRolDto>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenerOna().Select(item => _mapper.Map<OnaDto>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenerVwMenu().Select(item => _mapper.Map<VwMenuDto>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenerVwPanelOna().Select(item => _mapper.Map<vwPanelONA>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenervwEsquemaOrganiza().Select(item => _mapper.Map<vwEsquemaOrganiza>(item)).ToList()
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
                    Result = _iCatalogosService.ObtenervwOna().Select(item => _mapper.Map<vwONADto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerVwOna));
            }
        }

        /// <summary>
        /// filters/anidados
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de vwONADto que representa los ONAs registrados.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpPost("filters/anidados")]
        public IActionResult ObtenerFiltrosAnidados([FromBody] List<FiltrosBusquedaSeleccionDto> filtrosSeleccionados)
        {
            try
            {
                var resultado = _iCatalogosService.ObtenerFiltrosAnidados(filtrosSeleccionados);
                var dto = new Dictionary<string, List<vw_FiltrosAnidadosDto>>();

                // Agrupamos las opciones por tipo de filtro (KEY_FIL_ONA, KEY_FIL_PAI, etc.)
                foreach (var key in new[] { "KEY_FIL_ONA", "KEY_FIL_PAI", "KEY_FIL_EST", "KEY_FIL_ESO", "KEY_FIL_NOR", "KEY_FIL_REC" })
                {
                    var valores = resultado
                        .Select(r => ObtenerValorPorClave(r, key))
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Distinct()
                        .ToList();

                    dto[key] = valores.Select(val => new vw_FiltrosAnidadosDto
                    {
                        KEY_FIL_ONA = key == "KEY_FIL_ONA" ? val : null,
                        KEY_FIL_PAI = key == "KEY_FIL_PAI" ? val : null,
                        KEY_FIL_EST = key == "KEY_FIL_EST" ? val : null,
                        KEY_FIL_ESO = key == "KEY_FIL_ESO" ? val : null,
                        KEY_FIL_NOR = key == "KEY_FIL_NOR" ? val : null,
                        KEY_FIL_REC = key == "KEY_FIL_REC" ? val : null,
                    }).ToList();
                }

                return Ok(dto);
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerFiltrosAnidados));
            }
        }

        /// <summary>
        /// filters/anidados
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de vwONADto que representa los ONAs registrados.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet("anidados")]
        public IActionResult ObtenerFiltrosAnidadosAll()
        {
            try
            {
                var resultado = _iCatalogosService.ObtenerFiltrosAnidadosAll();
                return Ok(resultado);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error al obtener filtros anidados: {e.Message}");
            }
        }




        private string ObtenerValorPorClave(vw_FiltrosAnidadosDto item, string clave)
        {
            return clave switch
            {
                "KEY_FIL_ONA" => item.KEY_FIL_ONA,
                "KEY_FIL_PAI" => item.KEY_FIL_PAI,
                "KEY_FIL_EST" => item.KEY_FIL_EST,
                "KEY_FIL_ESO" => item.KEY_FIL_ESO,
                "KEY_FIL_NOR" => item.KEY_FIL_NOR,
                "KEY_FIL_REC" => item.KEY_FIL_REC,
                _ => string.Empty
            };
        }

    }
}
