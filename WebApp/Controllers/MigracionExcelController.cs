using WebApp.Models;
using WebApp.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Service.IService;
using System.Security.AccessControl;

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
                    return BadRequest(new { message = "Archivo no encontrado" });
                }
                if (idOna <= 0)
                {
                    return BadRequest(new { message = "idOna no es válido" });
                }

                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    return BadRequest(new { message = "Archivo no válido" });
                }

                // 🔹 Obtener la ruta correcta de "wwwroot/Files" dentro del proyecto
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string filesPath = Path.Combine(wwwrootPath, "Files");

                // 🔹 Verificar y crear la carpeta con permisos adecuados
                EnsureDirectoryExistsWithPermissions(filesPath);

                // 🔹 Normalizar el nombre del archivo para evitar errores con espacios
                string safeFileName = file.FileName.Replace(" ", "_");
                string filePath = Path.Combine(filesPath, safeFileName);

                // 🔹 Guardar el archivo en "Files"
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // 🔹 Registrar en la base de datos
                LogMigracion migracion = _iRepo.Create(new LogMigracion
                {
                    Estado = "PENDING",
                    ExcelFileName = safeFileName
                });

                var result = importer.ImportarExcel(filePath, migracion, idOna);

                return Ok(new { isSuccess = true, message = "Archivo subido con éxito." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(500, new { statusCode = 500, isSuccess = false, errorMessages = new[] { "Permisos insuficientes en la carpeta 'Files'. Contacte al administrador del servidor.", ex.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, isSuccess = false, errorMessages = new[] { ex.Message } });
            }
        }
        private void EnsureDirectoryExistsWithPermissions(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                // 🔹 Asignar permisos de escritura a IIS_IUSRS
                var directoryInfo = new DirectoryInfo(path);
                var security = directoryInfo.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(
                    "IIS_IUSRS",
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                security.AddAccessRule(new FileSystemAccessRule(
                    "IUSR",
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                directoryInfo.SetAccessControl(security);
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
