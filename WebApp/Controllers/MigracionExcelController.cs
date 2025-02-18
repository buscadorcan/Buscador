using WebApp.Models;
using WebApp.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Service.IService;

namespace WebApp.Controllers
{
  [Route("api/migracionexcel")]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public class MigacionExcelController(
    IMigracionExcelRepository iRepo,
    IMapper mapper,
    IExcelService importer
  ) : BaseController
  {
        private readonly IMigracionExcelRepository _iRepo = iRepo;
        private readonly IMapper _mapper = mapper;
        [Authorize]
        [HttpGet]
        public IActionResult FindAll()
        {
              try
              {
                return Ok(new RespuestasAPI<List<MigracionExcelDto>> {
                  Result = _iRepo.FindAll().Select(item => _mapper.Map<MigracionExcelDto>(item)).ToList()
                });
              }
              catch (Exception e)
              {
                return HandleException(e, nameof(FindAll));
              }
        }

        //[Authorize]
        //[HttpPost("upload")]
        //public IActionResult ImportarExcel(IFormFile file, [FromQuery] int idOna)
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //        {
        //            return BadRequestResponse("Archivo no encontrado");
        //        }
        //        if (idOna <= 0)
        //        {
        //            return BadRequestResponse("idOna no es válido");
        //        }

        //        string fileExtension = Path.GetExtension(file.FileName);
        //        if (fileExtension != ".xls" && fileExtension != ".xlsx")
        //        {
        //            return BadRequestResponse("Archivo no válido");
        //        }

        //        //SOLUCIÓN: Obtener la ruta del directorio del proyecto sin depender del bin
        //        string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        //        string filesPath = Path.Combine(projectRoot, "WebApp", "wwwroot", "Files");

        //        // Crear la carpeta "Files" si no existe
        //        if (!Directory.Exists(filesPath))
        //        {
        //            Directory.CreateDirectory(filesPath);
        //        }

        //        var filePath = Path.Combine(filesPath, file.FileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            file.CopyTo(stream);
        //        }

        //        LogMigracion migracion = iRepo.Create(new LogMigracion
        //        {
        //            Estado = "PENDING",
        //            ExcelFileName = file.FileName
        //        });

        //        var result = importer.ImportarExcel(filePath, migracion, idOna);

        //        return Ok(new RespuestasAPI<bool>
        //        {
        //            IsSuccess = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { statusCode = 500, isSuccess = false, errorMessages = new[] { ex.Message } });
        //    }
        //}

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

                // 🔹 Obtener la ruta de `wwwroot/Files` correctamente en IIS
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string filesPath = Path.Combine(wwwrootPath, "Files");

                // 🔹 Asegurar que la carpeta "Files" existe, si no, crearla
                if (!Directory.Exists(filesPath))
                {
                    Directory.CreateDirectory(filesPath);
                }

                // 🔹 Construir la ruta final del archivo
                string filePath = Path.Combine(filesPath, file.FileName);

                // 🔹 Guardar el archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // 🔹 Guardar en base de datos
                LogMigracion migracion = iRepo.Create(new LogMigracion
                {
                    Estado = "PENDING",
                    ExcelFileName = file.FileName
                });

                var result = importer.ImportarExcel(filePath, migracion, idOna);

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

        //[Authorize]
        //[HttpPost("upload")]
        //public IActionResult ImportarExcel(IFormFile file, [FromQuery] int idOna)
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //        {
        //            return BadRequestResponse("Archivo no encontrado");
        //        }
        //        if (idOna <= 0)
        //        {
        //            return BadRequestResponse("idOna no es válido");
        //        }

        //        string fileExtension = Path.GetExtension(file.FileName);
        //        if (fileExtension != ".xls" && fileExtension != ".xlsx")
        //        {
        //            return BadRequestResponse("Archivo no válido");
        //        }

        //        var path = Path.Combine(Directory.GetCurrentDirectory(), "Files", file.FileName);

        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            file.CopyTo(stream);
        //        }

        //        LogMigracion migracion = iRepo.Create(new LogMigracion
        //        {
        //            Estado = "PENDING",
        //            ExcelFileName = file.FileName
        //        });

        //        var result = importer.ImportarExcel(path, migracion, idOna);

        //        return Ok(new RespuestasAPI<bool>
        //        {
        //            IsSuccess = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { statusCode = 500, isSuccess = false, errorMessages = new[] { ex.Message } });
        //        //return HandleException(e, nameof(ImportarExcel));
        //    }
        //}

    }
}
