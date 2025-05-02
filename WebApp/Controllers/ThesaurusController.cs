/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/ThesaurusController: Controlador para funcionalidad en Thesaurus
using Microsoft.AspNetCore.Mvc;
using SharedApp.Response;
using AutoMapper;
using SharedApp.Dtos;
using Core.Interfaces;
using DataAccess.Models;

namespace WebApp.Controllers
{
    [Route(Routes.THESAURUS)]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ThesaurusController : BaseController
    {
        private readonly IThesaurusService _thesaurusService;
        private readonly IMapper _mapper;

        public ThesaurusController(IThesaurusService thesaurusService, 
            IMapper mapper, 
            ILogger<BaseController> logger): base(logger)
        {
            this._thesaurusService = thesaurusService;
            this._mapper = mapper;
        }

        /// <summary>
        /// ObtenerThesaurus
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con el contenido del thesaurus actual.
        /// </returns>
        ///</summary>
        [HttpGet(Routes.GET_THESAURUS)]
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

        /// <summary>
        /// AgregarExpansion
        /// </summary>
        /// <param name="sinonimos">Lista de sinónimos a agregar como una nueva expansión.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la operación fue exitosa.
        /// </returns>
        [HttpPost(Routes.THESAURUS_AGREGAR)]
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

        /// <summary>
        /// ActualizarExpansion.
        /// </summary>
        /// <param name="expansions">Lista de expansiones a actualizar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la actualización fue exitosa.
        /// </returns>
        [HttpPost(Routes.THESAURUS_ACTUALIZAR)]
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
                return HandleException(e, nameof(ActualizarExpansion));
            }
        }

        /// <summary>
        /// AgregarSubExpansion
        /// </summary>
        /// <param name="expansionExistente">Nombre de la expansión a la que se añadirá el sub-sinónimo.</param>
        /// <param name="nuevoSub">Término que se añadirá como sub-sinónimo.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la operación fue exitosa.
        /// </returns>
        [HttpGet(Routes.THESAURUS_AGREGAR_SUB)]
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

        /// <summary>
        /// EjecutarBat
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la ejecución del archivo BAT fue exitosa.
        /// </returns>
        ///</summary>
        [HttpGet(Routes.THESAURUS_EJECUTAR)]
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
                return HandleException(e, nameof(EjecutarBat));
            }
        }

        /// <summary>
        /// ResetSQLServer
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la actualización de SQL Server fue exitosa.
        /// </returns>
        ///</summary>
        [HttpGet(Routes.THESAURUS_RESET)]
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
                return HandleException(e, nameof(ResetSQLServer));
            }
        }

    }
}
