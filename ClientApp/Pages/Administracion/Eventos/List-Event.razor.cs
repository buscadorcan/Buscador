using ClientApp.Services;
using ClientApp.Services.IService;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using SharedApp.Models.Dtos;
using System.Text.Json;

namespace ClientApp.Pages.Administracion.Eventos
{
    public partial class List_Event
    {
        private DateOnly fini { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly ffin { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Inject]
        public IEventService? iEventService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        private List<VwEventUserAllDto>? listaRep;
        private string sortColumn = nameof(EventUserDto.Nombre);
        private bool sortAscending = true;
        private bool isLoading;
        private string selectReport;
        private List<EventUserDto> listasEvent = new();
        private int ProgressValue { get; set; } = 0;
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;
        private IEnumerable<EventUserDto> PaginatedItems => listasEvent
              .Skip((CurrentPage - 1) * PageSize)
              .Take(PageSize);
        private int TotalPages => listasEvent.Count > 0 ? (int)Math.Ceiling((double)listasEvent.Count / PageSize) : 1;
        private bool CanGoPrevious => CurrentPage > 1;
        private bool CanGoNext => CurrentPage < TotalPages;
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }
        protected override async Task OnInitializedAsync()
        {
            listaRep = await iEventService.GetListEventUserAllAsync();
        }
        private void OrdenarPor(string column)
        {
            if (sortColumn == column)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                sortColumn = column;
                sortAscending = true;
            }

            listasEvent = sortAscending
                ? listasEvent.OrderBy(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList()
                : listasEvent.OrderByDescending(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList();
        }
        private void SelectValue(ChangeEventArgs e)
        {
            selectReport = e.Value.ToString();
            clearData();
        }

        private async Task SearchEvent()
        {
            try
            {
                if (selectReport != null)
                {
                    isLoading = true;
                    listasEvent = await iEventService.GetEventAsync(selectReport, fini, ffin) ?? new List<EventUserDto>();
                }
  
            }
            finally
            {
                isLoading = false;
               
            }

            StateHasChanged();
        }


        private async Task ExportarExcel()
        {
            if (listasEvent == null || !listasEvent.Any())
            {
                return;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Configurar licencia para EPPlus

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Eventos");

            // Agregar encabezados
            worksheet.Cells[1, 1].Value = "Siglas";
            worksheet.Cells[1, 2].Value = "Nombre";
            worksheet.Cells[1, 3].Value = "Apellido";
            worksheet.Cells[1, 4].Value = "Pagina";
            worksheet.Cells[1, 5].Value = "Pagina Control";
            worksheet.Cells[1, 6].Value = "Fecha";

            if (selectReport == "vw_EventUserSEARCH")
            {
                worksheet.Cells[1, 7].Value = "Texto Buscar";
                worksheet.Cells[1, 8].Value = "Exacta Buscar";
                worksheet.Cells[1, 9].Value = "Filtro Pais";
                worksheet.Cells[1, 9].Value = "Filtro Ona";
                worksheet.Cells[1, 9].Value = "Filtro Estado";
            }
            

            int row = 2;
            foreach (var even in listasEvent)
            {
                worksheet.Cells[row, 1].Value = even.OnaSiglas;
                worksheet.Cells[row, 2].Value = even.Nombre;
                worksheet.Cells[row, 3].Value = even.Apellido;
                worksheet.Cells[row, 4].Value = even.Pagina;
                worksheet.Cells[row, 5].Value = even.PaginaControl;
                worksheet.Cells[row, 6].Value = even.FechaCreacion;

                if (selectReport == "vw_EventUserSEARCH")
                {
                    worksheet.Cells[row, 7].Value = even.TextoBuscar;
                    worksheet.Cells[row, 8].Value = even.ExactaBuscar;
                    worksheet.Cells[row, 9].Value = even.FiltroPais;
                    worksheet.Cells[row, 9].Value = even.FiltroOna;
                    worksheet.Cells[row, 9].Value = even.FiltroEstado;
                }
               
                row++;
            }

            worksheet.Cells.AutoFitColumns(); // Ajustar automáticamente las columnas

            var fileName = "Eventos_Export.xlsx";
            var fileBytes = package.GetAsByteArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);

            await JSRuntime.InvokeVoidAsync("downloadExcel", fileName, fileBase64);
        }
        private async Task ExportarPDF()
        {
 
            if (listasEvent == null || !listasEvent.Any())
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            var document = new Document(iTextSharp.text.PageSize.A4);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var table = new PdfPTable(7) { WidthPercentage = 100 };

            foreach (var header in new[] { "Siglas", "Nombre", "Apellido", "Pagina", "Pagina Control", "Fecha"})
            {
                table.AddCell(new Phrase(header, font));
            }

            foreach (var even in listasEvent)
            {
                table.AddCell(even.OnaSiglas ?? "-");
                table.AddCell(even.Nombre ?? "-");
                table.AddCell(even.Apellido ?? "-");
                table.AddCell(even.Pagina ?? "-");
                table.AddCell(even.PaginaControl ?? "-");
                table.AddCell(even.FechaCreacion.ToString() ?? "-");
            }

            document.Add(table);
            document.Close();

            var fileName = "Eventos_Export.pdf";
            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, "application/pdf", Convert.ToBase64String(memoryStream.ToArray()));
        }

        private async Task DeleteEventAll()
        {
            var result = await JSRuntime.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "¿Estás seguro?",
                text = "Esta acción eliminará todo y no podrá ser consultado nuevamente.",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#d33",
                cancelButtonColor = "#3085d6",
                confirmButtonText = "Sí, eliminar",
                cancelButtonText = "Cancelar"
            });

            bool isConfirmed = result.TryGetProperty("isConfirmed", out JsonElement isConfirmedElement) && isConfirmedElement.GetBoolean();

            if (isConfirmed)
            {

                var success = await iEventService.DeleteEventAllAsync(selectReport, fini, ffin);

                if (success)
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Eliminado", "Los registros han sido eliminados correctamente.", "success");
                    await SearchEvent();
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Error", "No se pudieron eliminar los registro.", "error");
                }
            }
        }

        private async Task DeleteByIdAsync(int codigoEvento)
        {

            var result = await JSRuntime.InvokeAsync<JsonElement>("Swal.fire", new
            {
                title = "¿Estás seguro?",
                text = "Esta acción eliminará el registro y no podrá ser consultado nuevamente.",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#d33",
                cancelButtonColor = "#3085d6",
                confirmButtonText = "Sí, eliminar",
                cancelButtonText = "Cancelar"
            });

            // Extraer isConfirmed del JsonElement
            bool isConfirmed = result.TryGetProperty("isConfirmed", out JsonElement isConfirmedElement) && isConfirmedElement.GetBoolean();

            if (isConfirmed)
            {
                var success = await iEventService.DeleteEventByIdAsync(selectReport, codigoEvento);

                if (success)
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Eliminado", "El registro ha sido eliminado correctamente.", "success");
                    await SearchEvent();
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Error", "No se pudo eliminar el registro.", "error");
                }
            }
        }

        private void clearData()
        {
            listasEvent.Clear();
        }
    }
}