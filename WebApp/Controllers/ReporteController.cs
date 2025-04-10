﻿/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/ReporteController: Controlador para funcionalidad en reportes
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Repositories.IRepositories;

namespace WebApp.Controllers
{
    [Route("api/reportevista")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ReporteController(IReporteRepository iRepo, IMapper mapper) : BaseController
    {
        private readonly IReporteRepository _vhRepo = iRepo;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// findByVista
        /// </summary>
        /// <param name="codigoHomologacion">Código de homologación a buscar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con la homologación correspondiente al código proporcionado.
        /// En caso de que no exista, devuelve un objeto vacío.
        /// </returns>
        [Authorize]
        [HttpGet("findByVista/{codigoHomologacion}")]
        public IActionResult findByVista(string codigoHomologacion)
        {
            try
            {
                var homologacion = _vhRepo.findByVista(codigoHomologacion);

                if (homologacion == null)
                {
                    return NotFound(new RespuestasAPI<VwHomologacionDto>
                    {
                        Result = new VwHomologacionDto()
                    });
                }

                var homologacionDto = _mapper.Map<VwHomologacionDto>(homologacion);

                return Ok(new RespuestasAPI<VwHomologacionDto>
                {
                    Result = homologacionDto
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(findByVista));
            }
        }

        /// <summary>
        /// ObtenerAcreditacionOna
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de acreditaciones de ONAs disponibles.
        /// </returns>
        [Authorize]
        [HttpGet("acreditacion-ona")]
        public IActionResult ObtenerAcreditacionOna()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwAcreditacionOnaDto>>
                {
                    Result = _vhRepo.ObtenerVwAcreditacionOna().Select(item => _mapper.Map<VwAcreditacionOnaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerAcreditacionOna));
            }
        }

        /// <summary>
        /// ObtenerAcreditacionEsquema
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de esquemas acreditados.
        /// </returns>
        [Authorize]
        [HttpGet("acreditacion-esquema")]
        public IActionResult ObtenerAcreditacionEsquema()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwAcreditacionEsquemaDto>>
                {
                    Result = _vhRepo.ObtenerVwAcreditacionEsquema().Select(item => _mapper.Map<VwAcreditacionEsquemaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerAcreditacionEsquema));
            }
        }

        /// <summary>
        /// ObtenerEstadoEsquema
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de estados de los esquemas.
        /// </returns>
        [Authorize]
        [HttpGet("estado-esquema")]
        public IActionResult ObtenerEstadoEsquema()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwEstadoEsquemaDto>>
                {
                    Result = _vhRepo.ObtenerVwEstadoEsquema().Select(item => _mapper.Map<VwEstadoEsquemaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerEstadoEsquema));
            }
        }

        /// <summary>
        /// ObtenerOecPais
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de OECs asociados a cada país.
        /// </returns>
        [Authorize]
        [HttpGet("oec-pais")]
        public IActionResult ObtenerOecPais()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwOecPaisDto>>
                {
                    Result = _vhRepo.ObtenerVwOecPais().Select(item => _mapper.Map<VwOecPaisDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerOecPais));
            }
        }

        /// <summary>
        /// ObtenerEsquemaPais
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de esquemas disponibles en cada país.
        /// </returns>
        [Authorize]
        [HttpGet("esquema-pais")]
        public IActionResult ObtenerEsquemaPais()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwEsquemaPaisDto>>
                {
                    Result = _vhRepo.ObtenerVwEsquemaPais().Select(item => _mapper.Map<VwEsquemaPaisDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerEsquemaPais));
            }
        }

        /// <summary>
        /// ObtenerOecFecha
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de OECs filtrados por fecha.
        /// </returns>
        [Authorize]
        [HttpGet("oec-fecha")]
        public IActionResult ObtenerOecFecha()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwOecFechaDto>>
                {
                    Result = _vhRepo.ObtenerVwOecFecha().Select(item => _mapper.Map<VwOecFechaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerOecFecha));
            }
        }


        /// <summary>
        /// ObtenerProfesionalCalificado
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de profesionales calificados.
        /// </returns>
        [Authorize]
        [HttpGet("profesional-calificado")]
        public IActionResult ObtenerProfesionalCalificado()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwProfesionalCalificadoDto>>
                {
                    Result = _vhRepo.ObtenerVwProfesionalCalificado().Select(item => _mapper.Map<VwProfesionalCalificadoDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerProfesionalCalificado));
            }
        }

        /// <summary>
        /// ObtenerProfesionalOna
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de profesionales en ONAs.
        /// </returns>
        [Authorize]
        [HttpGet("profesional-ona")]
        public IActionResult ObtenerProfesionalOna()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwProfesionalOnaDto>>
                {
                    Result = _vhRepo.ObtenerVwProfesionalOna().Select(item => _mapper.Map<VwProfesionalOnaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerProfesionalOna));
            }
        }

        /// <summary>
        /// ObtenerProfesionalEsquema
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de profesionales organizados por esquema.
        /// </returns>
        [Authorize]
        [HttpGet("profesional-esquema")]
        public IActionResult ObtenerProfesionalEsquema()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwProfesionalEsquemaDto>>
                {
                    Result = _vhRepo.ObtenerVwProfesionalEsquema().Select(item => _mapper.Map<VwProfesionalEsquemaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerProfesionalEsquema));
            }
        }

        /// <summary>
        /// ObtenerProfesionalFecha
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de profesionales filtrados por fecha.
        /// </returns>
        [Authorize]
        [HttpGet("profesional-fecha")]
        public IActionResult ObtenerProfesionalFecha()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwProfesionalFechaDto>>
                {
                    Result = _vhRepo.ObtenerVwProfesionalFecha().Select(item => _mapper.Map<VwProfesionalFechaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerProfesionalFecha));
            }
        }

        /// <summary>
        /// ObtenerCalificaUbicacion
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de calificaciones organizadas por ubicación.
        /// </returns>
        [Authorize]
        [HttpGet("califica-ubicacion")]
        public IActionResult ObtenerCalificaUbicacion()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwCalificaUbicacionDto>>
                {
                    Result = _vhRepo.ObtenerVwCalificaUbicacion().Select(item => _mapper.Map<VwCalificaUbicacionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerCalificaUbicacion));
            }
        }

        /// <summary>
        /// ObtenerBusquedaFecha
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de búsquedas filtradas por fecha.
        /// </returns>
        [Authorize]
        [HttpGet("busqueda-fecha")]
        public IActionResult ObtenerBusquedaFecha()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwBusquedaFechaDto>>
                {
                    Result = _vhRepo.ObtenerVwBusquedaFecha().Select(item => _mapper.Map<VwBusquedaFechaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerBusquedaFecha));
            }
        }

        /// <summary>
        /// ObtenerBusquedaFiltro
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de búsquedas filtradas según los criterios aplicados.
        /// </returns>
        [Authorize]
        [HttpGet("busqueda-filtro")]
        public IActionResult ObtenerBusquedaFiltro()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwBusquedaFiltroDto>>
                {
                    Result = _vhRepo.ObtenerVwBusquedaFiltro().Select(item => _mapper.Map<VwBusquedaFiltroDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerBusquedaFiltro));
            }
        }

        /// <summary>
        /// ObtenerBusquedaUbicacion
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de búsquedas organizadas por ubicación.
        /// </returns>
        [Authorize]
        [HttpGet("busqueda-ubicacion")]
        public IActionResult ObtenerBusquedaUbicacion()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwBusquedaUbicacionDto>>
                {
                    Result = _vhRepo.ObtenerVwBusquedaUbicacion().Select(item => _mapper.Map<VwBusquedaUbicacionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerBusquedaUbicacion));
            }
        }

        /// <summary>
        /// ObtenerActualizacionONA
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de actualizaciones registradas en los ONAs.
        /// </returns>
        [Authorize]
        [HttpGet("actualizacion-ona")]
        public IActionResult ObtenerActualizacionONA()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwActualizacionONADto>>
                {
                    Result = _vhRepo.ObtenerVwActualizacionONA().Select(item => _mapper.Map<VwActualizacionONADto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerActualizacionONA));
            }
        }

        /// <summary>
        /// ObtenerOrganismoRegistrado
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de organismos registrados.
        /// </returns>
        [Authorize]
        [HttpGet("organismo-registrado")]
        public IActionResult ObtenerOrganismoRegistrado()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwOrganismoRegistradoDto>>
                {
                    Result = _vhRepo.ObtenerVwOrganismoRegistrado().Select(item => _mapper.Map<VwOrganismoRegistradoDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerOrganismoRegistrado));
            }
        }

        /// <summary>
        /// ObtenerOrganizacionEsquema
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de organizaciones y sus esquemas asociados.
        /// </returns>
        [Authorize]
        [HttpGet("organizacion-esquema")]
        public IActionResult ObtenerOrganizacionEsquema()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwOrganizacionEsquemaDto>>
                {
                    Result = _vhRepo.ObtenerVwOrganizacionEsquema().Select(item => _mapper.Map<VwOrganizacionEsquemaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerOrganizacionEsquema));
            }
        }

        /// <summary>
        /// ObtenerOrganismoActividad
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de organismos y las actividades que realizan.
        /// </returns>
        [Authorize]
        [HttpGet("organismo-actividad")]
        public IActionResult ObtenerOrganismoActividad()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwOrganismoActividadDto>>
                {
                    Result = _vhRepo.ObtenerVwOrganismoActividad().Select(item => _mapper.Map<VwOrganismoActividadDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerOrganismoActividad));
            }
        }



    }
}
