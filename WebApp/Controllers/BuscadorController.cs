/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/BuscadorController: Controlador para formulario del buscador
using Microsoft.AspNetCore.Mvc;
using SharedApp.Response;
using System.Text.Json;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DataAccess.Interfaces;
using SharedApp.Dtos;
using Core.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/buscador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public class BuscadorController(IBuscadorService vhRepo) : BaseController
    {
        private readonly IBuscadorService _vhRepo = vhRepo;
        
        /// <summary>
        /// PsBuscarPalabra
        /// </summary>
        /// <param name="paramJSON"></param>
        /// <param name="PageNumber"></param>
        /// <param name="RowsPerPage"></param>
        /// <returns></returns>
       [HttpGet("search/phrase")]
        public IActionResult PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage)
        {
            try
            {
                return Ok(new RespuestasAPI<BuscadorDto>
                {
                    Result = _vhRepo.PsBuscarPalabra(paramJSON, PageNumber, RowsPerPage)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(PsBuscarPalabra));
            }
        }

        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/FnHomologacionEsquemaTodo: Obtiene todos los esquemas homologados en función de una vista y un identificador de ONA.
        /// </summary>
        /// <param name="VistaFk">Nombre de la vista en la base de datos.</param>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación.</param>
        /// <returns>Lista de esquemas homologados.</returns>
        [HttpGet("homologacionEsquemaTodo")]
        public IActionResult FnHomologacionEsquemaTodo(string VistaFk, int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<EsquemaDto>>
                {
                    Result = _vhRepo.FnHomologacionEsquemaTodo(VistaFk, idOna)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaTodo));
            }
        }

        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/FnHomologacionEsquema: Recupera la información de un esquema homologado específico a partir de su identificador.
        /// </summary>
        /// <param name="idEsquema">Identificador único del esquema.</param>
        /// <returns>Información del esquema homologado.</returns>
        [HttpGet("homologacionEsquema/{idEsquema}")]
        public IActionResult FnHomologacionEsquema(int idEsquema)
        {
            try
            {
                return Ok(new RespuestasAPI<FnEsquemaDto>
                {
                    Result = _vhRepo.FnHomologacionEsquema(idEsquema)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaTodo));
            }
        }

        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/FnEsquemaCabecera: Obtiene la cabecera de un esquema a partir de su identificador.
        /// </summary>
        /// <param name="IdEsquemadata">Identificador del esquema.</param>
        /// <returns>Información de la cabecera del esquema.</returns>
        [HttpGet("fnesquemacabecera/{IdEsquemadata}")]
        public IActionResult FnEsquemaCabecera(int IdEsquemadata)
        {
            try
            {
                return Ok(new RespuestasAPI<fnEsquemaCabeceraDto>
                {
                    Result = _vhRepo.FnEsquemaCabecera(IdEsquemadata)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnEsquemaCabecera));
            }
        }


        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/FnHomologacionEsquemaDato: Obtiene los datos de homologación de un esquema en función de su identificador, vista y ONA.
        /// </summary>
        /// <param name="idEsquema">Identificador del esquema.</param>
        /// <param name="VistaFK">Vista relacionada en la base de datos.</param>
        /// <param name="idOna">Identificador del ONA.</param>
        /// <returns>Lista de datos homologados del esquema.</returns>
        [HttpGet("homologacionEsquemaDato/{idEsquema}/{idOna}")]
        public IActionResult FnHomologacionEsquemaDato(int idEsquema,string VistaFK, int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnHomologacionEsquemaDataDto>>
                {
                    Result = _vhRepo.FnHomologacionEsquemaDato(idEsquema, VistaFK, idOna)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaDato));
            }
        }

        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/FnEsquemaDato: Realiza una búsqueda de datos dentro de un esquema específico según el texto ingresado.
        /// </summary>
        /// <param name="idEsquemaData">Identificador del esquema de datos.</param>
        /// <param name="TextoBuscar">Texto a buscar dentro del esquema.</param>
        /// <returns>Lista de datos coincidentes.</returns>
        [HttpGet("EsquemaDatoBuscado")]
        public IActionResult FnEsquemaDato(int idEsquemaData, string TextoBuscar)
        {
            try
            {
                var result = _vhRepo.FnEsquemaDatoBuscar(idEsquemaData, TextoBuscar);
                return Ok(new RespuestasAPI<List<FnEsquemaDataBuscadoDto>>
                {
                    Result = result
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnEsquemaDato));
            }
        }


        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/FnPredictWords: Predice palabras basadas en el inicio de una palabra ingresada.
        /// </summary>
        /// <param name="word">Texto de entrada para predicción.</param>
        /// <returns>Lista de palabras sugeridas.</returns>
        [HttpGet("predictWords")]
        public IActionResult FnPredictWords(string word)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnPredictWordsDto>>
                {
                    Result = _vhRepo.FnPredictWords(word)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnPredictWords));
            }
        }

        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/ValidateWords: Valida una lista de palabras ingresadas por el usuario.
        /// </summary>
        /// <param name="words">Lista de palabras a validar.</param>
        /// <returns>Resultado de validación (true o false).</returns>
        [HttpPost("validateWords")]
        public IActionResult ValidateWords([FromBody] List<string> words)
        {
            try
            {
                return Ok(new RespuestasAPI<bool>
                {
                    Result = _vhRepo.ValidateWords(words)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ValidateWords));
            }
        }

        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/AddEventTracking: Registra la acción realizada por un control.
        /// </summary>
        /// <param name="eventTracking">Objeto con los detalles del evento a registrar.</param>
        /// <returns>Mensaje de confirmación.</returns>
        [HttpPost("addEventTracking")]
        public IActionResult AddEventTracking([FromBody] EventTrackingDto eventTracking)
        {
            try
            {
                // Obtener la IP del cliente
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                // Obtener datos del país
                var ipInfo = GetIpLocationInfo(ipAddress);

                eventTracking.UbicacionJson = JsonSerializer.Serialize(new
                {
                    IpAddress = ipAddress,
                    Country = ipInfo.Result?.Country,
                    City = ipInfo.Result?.City,
                    Isp = ipInfo.Result?.Isp
                });

                if (eventTracking == null)
                    return BadRequest("El objeto EventTracking no puede ser nulo.");

                _vhRepo.AddEventTracking(eventTracking);

                return Ok(new RespuestasAPI<string>
                {
                    Result = "Evento registrado con éxito."
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AddEventTracking));
            }
        }

        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/GetCoordinates: Obtiene coordenadas geográficas (latitud y longitud) de una dirección dada mediante la API de Google Maps.
        /// </summary>
        /// <param name="address">Dirección a geolocalizar.</param>
        /// <returns>Coordenadas geográficas en formato JSON.</returns>
        [HttpGet("geocode")]
        public async Task<IActionResult> GetCoordinates([FromQuery] string address)
        {
            try
            {
                var apiKey = "AIzaSyC7NUCEvrqrrQDDDRLK2q0HSqswPxtBVAk"; // Reemplaza con tu API Key
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);

                return Ok(response);
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetCoordinates));
            }
        }

        private async Task<IpLocationDto> GetIpLocationInfo(string ipAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1") // Verifica si es localhost
                    return new IpLocationDto { Country = "Local", City = "Local", Isp = "Local" };

                using var httpClient = new HttpClient();
                var url = $"http://ip-api.com/json/{ipAddress}"; // API gratuita de geolocalización
                var response = await httpClient.GetStringAsync(url);

                return JsonSerializer.Deserialize<IpLocationDto>(response);
            }
            catch
            {
                return new IpLocationDto { Country = "Desconocido", City = "Desconocido", Isp = "Desconocido" };
            }
        }

        [HttpPost("excel")]
        public IActionResult ExportExcel([FromBody] List<Dictionary<string, string>> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Datos");

            if (data.Count == 0)
                return BadRequest("No hay datos para exportar");

            var headers = data[0].Keys.ToList();

            // Cabeceras
            for (int i = 0; i < headers.Count; i++)
                ws.Cells[1, i + 1].Value = headers[i];

            // Filas
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < headers.Count; j++)
                {
                    ws.Cells[i + 2, j + 1].Value = data[i][headers[j]];
                }
            }

            var stream = package.GetAsByteArray();
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "exportacion.xlsx");
        }


        [HttpPost("pdf")]
        public IActionResult ExportPdf([FromBody] List<Dictionary<string, string>> data)
        {
            using var stream = new MemoryStream();
            var doc = new iTextSharp.text.Document();
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            if (data.Count == 0)
            {
                doc.Add(new Paragraph("No hay datos para exportar"));
                doc.Close();
                return File(stream.ToArray(), "application/pdf", "exportacion.pdf");
            }

            var headers = data[0].Keys.ToList();
            var table = new PdfPTable(headers.Count);

            foreach (var header in headers)
                table.AddCell(new PdfPCell(new Phrase(header)) { BackgroundColor = BaseColor.LIGHT_GRAY });

            foreach (var fila in data)
            {
                foreach (var header in headers)
                {
                    var valor = fila.ContainsKey(header) ? fila[header] : "";
                    table.AddCell(valor);
                }
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(), "application/pdf", "exportacion.pdf");
        }


    }
}
