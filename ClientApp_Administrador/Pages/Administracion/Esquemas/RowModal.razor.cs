using Infractruture.Interfaces;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using SharedApp.Dtos;

namespace ClientAppAdministrador.Pages.Administracion.Esquemas
{
    /// <summary>
    /// Componente modal para mostrar una tabla paginada de homologaciones.
    /// </summary>
    public partial class RowModal
    {
        /// <summary>
        /// Lista de columnas que se mostrarán en la tabla.
        /// </summary>
        [Parameter] public List<HomologacionDto> columnas { get; set; }
        /// <summary>
        /// Lista de homologaciones disponibles para selección.
        /// </summary>
        [Parameter] public List<HomologacionDto> listaVwHomologacion { get; set;}
        /// <summary>
        /// Número de elementos por página en la tabla.
        /// </summary>
        private int PageSize = 10; // Cantidad de registros por página
        /// <summary>
        /// Página actual de la tabla.
        /// </summary>
        private int CurrentPage = 1;

        /// <summary>
        /// Obtiene los elementos paginados para mostrar en la tabla.
        /// </summary>
        private IEnumerable<HomologacionDto> PaginatedItems => columnas
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        /// <summary>
        /// Calcula el número total de páginas en función del tamaño de la lista.
        /// </summary>
        private int TotalPages => columnas.Count > 0 ? (int)Math.Ceiling((double)columnas.Count / PageSize) : 1;
        /// <summary>
        /// Determina si se puede retroceder a la página anterior.
        /// </summary>
        private bool CanGoPrevious => CurrentPage > 1;
        /// <summary>
        /// Determina si se puede avanzar a la siguiente página.
        /// </summary>
        private bool CanGoNext => CurrentPage < TotalPages;

        /// <summary>
        /// Método para navegar a la página anterior en la tabla.
        /// </summary>
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        /// <summary>
        /// Metodo para navegar a la siguiente página en la tabla.
        /// </summary>
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// Método asincrónico que ajusta la paginación cuando la lista de columnas cambia.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            
            if (columnas.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }

        private string sortColumn = nameof(HomologacionDto.NombreHomologado);
        private bool sortAscending = true;

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

            columnas = sortAscending
                ? columnas.OrderBy(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList()
                : columnas.OrderByDescending(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList();
        }

        private async Task ExportarExcel()
        {
            if (columnas == null || !columnas.Any())
            {
                return;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Configurar licencia para EPPlus

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Homologaciones");

            // Agregar encabezados
            worksheet.Cells[1, 1].Value = "Vista Código Homologado";
            worksheet.Cells[1, 2].Value = "Texto a Mostrar en la Web";
            worksheet.Cells[1, 3].Value = "Tooltip Web";
            worksheet.Cells[1, 4].Value = "Mostrar";
            worksheet.Cells[1, 5].Value = "Tipo de Dato";
            worksheet.Cells[1, 6].Value = "Si No Hay Dato";

            int row = 2;
            foreach (var context in columnas)
            {
                var homologacion = listaVwHomologacion?.FirstOrDefault(c => c.IdHomologacion == context.IdHomologacion);
                worksheet.Cells[row, 1].Value = homologacion?.NombreHomologado;
                worksheet.Cells[row, 2].Value = homologacion?.MostrarWeb;
                worksheet.Cells[row, 3].Value = homologacion?.TooltipWeb;
                worksheet.Cells[row, 4].Value = homologacion?.Mostrar;
                worksheet.Cells[row, 5].Value = homologacion?.MascaraDato;
                worksheet.Cells[row, 6].Value = homologacion?.SiNoHayDato;
                row++;
            }

            worksheet.Cells.AutoFitColumns(); // Ajustar automáticamente las columnas

            var fileName = "Homologaciones_Export.xlsx";
            var fileBytes = package.GetAsByteArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);

            await JSRuntime.InvokeVoidAsync("downloadExcel", fileName, fileBase64);
        }
        private async Task ExportarPDF()
        {
            if (columnas == null || !columnas.Any())
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            var document = new Document(iTextSharp.text.PageSize.A4);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var table = new PdfPTable(6) { WidthPercentage = 100 };

            table.AddCell(new Phrase("Vista Código Homologado", font));
            table.AddCell(new Phrase("Texto a Mostrar en la Web", font));
            table.AddCell(new Phrase("Tooltip Web", font));
            table.AddCell(new Phrase("Mostrar", font));
            table.AddCell(new Phrase("Tipo de Dato", font));
            table.AddCell(new Phrase("Si No Hay Dato", font));

            foreach (var context in columnas)
            {
                var homologacion = listaVwHomologacion?.FirstOrDefault(c => c.IdHomologacion == context.IdHomologacion);
                table.AddCell(homologacion?.NombreHomologado ?? "-");
                table.AddCell(homologacion?.MostrarWeb ?? "-");
                table.AddCell(homologacion?.TooltipWeb ?? "-");
                table.AddCell(homologacion?.Mostrar ?? "-");
                table.AddCell(homologacion?.MascaraDato ?? "-");
                table.AddCell(homologacion?.SiNoHayDato ?? "-");
            }

            document.Add(table);
            document.Close();

            var fileName = "Homologaciones_Export.pdf";
            var fileBase64 = Convert.ToBase64String(memoryStream.ToArray());

            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, "application/pdf", fileBase64);
        }
    }
    
}