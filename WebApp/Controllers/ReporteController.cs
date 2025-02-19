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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/findByVista: Obtiene la homologación por código de homologación.
         */
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

                // Mapear el objeto a VwHomologacionDto
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerAcreditacionOna: Obtiene la acreditación de ONAs.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerAcreditacionEsquema: Obtiene la acreditación de esquemas.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerEstadoEsquema: Obtiene el estado de los esquemas.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerOecPais: Obtiene información sobre OECs por país.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerEsquemaPais: Obtiene información de esquemas por país.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerOecFecha: Obtiene información de OECs filtrados por fecha.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerProfesionalCalificado: Obtiene información sobre profesionales calificados en el sistema.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerProfesionalOna: Obtiene información sobre profesionales en ONAs.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerProfesionalEsquema: Obtiene información de profesionales por esquema.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerProfesionalFecha: Obtiene información de profesionales filtrados por fecha.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerCalificaUbicacion: Obtiene información de calificaciones por ubicación.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerBusquedaFecha: Obtiene información de búsquedas filtradas por fecha.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerBusquedaFiltro: Obtiene información de búsquedas aplicando filtros específicos.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerBusquedaUbicacion: Obtiene información sobre búsquedas realizadas por ubicación.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerActualizacionONA: Obtiene información sobre la actualización de ONAs.
         */
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


        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerOrganismoRegistrado: Obtiene información sobre organismos registrados.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerOrganizacionEsquema: Obtiene información sobre la relación entre organizaciones y esquemas.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerOrganismoActividad: Obtiene información sobre organismos y sus actividades.
         */
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
