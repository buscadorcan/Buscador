using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using SharedApp.Dtos;

namespace ClientApp.Pages.Administracion.Eventos
{
    public partial class Form_Graf_Event
    {

        [Parameter] public string? selectUser { get; set; }
        [Parameter] public DateOnly? dateStart { get; set; }
        [Parameter] public DateOnly? dateEnd { get; set; }

        [Inject] public IEventService? EventService { get; set; }
        [Inject] IJSRuntime JS { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        private enum reporteView
        {
            TiempoSession = 1,
            PaginasMasVisitadas = 2,
            FiltroMasUsado = 3
        }

        private bool isLoading = false;
        private reporteView selectView;

        private List<HeatmapPoint> heatmapData = new();
        private List<VwEventTrackingSessionDto> listasEventSession = new();
        private List<PaginasMasVisitadaDto> listasEventPagMasVist = new();
        private List<FiltrosMasUsadoDto> listasEventFiltrMasUsad = new();

        private bool IsModalOpen = false;
        private int ProgressValue { get; set; } = 0;
        private int PageSize = 5; // Cantidad de registros por página
        private int CurrentPage = 1;
        private IEnumerable<VwEventTrackingSessionDto> PaginatedItems => listasEventSession
               .Skip((CurrentPage - 1) * PageSize)
               .Take(PageSize);
        private int TotalPages => listasEventSession.Count > 0 ? (int)Math.Ceiling((double)listasEventSession.Count / PageSize) : 1;
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

        protected override void OnParametersSet()
        {
            Console.WriteLine($"Usuario seleccionado: {selectUser}");
            Console.WriteLine($"Fecha inicio: {dateStart}");
            Console.WriteLine($"Fecha fin: {dateEnd}");
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initMap");
            }
        }

        private async Task verReport(reporteView report)
        {
            isLoading = true;

            selectView = report;

            try
            {
                switch (report)
                {
                    case reporteView.TiempoSession:
                        listasEventSession = await EventService.GetEventSessionAsync();
                        break;
                    case reporteView.PaginasMasVisitadas:
                        listasEventPagMasVist = await EventService.GetEventPagMasVistAsync();
                        break;
                    case reporteView.FiltroMasUsado:
                        listasEventFiltrMasUsad = await EventService.GetEventFiltroMasUsadAsync();
                        break;
                }
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task verGrafica(reporteView report)
        {
            IsModalOpen = true;

            switch (report)
            {
                case reporteView.TiempoSession:
                    await CargarDatos();
                    break;
                case reporteView.PaginasMasVisitadas:
                    await CargarDatosPagMasVisit();
                    break;
                case reporteView.FiltroMasUsado:
                    await CargarDatosFiltrMasUsad();
                    break;
            }
            await Task.Delay(500);
            await JS.InvokeVoidAsync("invalidateSize");
        }

        private void CloseModal()
        {
            IsModalOpen = false;
        }

        public async Task CargarDatos()
        {

            if (listasEventSession != null)
            {

                heatmapData.Clear();
                var markers = new List<object>();

                var zoomLevel = await JS.InvokeAsync<int>("getMapZoom");

                foreach (var session in listasEventSession)
                {
                    if (session.Latitud != null && session.Longitud != null)
                    {

                        double zoomFactor = Math.Pow(2, (15 - zoomLevel));
                        double adjustedIntensity = session.TiempoDeConeccionEnMin / zoomFactor;

                        heatmapData.Add(new HeatmapPoint
                        {
                            Lat = session.Latitud,
                            Lng = session.Longitud,
                            Intensity = adjustedIntensity
                        });

                        var display = $"{session.CodigoHomologacionRol}, {session.TiempoDeConeccionEnMin}Min";
                        await JS.InvokeVoidAsync("addMarker", session.Latitud, session.Longitud, display);
                    }
                }

                await JS.InvokeVoidAsync("addHeatmapData", heatmapData);
            }
        }

        public async Task CargarDatosPagMasVisit()
        {

            if (listasEventPagMasVist != null)
            {
                foreach (var session in listasEventPagMasVist)
                {
                    if (session.Latitud != null && session.Longitud != null)
                    {
                        heatmapData.Add(new HeatmapPoint
                        {
                            Lat = session.Latitud,
                            Lng = session.Longitud,
                            Intensity = session.uso
                        });

                        var display = $"{session.CodigoHomologacionRol}, {session.uso}";

                        await JS.InvokeVoidAsync("addMarker", session.Latitud, session.Longitud, display);
                    }

                }
            }

            await JS.InvokeVoidAsync("addHeatmapData", heatmapData);
        }

        public async Task CargarDatosFiltrMasUsad()
        {

            if (listasEventFiltrMasUsad != null)
            {
                foreach (var session in listasEventFiltrMasUsad)
                {
                    if (session.Latitud != null && session.Longitud != null)
                    {
                        heatmapData.Add(new HeatmapPoint
                        {
                            Lat = session.Latitud,
                            Lng = session.Longitud,
                            Intensity = session.Uso
                        });

                        var display = $"{session.CodigoHomologacionRol}, {session.Uso}";

                        await JS.InvokeVoidAsync("addMarker", session.Latitud, session.Longitud, display);
                    }

                }
            }

            await JS.InvokeVoidAsync("addHeatmapData", heatmapData);
        }

        private void GoBack()
        {
            Navigation.NavigateTo("/eventos", forceLoad: false);
        }

        private async Task ExportarExcel(reporteView TypeReport)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Configurar licencia para EPPlus

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Eventos");

            // Agregar encabezados

            worksheet.Cells[1, 1].Value = "Codigo Homologación";
            worksheet.Cells[1, 2].Value = "Ip";

            switch (TypeReport)
            {
                case reporteView.TiempoSession:
                    worksheet.Cells[1, 3].Value = "Fecha";
                    worksheet.Cells[1, 4].Value = "Fecha Inicio";
                    worksheet.Cells[1, 5].Value = "Fecha Fin";
                    worksheet.Cells[1, 6].Value = "Tiempo de Conección";
                    break;
                case reporteView.PaginasMasVisitadas:
                    worksheet.Cells[1, 3].Value = "Nombre Control";
                    worksheet.Cells[1, 4].Value = "Uso";
                    break;
                case reporteView.FiltroMasUsado:
                    worksheet.Cells[1, 3].Value = "Tipo de Filtro";
                    worksheet.Cells[1, 4].Value = "Filtro";
                    worksheet.Cells[1, 5].Value = "Uso";
                    break;
            }

            int row = 2;

            switch (TypeReport)
            {
                case reporteView.TiempoSession:
                    foreach (var even in listasEventSession)
                    {
                        worksheet.Cells[row, 1].Value = even.CodigoHomologacionRol;
                        worksheet.Cells[row, 2].Value = even.IpDirec;
                        worksheet.Cells[row, 3].Value = even.Fecha;
                        worksheet.Cells[row, 4].Value = even.FechaInicio;
                        worksheet.Cells[row, 5].Value = even.FechaFin;
                        worksheet.Cells[row, 6].Value = even.TiempoDeConeccionEnMin;

                        row++;
                    }
                    break;
                case reporteView.PaginasMasVisitadas:
                    foreach (var even in listasEventPagMasVist)
                    {
                        worksheet.Cells[row, 1].Value = even.CodigoHomologacionRol;
                        worksheet.Cells[row, 2].Value = even.IpAddress;
                        worksheet.Cells[row, 3].Value = even.CodigoHomologacionMenu;
                        worksheet.Cells[row, 4].Value = even.uso;
                        row++;
                    }
                    break;
                case reporteView.FiltroMasUsado:
                    foreach (var even in listasEventFiltrMasUsad)
                    {
                        worksheet.Cells[row, 1].Value = even.CodigoHomologacionRol;
                        worksheet.Cells[row, 2].Value = even.IpAddress;
                        worksheet.Cells[row, 3].Value = even.FiltroTipo;
                        worksheet.Cells[row, 4].Value = even.FiltroValor;
                        worksheet.Cells[row, 5].Value = even.Uso;
                        row++;
                    }
                    break;
            }

            worksheet.Cells.AutoFitColumns(); // Ajustar automáticamente las columnas

            var fileName = "Eventos_Export.xlsx";
            var fileBytes = package.GetAsByteArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);

            await JSRuntime.InvokeVoidAsync("downloadExcel", fileName, fileBase64);
        }

        public class HeatmapPoint
        {
            public double? Lat { get; set; }
            public double? Lng { get; set; }
            public double Intensity { get; set; }
        }
    }
}
