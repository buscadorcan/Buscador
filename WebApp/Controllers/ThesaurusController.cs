using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Service.IService;
using AutoMapper;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/thesaurus")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ThesaurusController(IThesaurusService thesaurusService, IMapper mapper) : BaseController
    {
        private readonly IThesaurusService _thesaurusService = thesaurusService;
        private readonly IMapper _mapper = mapper;


        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerThesaurus: Obtiene el thesaurus actual con sus expansiones y sinónimos.
         */
        [HttpGet("obtener/thesaurus")]
        public IActionResult ObtenerThesaurus()
        {
            try
            {
                return Ok(new RespuestasAPI<ThesaurusDto>
                {
                    Result = _mapper.Map<ThesaurusDto>(_thesaurusService.ObtenerThesaurus())
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerThesaurus));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/AgregarExpansion: Agrega una nueva expansión de sinónimos al thesaurus.
         */
        [HttpPost("agregar/expansion")]
        public IActionResult AgregarExpansion([FromBody] List<string> sinonimos)
        {
            try
            {
                return Ok(new RespuestasAPI<string>
                {
                    Result = _thesaurusService.AgregarExpansion(sinonimos)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AgregarExpansion));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ActualizarExpansion: Actualiza una expansión existente en el thesaurus.
         */
        [HttpPost("actualizar/expansion")]
        public IActionResult ActualizarExpansion([FromBody] List<ExpansionDto> expansions)
        {
            try
            {
                var exp = _mapper.Map<List<Expansion>>(expansions);
                return Ok(new RespuestasAPI<string>
                {
                    Result = _thesaurusService.ActualizarExpansion(exp)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AgregarExpansion));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/AgregarSubExpansion: Agrega un nuevo sub-sinónimo a una expansión existente en el thesaurus.
         */
        [HttpGet("agregar/expansion/{expansionExistente}/sub/{nuevoSub}")]
        public IActionResult AgregarSubExpansion([FromRoute] string expansionExistente, string nuevoSub)
        {
            try
            {
                return Ok(new RespuestasAPI<string>
                {
                    Result = _thesaurusService.AgregarSubAExpansion(expansionExistente, nuevoSub)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AgregarSubExpansion));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/EjecutarBat: Ejecuta un archivo BAT en el servidor para procesar el thesaurus.
         */
        [HttpGet("ejecutar/bat")]
        public IActionResult EjecutarBat()
        {
            try
            {
                return Ok(new RespuestasAPI<string>
                {
                    Result = _thesaurusService.EjecutarArchivoBat()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AgregarSubExpansion));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ResetSQLServer: actualiza el servidor de sqlserver.
         */
        [HttpGet("reset/sqlserver")]
        public IActionResult ResetSQLServer()
        {
            try
            {
                return Ok(new RespuestasAPI<string>
                {
                    Result = _thesaurusService.ResetSQLServer()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AgregarSubExpansion));
            }
        }
    }
}
