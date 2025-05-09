using BlazorBootstrap;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using ClientAppAdministrador.Pages.Administracion.Esquemas;
using Infractruture.Services;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.JSInterop;
using OfficeOpenXml;

namespace ClientAppAdministrador.Pages.Administracion.Usuarios
{
    public partial class Listado
    {
        private List<UsuarioDto>? listaUsuarios;
        private bool isRolRead; // Variable para controlar la visibilidad del botón
        private bool isRol16;
        private bool showModal; // Controlar la visibilidad de la ventana modal  
        private string modalMessage;
        private int rolCargo;
        private int onaPais;
        [Inject]
        IUsuariosService? iUsuariosService { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        [Inject]
        private NavigationManager iNavigationManager { get; set; }

        private List<VwRolDto>? listaRoles;
        private List<OnaDto>? listaOna;

        private Button saveButton = default!;
        private Grid<UsuarioDto>? grid;
        private int? selectedIdUsuario;    // Almacena el ID del usuario seleccionado
        [Inject]
        public Infractruture.Services.ToastService? toastService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        private IEnumerable<UsuarioDto> PaginatedItems => listaUsuarios
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => listaUsuarios.Count > 0 ? (int)Math.Ceiling((double)listaUsuarios.Count / PageSize) : 1;

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
            objEventTracking.CodigoHomologacionMenu = "/usuarios";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "usuarios";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local); objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            listaUsuarios = await iUsuariosService.GetUsuariosAsync();
            listaOna = await iUsuariosService.GetOnaAsync();

            var rolRelacionado = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            var onaRelacionado = listaOna.FirstOrDefault(ona => ona.IdONA == onaPais);
 
            isRolRead = rolRelacionado == "KEY_USER_READ";

            // Ajusta la paginación si la lista está vacía o cambia
            if (listaUsuarios.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }
        private async void EditarUsuario(UsuarioDto usuario)
        {

            onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            listaUsuarios = await iUsuariosService.GetUsuariosAsync();

            listaRoles = await iUsuariosService.GetRolesAsync();
            listaOna = await iUsuariosService.GetOnaAsync();

            var rolRelacionado = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            var onaRelacionado = listaOna.FirstOrDefault(ona => ona.IdONA == onaPais);
            var Homolog = listaUsuarios.FirstOrDefault(usu => usu.IdUsuario == usuario.IdUsuario);
            var rolUsuario = listaRoles.FirstOrDefault(rol => rol.IdHomologacionRol == Homolog.IdHomologacionRol);

            if (rolRelacionado == "KEY_USER_ONA" && rolUsuario.CodigoHomologacion == "KEY_USER_CAN")
            {
                // No tiene permisos, mostrar la modal
                modalMessage = "No tiene permisos para editar este usuario.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            if (usuario.IdONA != onaPais && rolRelacionado == "KEY_USER_ONA")
            {
                modalMessage = "No tiene permisos para editar este usuario porque no pertenece a este País.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            if (usuario.IdONA == onaPais && rolRelacionado == "KEY_USER_CAN")
            {
                modalMessage = "No tiene permisos para editar este usuario porque no pertenece a este País.";
                showModal = true;
                StateHasChanged(); // Forzar la actualización de la interfaz
            }

            if (rolRelacionado == "KEY_USER_ONA"   && usuario.IdONA == onaPais && rolUsuario.CodigoHomologacion != "KEY_USER_CAN")
            {
                // Navegar al editar usuario
                iNavigationManager.NavigateTo($"/editar-usuario/{usuario.IdUsuario}");
            }

            if (rolRelacionado == "KEY_USER_CAN")
            {
                // Navegar al editar usuario
                iNavigationManager.NavigateTo($"/editar-usuario/{usuario.IdUsuario}");
            }
        }
        private void CerrarModal()
        {
            showModal = false;
        }
        private async Task<GridDataProviderResult<UsuarioDto>> UsuariosDataProvider(GridDataProviderRequest<UsuarioDto> request)
        {
            if (listaUsuarios is null && iUsuariosService != null)
            {
                listaUsuarios = await iUsuariosService.GetUsuariosAsync();
            }

            return await Task.FromResult(request.ApplyTo(listaUsuarios ?? new List<UsuarioDto>()));
        }
        // Abre el modal
        private void OpenDeleteModal(int idUsuario)
        {
            selectedIdUsuario = idUsuario;
            showModal = true;
        }

        // Cierra el modal
        private void CloseModal()
        {
            selectedIdUsuario = null;
            showModal = false;
        }

        // Confirmar eliminación del registro
        private async Task ConfirmDelete()
        {
            objEventTracking.CodigoHomologacionMenu = "/usuarios";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "btnEliminar";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);
            if (selectedIdUsuario.HasValue && iUsuariosService != null)
            {
                var result = await iUsuariosService.DeleteUsuarioAsync(selectedIdUsuario.Value);
                if (result)
                {
                    CloseModal(); // Cierra el modal
                    toastService?.CreateToastMessage(ToastType.Success, "Registro eliminado exitosamente.");
                    iNavigationManager?.NavigateTo("/usuarios");
                    await LoadUsuarios(); // Actualiza la lista
                    //await grid?.RefreshDataAsync(); //resfresca la grilla
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar el registro.");
                    iNavigationManager?.NavigateTo("/usuarios");
                }
            }

        }
        // Método para cargar la lista de Usuarios
        private async Task LoadUsuarios()
        {
            if (iUsuariosService != null)
            {
                listaUsuarios = await iUsuariosService.GetUsuariosAsync();
            }
        }

        private string sortColumn = nameof(UsuarioDto.Nombre);
        private bool sortAscending = true;

        private void OrdenarPor(string columnName)
        {
            if (sortColumn == columnName)
            {
                sortAscending = !sortAscending; // Invierte el orden si es la misma columna
            }
            else
            {
                sortColumn = columnName;
                sortAscending = true;
            }

            // Ordenar la lista
            listaUsuarios = sortAscending
                ? listaUsuarios.OrderBy(u => typeof(UsuarioDto).GetProperty(sortColumn)?.GetValue(u, null)).ToList()
                : listaUsuarios.OrderByDescending(u => typeof(UsuarioDto).GetProperty(sortColumn)?.GetValue(u, null)).ToList();
        }
        private async Task ExportarExcel()
        {
            objEventTracking.CodigoHomologacionMenu = "/usuarios";
            objEventTracking.NombreAccion = "ExportarExcel";
            objEventTracking.NombreControl = "btnExportarExcel";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listaUsuarios == null || !listaUsuarios.Any())
            {
                return;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Configurar licencia para EPPlus

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Usuarios");

            // Agregar encabezados
            worksheet.Cells[1, 1].Value = "Nombre";
            worksheet.Cells[1, 2].Value = "Apellido";
            worksheet.Cells[1, 3].Value = "Teléfono";
            worksheet.Cells[1, 4].Value = "Email";
            worksheet.Cells[1, 5].Value = "Rol";
            worksheet.Cells[1, 6].Value = "Razón Social";
            worksheet.Cells[1, 7].Value = "Estado";

            int row = 2;
            foreach (var usuario in listaUsuarios)
            {
                worksheet.Cells[row, 1].Value = usuario.Nombre;
                worksheet.Cells[row, 2].Value = usuario.Apellido;
                worksheet.Cells[row, 3].Value = usuario.Telefono;
                worksheet.Cells[row, 4].Value = usuario.Email;
                worksheet.Cells[row, 5].Value = usuario.Rol;
                worksheet.Cells[row, 6].Value = usuario.RazonSocial;
                worksheet.Cells[row, 7].Value = usuario.Estado == "A" ? "Activo" : "Inactivo";
                row++;
            }

            worksheet.Cells.AutoFitColumns(); // Ajustar automáticamente las columnas

            var fileName = "Usuarios_Export.xlsx";
            var fileBytes = package.GetAsByteArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);

            await JSRuntime.InvokeVoidAsync("downloadExcel", fileName, fileBase64);
        }
        private async Task ExportarPDF()
        {
            objEventTracking.CodigoHomologacionMenu = "/usuarios";
            objEventTracking.NombreAccion = "ExportarPDF";
            objEventTracking.NombreControl = "btnExportarPDF";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listaUsuarios == null || !listaUsuarios.Any())
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            var document = new Document(iTextSharp.text.PageSize.A4);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var table = new PdfPTable(7) { WidthPercentage = 100 };

            table.AddCell(new Phrase("Nombre", font));
            table.AddCell(new Phrase("Apellido", font));
            table.AddCell(new Phrase("Teléfono", font));
            table.AddCell(new Phrase("Email", font));
            table.AddCell(new Phrase("Rol", font));
            table.AddCell(new Phrase("Razón Social", font));
            table.AddCell(new Phrase("Estado", font));

            foreach (var usuario in listaUsuarios)
            {
                table.AddCell(usuario.Nombre);
                table.AddCell(usuario.Apellido);
                table.AddCell(usuario.Telefono);
                table.AddCell(usuario.Email);
                table.AddCell(usuario.Rol);
                table.AddCell(usuario.RazonSocial);
                table.AddCell(usuario.Estado == "A" ? "Activo" : "Inactivo");
            }

            document.Add(table);
            document.Close();

            var fileName = "Usuarios_Export.pdf";
            var fileBase64 = Convert.ToBase64String(memoryStream.ToArray());

            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, "application/pdf", fileBase64);
        }
    }
}
