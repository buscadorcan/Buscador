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
        public async Task<IActionResult> UploadIcon([FromForm] IFormFile file, [FromForm] int idONA)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No se ha enviado un archivo válido.");

                // Confirmar que el idONA se recibe correctamente
                Console.WriteLine($"ID ONA recibido: {idONA}");

                // Procesar el archivo normalmente...
                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                var allowedExtensions = new[] { ".png", ".svg" };

                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                    return BadRequest("Formato de archivo no permitido.");

                var uniqueFileName = $"{idONA}{extension}"; // Guardar con el ID como nombre
                var folderPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Icono");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"Icono/{uniqueFileName}";
                return Ok(new { FilePath = relativePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al procesar el archivo. {ex.Message}");
            }
        }



    }
}
