/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/MigacionExcelController: Controlador para funcionalidad de migración excel
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Response;
using DataAccess.Interfaces;
using SharedApp.Dtos;
using Core.Interfaces;
using DataAccess.Models;

namespace WebApp.Controllers
{
  [Route("api/migracionexcel")]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public class MigacionExcelController: BaseController
  {
        private readonly IMigracionExcelService _migracionExcelService;
        private readonly IMapper _mapper;
        private readonly IExcelService _importer;

        public MigacionExcelController(
                IMigracionExcelService migracionExcelService,
                IMapper mapper,
                IExcelService importer,
                ILogger<BaseController> logger
                ): base(logger)
        {
            this._migracionExcelService = migracionExcelService;
            this._mapper = mapper;
            this._importer = importer;
        }

        /// <summary>
        /// FindAll
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de MigracionExcelDto representando los registros de migración.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<MigracionExcelDto>>
                {
                    Result = _migracionExcelService.FindAll().Select(item => _mapper.Map<MigracionExcelDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAll));
            }
        }

        /// <summary>
        /// ImportarExcel
        /// </summary>
        /// <param name="file">Archivo Excel a importar.</param>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA) al que se asocia la importación.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la importación fue exitosa.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpPost("upload")]
        public IActionResult ImportarExcel(IFormFile file, [FromQuery] int idOna)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequestResponse("Archivo no encontrado");
                }
                if (idOna <= 0)
                {
                    return BadRequestResponse("idOna no es válido");
                }

                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    return BadRequestResponse("Archivo no válido");
                }

                // Obtener la ruta de `wwwroot/Files` correctamente en IIS
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string filesPath = Path.Combine(wwwrootPath, "Files");

                // Asegurar que la carpeta "Files" existe, si no, crearla
                if (!Directory.Exists(filesPath))
                {
                    Directory.CreateDirectory(filesPath);
                }

                // Construir la ruta final del archivo
                string filePath = Path.Combine(filesPath, file.FileName);

                // Guardar el archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Guardar en base de datos
                LogMigracion migracion = _migracionExcelService.Create(new LogMigracion
                {
                    Estado = "PENDING",
                    ExcelFileName = file.FileName
                });

                var result = _importer.ImportarExcel(filePath, migracion, idOna);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    isSuccess = false,
                    errorMessages = new[] { ex.Message }
                });
            }
        }

    }
}
