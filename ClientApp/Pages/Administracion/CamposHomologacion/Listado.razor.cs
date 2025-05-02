using BlazorBootstrap;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Dtos;
using iTextSharp.text.pdf;
using iTextSharp.text;
using OfficeOpenXml;

namespace ClientApp.Pages.Administracion.CamposHomologacion
{
    public partial class Listado
    {
        // Elemento seleccionado en la lista de homologaciones
        private HomologacionDto? homologacionSelected;
        // Componente de la grilla para mostrar los datos
        private Grid<HomologacionDto>? grid;
        // Lista de homologaciones obtenidas
        private List<HomologacionDto>? listaHomologacions = new List<HomologacionDto>();
        // Servicio inyectado para acceder a los catálogos
        [Inject]
        private ICatalogosService? iCatalogosService { get; set; }
        // Servicio inyectado para gestionar homologaciones
        [Inject]
        private IHomologacionService? iHomologacionService { get; set; }
        // Lista de visualización de homologaciones
        private List<HomologacionDto>? listaVwHomologacion;
        // Evento que se activa cuando los datos se han cargado
        public event Action? DataLoaded;
        // ID de la homologación seleccionada para operaciones
        private int? selectedIdHomologacion;    // Almacena el ID de la homologación seleccionado
        // Control de visibilidad del modal
        private bool showModal; // Controlar la visibilidad de la ventana modal  
        // Bandera para determinar si se está agregando un nuevo elemento
        private bool IsAdd;
        // Mensaje del modal
        private string modalMessage;
        // Servicio de notificaciones Toast
        [Inject]
        public Infractruture.Services.ToastService? toastService { get; set; }
        // Servicio de interoperabilidad con JavaScript
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        // Servicio de búsqueda
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();
        // Servicio de almacenamiento local en el navegador
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }

        /// <summary>
        /// Método asincrónico que inicializa la lista de campos de homologación.
        /// </summary>
        /// <returns>Devuelve la lista de homologaciones al iniciar la aplicación.</returns>
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/campos-homologacion";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "campos-homologacion";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (iCatalogosService != null)
            {
                listaVwHomologacion = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("grupos");

                DataLoaded += async () => {
                    if (!(listaHomologacions is null) && JSRuntime != null) {
                        await Task.Delay(2000);
                        await JSRuntime.InvokeVoidAsync("initSortable", DotNetObjectReference.Create(this));
                    }
                };
            }
        }

        /// <summary>
        /// Método que obtiene la lista de homologaciones y aplica los criterios de filtrado.
        /// </summary>
        /// <returns>Lista de homologaciones aplicando los filtros.</returns>
        private async Task<GridDataProviderResult<HomologacionDto>> HomologacionDataProvider(GridDataProviderRequest<HomologacionDto> request)
        {
            if (homologacionSelected != null)
            {
                IsAdd = homologacionSelected.CodigoHomologacion == "KEY_DIM_ESQUEMA";
                //listaHomologacions = await iHomologacionService.GetHomologacionsAsync();
                listaHomologacions = await iHomologacionService.GetHomologacionsSelectAsync(homologacionSelected.CodigoHomologacion);
            }
            //IsAdd = true;
            DataLoaded?.Invoke();

            return await Task.FromResult(request.ApplyTo(listaHomologacions ?? []));
        }

        /// <summary>
        /// Método que maneja el cambio de selección en un elemento de autocompletado.
        /// </summary>
        /// <param name="e">Evento de cambio.</param>
        private async Task OnAutoCompleteChangedHandler(ChangeEventArgs e)
        {
            
            // Obtén el ID seleccionado desde el <select>
            var selectedId = Convert.ToInt32(e.Value);

            // Busca el elemento correspondiente en la lista
            var selectedHomologacion = listaVwHomologacion?.FirstOrDefault(h => h.IdHomologacion == selectedId);

            // Si se encuentra, actualiza la selección
            if (selectedHomologacion != null)
            {
                homologacionSelected = selectedHomologacion;
                IsAdd = homologacionSelected.CodigoHomologacion == "KEY_DIM_ESQUEMA";
                // Refresca la grilla si es necesario
                if (grid != null)
                {
                    await grid.RefreshDataAsync();
                }
            }
        }

        /// <summary>
        /// Método invocable desde JavaScript para actualizar el orden de los elementos en la lista al arrastrar.
        /// </summary>
        /// <param name="sortedIds">Lista de identificadores ordenados.</param>

        [JSInvokable]
        public async Task OnDragEnd(string[] sortedIds)
        {
            for (int i = 0; i < sortedIds.Length; i += 1)
            {
                HomologacionDto homo = listaHomologacions.FirstOrDefault(h => h.IdHomologacion == int.Parse(sortedIds[i]));
                if (homo != null && homo.MostrarWebOrden != i + 1)
                {
                    homo.MostrarWebOrden = i + 1;
                    await iHomologacionService.RegistrarOActualizar(homo);
                }
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Método que abre el modal de confirmación de eliminación.
        /// </summary>
        /// <param name="idHomologacion">ID de la homologación a eliminar.</param>

        private void OpenDeleteModal(int idHomologacion)
        {
            selectedIdHomologacion = idHomologacion;
            showModal = true;
        }

        /// <summary>
        /// Método que cierra el modal de confirmación de eliminación.
        /// </summary>
        private void CloseModal()
        {
            selectedIdHomologacion = null;
            showModal = false;
        }

        /// <summary>
        /// Método que confirma la eliminación de una homologación.
        /// </summary>
        private async Task ConfirmDelete()
        {
            objEventTracking.CodigoHomologacionMenu = "/campos-homologacion";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "btnEliminar";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (selectedIdHomologacion.HasValue && iHomologacionService != null)
            {
                var result = await iHomologacionService.DeleteHomologacionAsync(selectedIdHomologacion.Value);
                if (result)
                {
                    CloseModal(); // Cierra el modal
                    toastService?.CreateToastMessage(ToastType.Success, "Registro eliminado exitosamente.");
                    await LoadHomologacion(); // Actualiza la lista
                    await grid?.RefreshDataAsync(); //resfresca la grilla
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar el registro.");
                }
            }

        }

        /// <summary>
        /// Método que carga la lista de homologaciones.
        /// </summary>
        private async Task LoadHomologacion() 
        {
            if (iHomologacionService != null)
            {
                listaHomologacions = await iHomologacionService.GetHomologacionsAsync();
            }
        }

        private async Task ExportarExcel()
        {
            objEventTracking.CodigoHomologacionMenu = "/campos-homologacion";
            objEventTracking.NombreAccion = "ExportarExcel";
            objEventTracking.NombreControl = "btnExportarExcel";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listaHomologacions == null || !listaHomologacions.Any())
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
            worksheet.Cells[1, 4].Value = "Tipo de Dato";
            worksheet.Cells[1, 5].Value = "Si No Hay Dato";

            int row = 2;
            foreach (var homologacion in listaHomologacions)
            {
                worksheet.Cells[row, 1].Value = homologacion.NombreHomologado;
                worksheet.Cells[row, 2].Value = homologacion.MostrarWeb;
                worksheet.Cells[row, 3].Value = homologacion.TooltipWeb;
                worksheet.Cells[row, 4].Value = homologacion.MascaraDato;
                worksheet.Cells[row, 5].Value = homologacion.SiNoHayDato;
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
            objEventTracking.CodigoHomologacionMenu = "/campos-homologacion";
            objEventTracking.NombreAccion = "ExportarPDF";
            objEventTracking.NombreControl = "btnExportarPDF";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listaHomologacions == null || !listaHomologacions.Any())
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            var document = new Document(iTextSharp.text.PageSize.A4);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var table = new PdfPTable(5) { WidthPercentage = 100 };

            table.AddCell(new Phrase("Vista Código Homologado", font));
            table.AddCell(new Phrase("Texto a Mostrar en la Web", font));
            table.AddCell(new Phrase("Tooltip Web", font));
            table.AddCell(new Phrase("Tipo de Dato", font));
            table.AddCell(new Phrase("Si No Hay Dato", font));

            foreach (var homologacion in listaHomologacions)
            {
                table.AddCell(homologacion.NombreHomologado ?? "-");
                table.AddCell(homologacion.MostrarWeb ?? "-");
                table.AddCell(homologacion.TooltipWeb ?? "-");
                table.AddCell(homologacion.MascaraDato ?? "-");
                table.AddCell(homologacion.SiNoHayDato ?? "-");
            }

            document.Add(table);
            document.Close();

            var fileName = "Homologaciones_Export.pdf";
            var fileBase64 = Convert.ToBase64String(memoryStream.ToArray());

            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, "application/pdf", fileBase64);
        }
    }
}