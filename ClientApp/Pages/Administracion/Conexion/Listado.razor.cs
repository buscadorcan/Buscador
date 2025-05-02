using BlazorBootstrap;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.JSInterop;
using OfficeOpenXml;

namespace ClientApp.Pages.Administracion.Conexion
{

    public partial class Listado
    {
        ToastsPlacement toastsPlacement = ToastsPlacement.TopRight;
        private bool showModal; // Controlar la visibilidad de la ventana modal  
        private string modalMessage;
        private int? selectedIdOna;
        [Inject]
        public Infractruture.Services.ToastService? toastService { get; set; }
        List<ToastMessage> messages = new();

        [Inject]
        private IConexionService? iConexionService { get; set; }
        [Inject]
        private IDynamicService? iDynamicService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }

        private List<ONAConexionDto> listasHevd = new();
        private bool isRolAdmin;

        private EventTrackingDto objEventTracking { get; set; } = new();
        private bool IsLoading { get; set; } = false;
        private int ProgressValue { get; set; } = 0;
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        private IEnumerable<ONAConexionDto> PaginatedItems => listasHevd
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => listasHevd.Count > 0 ? (int)Math.Ceiling((double)listasHevd.Count / PageSize) : 1;

        private bool CanGoPrevious => CurrentPage > 1;
        private bool CanGoNext => CurrentPage < TotalPages;


        private string sortColumn = nameof(ONAConexionDto.Host);
        private bool sortAscending = true;

        /// <summary>
        /// PreviousPage: Previo de las paginas del listado.
        /// </summary>
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;

                objEventTracking.CodigoHomologacionMenu = "/conexion";
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";

                iBusquedaService.AddEventTrackingAsync(objEventTracking);
            }
        }

        /// <summary>
        /// NextPage: Proximas paginas del listado.
        /// </summary>
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// OnInitializedAsync: Iniciado del listado, carga del rol relacionado y de conexiones.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/conexion";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "conexion";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listasHevd != null && iConexionService != null)
            {
                var rolRelacionado = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                isRolAdmin = rolRelacionado == "KEY_USER_CAN";
                if (isRolAdmin)
                {
                    listasHevd = await iConexionService.GetConexionsAsync() ?? new List<ONAConexionDto>();
                }
                else
                {
                    int IdOna = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
                    listasHevd = await iConexionService.GetOnaConexionByOnaListAsync(IdOna) ?? new List<ONAConexionDto>();
                }
            }
            // Ajusta la paginación si la lista está vacía o cambia
            if (listasHevd.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }

        /// <summary>
        /// OnTestconexionClick: Test de la conexión externa, comprobando si la conexion esta en linea.
        /// </summary>
        /// <param name="conexion">
        /// <returns cref="Task"> devuelve un valor true o false dependiendo de la conexion</returns>
        private async Task<bool> OnTestconexionClick(int conexion)
        {
            try
            {
                objEventTracking.CodigoHomologacionMenu = "/conexion";
                objEventTracking.NombreAccion = "OnTestconexionClick";
                objEventTracking.NombreControl = "OnTestconexionClick";
                objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                if (iDynamicService != null && listasHevd != null)
                {
                    IsLoading = true;
                    ProgressValue = 0;
                    StateHasChanged();

                    try
                    {
                        // 🔹 Iniciar la prueba de conexión en un Task separado
                        var connectionTask = iDynamicService.TestConnectionAsync(conexion);

                        // 🔥 Simular el progreso en intervalos de 500ms, pero limitándolo a 95% antes de que termine la conexión
                        while (!connectionTask.IsCompleted)
                        {
                            await Task.Delay(500); // Espera 500ms antes de aumentar
                            if (ProgressValue < 95)
                            {
                                ProgressValue += 5; // Aumenta en 5% cada 500ms hasta 95%
                                StateHasChanged();
                            }
                        }

                        // 🔥 Esperar el resultado de la prueba de conexión
                        bool isConnected = await connectionTask;

                        // 🔹 Asegurar que la barra llegue al 100% solo cuando la prueba de conexión termine
                        ProgressValue = 100;
                        StateHasChanged();

                        var toastMessage = new ToastMessage
                        {
                            Type = isConnected ? ToastType.Success : ToastType.Danger,
                            Title = "Mensaje de confirmación",
                            HelpText = $"{DateTime.Now}",
                            Message = isConnected ? "Conexión satisfactoria" : "Conexión fallida",
                        };

                        messages.Add(toastMessage);
                        StateHasChanged();

                        // Mantener el mensaje visible por más tiempo (5 segundos)
                        await Task.Delay(5000);

                        // Remover mensaje después de la espera
                        messages.Remove(toastMessage);
                        InvokeAsync(StateHasChanged);

                        return isConnected;
                    }
                    finally
                    {
                        IsLoading = false;
                        ProgressValue = 0;
                        StateHasChanged();
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en OnTestconexionClick: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// OnMigrarClick: Migrar los datos de la ONA desde el servidor externo.
        /// </summary>
        /// <param name="conexion">
        /// <returns> devuelve un valor true o false dependiendo de la migracion</returns>
        private async Task<bool> OnMigrarClick(int conexion)
        {
            objEventTracking.CodigoHomologacionMenu = "/conexion";
            objEventTracking.NombreAccion = "OnMigrarClick";
            objEventTracking.NombreControl = "OnMigrarClick";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (iDynamicService != null && listasHevd != null)
            {
                IsLoading = true;
                ProgressValue = 0;
                StateHasChanged();

                try
                {
                    // 🔹 Iniciar la migración en un Task separado para permitir la actualización de la UI
                    var migrationTask = iDynamicService.MigrarConexionAsync(conexion);

                    // 🔥 Simular el progreso en intervalos de 500ms, pero limitándolo a 95% antes de que termine la migración
                    while (!migrationTask.IsCompleted)
                    {
                        await Task.Delay(500); // Espera 500ms antes de aumentar
                        if (ProgressValue < 95)
                        {
                            ProgressValue += 5; // Aumenta en 5% cada 500ms hasta 95%
                            StateHasChanged();
                        }
                    }

                    // 🔥 Esperar el resultado de la migración
                    bool migracion = await migrationTask;

                    // 🔹 Asegurar que la barra llegue al 100% solo cuando termine la migración
                    ProgressValue = 100;
                    StateHasChanged();

                    var toastMessage = new ToastMessage
                    {
                        Type = migracion ? ToastType.Success : ToastType.Danger,
                        Title = "Mensaje de confirmación",
                        HelpText = $"{DateTime.Now}",
                        Message = migracion ? "Migración satisfactoria" : "Migración no realizada",
                    };

                    messages.Add(toastMessage);
                    StateHasChanged();

                    // Mantener el mensaje visible por más tiempo (7 segundos)
                    await Task.Delay(7000);

                    // Remover mensaje después de la espera
                    messages.Remove(toastMessage);
                    InvokeAsync(StateHasChanged);

                    return migracion;
                }
                finally
                {
                    IsLoading = false;
                    ProgressValue = 0;
                    StateHasChanged();
                }
            }
            return false;
        }



        /// <summary>
        /// OpenDeleteModal: Abre el modal.
        /// </summary>
        private void OpenDeleteModal(int idOna)
        {
            selectedIdOna = idOna;
            showModal = true;
        }

        /// <summary>
        /// CloseModal: Cerrar el modal.
        /// </summary>
        private void CloseModal()
        {
            selectedIdOna = null;
            showModal = false;
        }

        /// <summary>
        /// ConfirmDelete: Elimina la conexion externa de la organizacion.
        /// </summary>
        private async Task ConfirmDelete()
        {
            objEventTracking.CodigoHomologacionMenu = "/conexion";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "btnEliminar";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (selectedIdOna.HasValue && iConexionService != null)
            {
                int idOna = selectedIdOna.Value;
                var respuesta = await iConexionService.EliminarConexion(selectedIdOna.Value);
                if (respuesta.registroCorrecto)
                {
                    CloseModal();
                    listasHevd = listasHevd.Where(c => c.IdONA != idOna).ToList();
                    await OnInitializedAsync();
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar el registro.");
                }
            }

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

            listasHevd = sortAscending
                ? listasHevd.OrderBy(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList()
                : listasHevd.OrderByDescending(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList();
        }

        private async Task ExportarExcel()
        {
            objEventTracking.CodigoHomologacionMenu = "/conexion";
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
            var worksheet = package.Workbook.Worksheets.Add("Conexiones");

            // Agregar encabezados
            worksheet.Cells[1, 1].Value = "Host";
            worksheet.Cells[1, 2].Value = "Puerto";
            worksheet.Cells[1, 3].Value = "Usuario";
            worksheet.Cells[1, 4].Value = "Base Datos";
            worksheet.Cells[1, 5].Value = "Origen Datos";
            worksheet.Cells[1, 6].Value = "Migrar";
            worksheet.Cells[1, 7].Value = "Estado";

            int row = 2;
            foreach (var conexion in listasHevd)
            {
                worksheet.Cells[row, 1].Value = conexion.Host;
                worksheet.Cells[row, 2].Value = conexion.Puerto;
                worksheet.Cells[row, 3].Value = conexion.Usuario;
                worksheet.Cells[row, 4].Value = conexion.BaseDatos;
                worksheet.Cells[row, 5].Value = conexion.OrigenDatos;
                worksheet.Cells[row, 6].Value = conexion.Migrar == "S" ? "Sí" : "No";
                worksheet.Cells[row, 7].Value = conexion.Estado == "A" ? "Activo" : "Inactivo";
                row++;
            }

            worksheet.Cells.AutoFitColumns(); // Ajustar automáticamente las columnas

            var fileName = "Conexiones_Export.xlsx";
            var fileBytes = package.GetAsByteArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);

            await JSRuntime.InvokeVoidAsync("downloadExcel", fileName, fileBase64);
        }
        private async Task ExportarPDF()
        {
            objEventTracking.CodigoHomologacionMenu = "/conexion";
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
            var document = new Document(iTextSharp.text.PageSize.A4);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var table = new PdfPTable(7) { WidthPercentage = 100 };

            foreach (var header in new[] { "Host", "Puerto", "Usuario", "Base Datos", "Origen Datos", "Migrar", "Estado" })
            {
                table.AddCell(new Phrase(header, font));
            }

            foreach (var conexion in listasHevd)
            {
                table.AddCell(conexion.Host ?? "-");
                table.AddCell(Convert.ToString(conexion.Puerto) ?? "-");
                table.AddCell(conexion.Usuario ?? "-");
                table.AddCell(conexion.BaseDatos ?? "-");
                table.AddCell(conexion.OrigenDatos ?? "-");
                table.AddCell(conexion.Migrar == "S" ? "Sí" : "No");
                table.AddCell(conexion.Estado == "A" ? "Activo" : "Inactivo");
            }

            document.Add(table);
            document.Close();

            var fileName = "Conexiones_Export.pdf";
            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, "application/pdf", Convert.ToBase64String(memoryStream.ToArray()));
        }

    }
}