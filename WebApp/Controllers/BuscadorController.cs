using Microsoft.AspNetCore.Mvc;
using SharedApp.Response;
using System.Text.Json;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SharedApp.Dtos;
using Core.Interfaces;


namespace WebApp.Controllers
{
    /// <summary>
    /// Controlador para las operaciones de búsqueda y utilidades de exportación.
    /// </summary>
    [ApiController]
    [Route(Routes.BUSCADOR)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class BuscadorController : BaseController
    {
        private readonly IBuscadorService _vhRepo;

        public BuscadorController(
            IBuscadorService vhRepo,
            ILogger<BaseController> logger)
            : base(logger)
        {
            _vhRepo = vhRepo;
        }

        // ---------------------------- MÉTODOS ----------------------------

        [HttpGet(Routes.SEARCH_PHRASE)]
        public IActionResult PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage)
        {
            try
            {
                return Ok(new RespuestasAPI<BuscadorDto>
                {
                    Result = _vhRepo.PsBuscarPalabra(paramJSON, PageNumber, RowsPerPage)
                });
            }
            catch (Exception e) { return HandleException(e, nameof(PsBuscarPalabra)); }
        }

        [HttpGet(Routes.HOMOLOGACION_ESQUEMA_TODO)]
        public IActionResult FnHomologacionEsquemaTodo(string VistaFk, int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<EsquemaDto>>
                {
                    Result = _vhRepo.FnHomologacionEsquemaTodo(VistaFk, idOna)
                });
            }
            catch (Exception e) { return HandleException(e, nameof(FnHomologacionEsquemaTodo)); }
        }

        [HttpGet($"{Routes.HOMOLOGACION_ESQUEMA_ID}/{{idEsquema:int}}")]
        public IActionResult FnHomologacionEsquema(int idEsquema)
        {
            try
            {
                return Ok(new RespuestasAPI<FnEsquemaDto>
                {
                    Result = _vhRepo.FnHomologacionEsquema(idEsquema)
                });
            }
            catch (Exception e) { return HandleException(e, nameof(FnHomologacionEsquema)); }
        }

        [HttpGet($"{Routes.FN_ESQUEMA_CABECERA_ID}/{{IdEsquemadata:int}}")]
        public IActionResult FnEsquemaCabecera(int IdEsquemadata)
        {
            try
            {
                return Ok(new RespuestasAPI<fnEsquemaCabeceraDto>
                {
                    Result = _vhRepo.FnEsquemaCabecera(IdEsquemadata)
                });
            }
            catch (Exception e) { return HandleException(e, nameof(FnEsquemaCabecera)); }
        }

        [HttpGet($"{Routes.HOMOLOGACION_ESQUEMA_DATO}/{{idEsquema:int}}/{{idOna:int}}")]
        public IActionResult FnHomologacionEsquemaDato(int idEsquema, string VistaFK, int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnHomologacionEsquemaDataDto>>
                {
                    Result = _vhRepo.FnHomologacionEsquemaDato(idEsquema, VistaFK, idOna)
                });
            }
            catch (Exception e) { return HandleException(e, nameof(FnHomologacionEsquemaDato)); }
        }

        [HttpGet(Routes.ESQUEMA_DATO_BUSCADO)]
        public IActionResult FnEsquemaDato(int idEsquemaData, string TextoBuscar)
        {
            try
            {
                var result = _vhRepo.FnEsquemaDatoBuscar(idEsquemaData, TextoBuscar);
                return Ok(new RespuestasAPI<List<FnEsquemaDataBuscadoDto>> { Result = result });
            }
            catch (Exception e) { return HandleException(e, nameof(FnEsquemaDato)); }
        }

        [HttpGet(Routes.PREDIC_WORDS)]
        public IActionResult FnPredictWords(string word)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnPredictWordsDto>>
                {
                    Result = _vhRepo.FnPredictWords(word)
                });
            }
            catch (Exception e) { return HandleException(e, nameof(FnPredictWords)); }
        }

        [HttpPost(Routes.PREDIC_WORDS)]
        public IActionResult ValidateWords([FromBody] List<string> words)
        {
            try
            {
                return Ok(new RespuestasAPI<bool> { Result = _vhRepo.ValidateWords(words) });
            }
            catch (Exception e) { return HandleException(e, nameof(ValidateWords)); }
        }

        [HttpPost(Routes.EVENTTRACKING)]
        public IActionResult AddEventTracking([FromBody] EventTrackingDto eventTracking)
        {
            try
            {
                // IP & ubicación
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var ipInfo = GetIpLocationInfo(ipAddress).Result;

                eventTracking.UbicacionJson = JsonSerializer.Serialize(new
                {
                    IpAddress = ipAddress,
                    ipInfo.Country,
                    ipInfo.City,
                    ipInfo.Isp
                });

                _vhRepo.AddEventTracking(eventTracking);

                return Ok(new RespuestasAPI<string> { Result = "Evento registrado con éxito." });
            }
            catch (Exception e) { return HandleException(e, nameof(AddEventTracking)); }
        }

        [HttpGet(Routes.GEO_CODE)]
        public async Task<IActionResult> GetCoordinates([FromQuery] string address)
        {
            try
            {
                var apiKey = "TU_API_KEY";
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);

                return Ok(response);
            }
            catch (Exception e) { return HandleException(e, nameof(GetCoordinates)); }
        }

        // -------------------- UTILIDADES PRIVADAS --------------------

        private async Task<IpLocationDto> GetIpLocationInfo(string ipAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
                    return new IpLocationDto { Country = "Local", City = "Local", Isp = "Local" };

                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync($"http://ip-api.com/json/{ipAddress}");
                return JsonSerializer.Deserialize<IpLocationDto>(response) ?? new();
            }
            catch
            {
                return new IpLocationDto { Country = "Desconocido", City = "Desconocido", Isp = "Desconocido" };
            }
        }

        // -------------------- EXPORTACIÓN --------------------

        [HttpPost(Routes.EXCEL)]
        public IActionResult ExportExcel([FromBody] List<Dictionary<string, string>> data)
        {
            if (data.Count == 0) return BadRequest("No hay datos para exportar");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Datos");

            var headers = data[0].Keys.ToList();
            for (int i = 0; i < headers.Count; i++) ws.Cells[1, i + 1].Value = headers[i];

            for (int i = 0; i < data.Count; i++)
                for (int j = 0; j < headers.Count; j++)
                    ws.Cells[i + 2, j + 1].Value = data[i][headers[j]];

            return File(package.GetAsByteArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "exportacion.xlsx");
        }

        [HttpPost(Routes.PDF)]
        public IActionResult ExportPdf([FromBody] List<Dictionary<string, string>> data)
        {
            using var stream = new MemoryStream();
            var doc = new Document();
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            if (data.Count == 0)
            {
                doc.Add(new Paragraph("No hay datos para exportar"));
            }
            else
            {
                var headers = data[0].Keys.ToList();
                var table = new PdfPTable(headers.Count);

                foreach (var header in headers)
                    table.AddCell(new PdfPCell(new Phrase(header)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                foreach (var row in data)
                    foreach (var header in headers)
                        table.AddCell(row.TryGetValue(header, out var v) ? v : string.Empty);

                doc.Add(table);
            }

            doc.Close();
            return File(stream.ToArray(), "application/pdf", "exportacion.pdf");
        }
    }
}
