using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public UtilitiesController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("UploadIcon")]
        public async Task<IActionResult> UploadIcon([FromForm] IFormFile file)
        {
            try
            {
                // Validar si el archivo está presente
                if (file == null || file.Length == 0)
                    return BadRequest("No se ha enviado un archivo válido.");

                // Verificar la extensión del archivo
                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg" };
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                    return BadRequest("Formato de archivo no permitido.");

                // Validar el tamaño del archivo (por ejemplo, no mayor a 2 MB)
                const long maxSizeInBytes = 2 * 1024 * 1024; // 2 MB
                if (file.Length > maxSizeInBytes)
                    return BadRequest("El archivo excede el tamaño máximo permitido de 2 MB.");

                // Generar un nombre único para el archivo
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";

                // Ruta absoluta de la carpeta "Icono" en ContentRootPath
                var folderPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Icono");

                // Crear la carpeta si no existe
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Ruta completa donde se guardará el archivo
                var filePath = Path.Combine(folderPath, uniqueFileName);

                // Guardar el archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generar la ruta relativa para devolver
                var relativePath = $"/Icono/{uniqueFileName}";
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fileUrl = $"{baseUrl}{relativePath}";

                // Retornar la URL del archivo
                return Ok(new { FilePath = fileUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir archivo: {ex.Message}");
                return StatusCode(500, $"Error interno al procesar el archivo. {ex.Message}");
            }
        }


    }
}
