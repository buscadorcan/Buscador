using BlazorBootstrap;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;
using System.Reflection.Metadata.Ecma335;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.JSInterop;
using OfficeOpenXml;

namespace ClientAppAdministrador.Pages.Administracion.MigracionExcel
{
    /// <summary>
    /// Componente de listado de logs de migraci�n de archivos Excel.
    /// Controla el acceso seg�n el rol del usuario y permite visualizar registros de migraci�n.
    /// </summary>
    public partial class Listado
    {
        /// <summary>
        /// Servicio de navegaci�n para redirigir a otras p�ginas.
        /// </summary>
        [Inject] public NavigationManager? navigationManager { get; set; }
        /// <summary>
        /// Servicio de almacenamiento local en el navegador.
        /// </summary>
        [Inject] ILocalStorageService iLocalStorageService { get; set; }
        /// <summary>
        /// Servicio de migraci�n de archivos Excel.
        /// </summary>
        [Inject] private IMigracionExcelService? iMigracionExcelService { get; set; }
        /// <summary>
        /// Servicio de logs de migraci�n.
        /// </summary>
        [Inject] private ILogMigracionService? iLogMigracionService { get; set; }
        // Componente de la grilla para mostrar los registros de migraci�n
        private Grid<LogMigracionDto>? grid;
        // Variables de control de acceso seg�n el rol del usuario
        private bool accessMigration;
        private bool isRolRead;
        private bool isRolOna;
        private bool isRolAdmin;
        /// <summary>
        /// Servicio de b�squeda y registro de eventos.
        /// </summary>
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();
        // Lista que almacena los registros de logs de migraci�n
        private List<LogMigracionDto> listasHevd = new();
        // Par�metros para la paginaci�n
        private int PageSize = 10; // Cantidad de registros por p�gina
        private int CurrentPage = 1;

        /// <summary>
        /// Obtiene los elementos paginados para la grilla.
        /// </summary>
        private IEnumerable<LogMigracionDto> PaginatedItems => listasHevd
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        /// <summary>
        /// Calcula el n�mero total de p�ginas basado en el n�mero de registros.
        /// </summary>
        private int TotalPages => listasHevd.Count > 0 ? (int)Math.Ceiling((double)listasHevd.Count / PageSize) : 1;

        /// <summary>
        /// Indica si se puede retroceder a la p�gina anterior.
        /// </summary>
        private bool CanGoPrevious => CurrentPage > 1;
        /// <summary>
        /// Indica si se puede avanzar a la siguiente p�gina.
        /// </summary>
        private bool CanGoNext => CurrentPage < TotalPages;


        /// <summary>
        /// Cambia a la p�gina anterior en la paginaci�n.
        /// </summary>
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        /// <summary>
        /// Cambia a la siguiente p�gina en la paginaci�n.
        /// </summary>
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// M�todo asincr�nico que se ejecuta al inicializar el componente.
        /// Carga la lista de logs de migraci�n y controla el acceso seg�n el rol del usuario.
        /// </summary>

        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/migracion-excel";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "migracion-excel";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            var usuarioBaseDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_BaseDatos_Local);
            var usuarioOrigenDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_OrigenDatos_Local);
            var usuarioEstadoMigracion = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_EstadoMigracion_Local);
            var usuarioMigrar = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Migrar_Local);
            var rolRelacionado = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);

            isRolRead = rolRelacionado == "KEY_USER_READ";
            isRolOna = rolRelacionado == "KEY_USER_ONA";
            isRolAdmin = rolRelacionado == "KEY_USER_CAN";

            // Verificaci�n de acceso
            if (!isRolAdmin && !isRolOna)
            {
                if (!isRolRead)
                {
                    if (usuarioMigrar != "S" ||
                        usuarioEstadoMigracion != "A" ||
                        (usuarioBaseDatos != "INACAL" && usuarioBaseDatos != "DTA") ||
                        usuarioOrigenDatos != "EXCEL")
                    {
                        navigationManager?.NavigateTo("/page-nodisponible");
                        return;
                    }
                }
                else
                {
                    navigationManager?.NavigateTo("/page-nodisponible");
                    return;
                }
            }

            // Carga de datos con validaci�n
            if (iLogMigracionService != null)
            {
                listasHevd = await iLogMigracionService.GetLogMigracionesAsync() ?? new List<LogMigracionDto>();
            }

            // Ajusta la paginaci�n si la lista est� vac�a o cambia
            if (listasHevd.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }

        private string sortColumn = nameof(LogMigracionDto.IdLogMigracion);
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

            listasHevd = sortAscending
                ? listasHevd.OrderBy(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList()
                : listasHevd.OrderByDescending(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList();
        }

        private async Task ExportarExcel()
        {
            objEventTracking.CodigoHomologacionMenu = "/migracion-excel";
            objEventTracking.NombreAccion = "ExportarExcel";
            objEventTracking.NombreControl = "btnExportarExcel";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listasHevd == null || !listasHevd.Any())
            {
                return;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Configurar licencia para EPPlus

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Migraciones");

            // Agregar encabezados
            worksheet.Cells[1, 1].Value = "Migraci�n";
            worksheet.Cells[1, 2].Value = "EsquemaVista";
            worksheet.Cells[1, 3].Value = "Estado";
            worksheet.Cells[1, 4].Value = "Inicio Migraci�n";
            worksheet.Cells[1, 5].Value = "Final Migraci�n";
            worksheet.Cells[1, 6].Value = "Nombre archivo";
            worksheet.Cells[1, 7].Value = "Error";

            int row = 2;
            foreach (var item in listasHevd)
            {
                worksheet.Cells[row, 1].Value = item.IdLogMigracion;
                worksheet.Cells[row, 2].Value = item.EsquemaVista;
                worksheet.Cells[row, 3].Value = item.Estado;
                worksheet.Cells[row, 4].Value = item.Inicio;
                worksheet.Cells[row, 5].Value = item.Final;
                worksheet.Cells[row, 6].Value = item.ExcelFileName;
                worksheet.Cells[row, 7].Value = item.Observacion;
                row++;
            }

            worksheet.Cells.AutoFitColumns(); // Ajustar autom�ticamente las columnas

            var fileName = "Migraciones_Export.xlsx";
            var fileBytes = package.GetAsByteArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);

            await JSRuntime.InvokeVoidAsync("downloadExcel", fileName, fileBase64);
        }
        private async Task ExportarPDF()
        {
            objEventTracking.CodigoHomologacionMenu = "/migracion-excel";
            objEventTracking.NombreAccion = "ExportarPDF";
            objEventTracking.NombreControl = "btnExportarPDF";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listasHevd == null || !listasHevd.Any())
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            var document = new Document(iTextSharp.text.PageSize.A4.Rotate()); // Documento en horizontal
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var table = new PdfPTable(7) { WidthPercentage = 100 }; // Cambi� a 7 columnas

            // Encabezados
            table.AddCell(new Phrase("Migraci�n", font));
            table.AddCell(new Phrase("EsquemaVista", font));
            table.AddCell(new Phrase("Estado", font));
            table.AddCell(new Phrase("Inicio Migraci�n", font));
            table.AddCell(new Phrase("Final Migraci�n", font));
            table.AddCell(new Phrase("Nombre archivo", font));
            table.AddCell(new Phrase("Error", font));

            // Ajustar el ancho de las columnas
            float[] widths = new float[] { 15f, 15f, 15f, 15f, 15f, 20f, 30f };
            table.SetWidths(widths);

            // Llenar la tabla con los datos
            foreach (var item in listasHevd)
            {
                table.AddCell(new Phrase(item.IdLogMigracion.ToString()));
                table.AddCell(new Phrase(item.EsquemaVista));
                table.AddCell(new Phrase(item.Estado));
                table.AddCell(new Phrase(item.Inicio.ToString()));
                table.AddCell(new Phrase(item.Final.ToString()));
                table.AddCell(new Phrase(item.ExcelFileName));
                table.AddCell(new Phrase(item.Observacion ?? "-"));
            }

            document.Add(table);
            document.Close();

            var fileName = "Migraciones_Export.pdf";
            var fileBase64 = Convert.ToBase64String(memoryStream.ToArray());

            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, "application/pdf", fileBase64);
        }
    }
}