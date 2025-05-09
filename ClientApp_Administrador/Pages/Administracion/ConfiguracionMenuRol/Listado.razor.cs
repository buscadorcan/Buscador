using BlazorBootstrap;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Dtos;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using iTextSharp.text;


namespace ClientAppAdministrador.Pages.Administracion.ConfiguracionMenuRol
{
    public partial class Listado
    {
        private bool showModal = false;
        private int? selectedIdHRol;
        private int? selectedIdHMenu;
        private List<MenuRolDto>? listaMenus;
        private Button saveButton = default!;

        [Inject]
        public IMenuService? iMenuService { get; set; }

        [Inject]
        public Infractruture.Services.ToastService? toastService { get; set; }

        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        private List<MenuRolDto> listaMenusOriginal = new();
        private bool estadoActivo;
        private MenuRolDto configuracionMenu = new MenuRolDto();

        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/menu-config-lista";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "menu-config-lista";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Local) + ' ' +
                                              await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Apellido_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            await LoadMenus();
            listaMenusOriginal = new List<MenuRolDto>(listaMenus);
        }
        private async Task LoadMenus()
        {
            listaMenus = await iMenuService.GetMenusAsync() ?? new List<MenuRolDto>();
            if (listaMenus.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }

        private int PageSize = 10;
        private int CurrentPage = 1;
        private IEnumerable<MenuRolDto> PaginatedItems => listaMenus
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => listaMenus.Count > 0 ? (int)Math.Ceiling((double)listaMenus.Count / PageSize) : 1;

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

        private void OpenDeleteModal(int? idHRol, int? idHMenu)
        {
            selectedIdHRol = idHRol;
            selectedIdHMenu = idHMenu;
            showModal = true;
        }

        private void CloseModal()
        {
            selectedIdHRol = null;
            selectedIdHMenu = null;
            showModal = false;
        }

        //Modificaci�n: No recargar toda la lista despu�s de eliminar un elemento
        private async Task ConfirmDelete(MenuRolDto menu)
        {
            objEventTracking.CodigoHomologacionMenu = "/menu-config-lista";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "btnEliminar";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Local) + ' ' +
                                              await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Apellido_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (menu != null && iMenuService != null)
            {
                // Guardamos la p�gina actual antes de la modificaci�n
                int paginaActual = CurrentPage;

                var result = await iMenuService.DeleteMenuAsync(menu.IdHRol, menu.IdHMenu);
                if (result)
                {
                    // Modificar solo el estado del elemento en la lista sin recargar toda la lista
                    var menuModificado = listaMenus?.FirstOrDefault(m => m.IdHRol == menu.IdHRol && m.IdHMenu == menu.IdHMenu);
                    if (menuModificado != null)
                    {
                        menuModificado.Estado = menuModificado.Estado == "A" ? "X" : "A";
                    }

                    // Restauramos la p�gina actual
                    CurrentPage = paginaActual;

                    CloseModal();
                    if (menuModificado.Estado == "A")
                    {
                        toastService?.CreateToastMessage(ToastType.Success, "Men� activado correctamente.");
                    }
                    else
                    {
                        toastService?.CreateToastMessage(ToastType.Success, "Men� desactivado correctamente.");
                    }

                    // Eliminamos la recarga innecesaria de toda la lista y la navegaci�n
                    // await LoadMenus();
                    // navigationManager?.NavigateTo("/menu-config-lista");

                    StateHasChanged(); // Forzar actualizaci�n sin recargar toda la p�gina
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al desactivar el registro.");
                }
            }
        }

        //private async Task ConfirmDelete(MenuRolDto menu)
        //{
        //    objEventTracking.CodigoHomologacionMenu = "Administraci�n de Men�";
        //    objEventTracking.NombreAccion = "ConfirmDelete";
        //    objEventTracking.NombreControl = "ConfirmDelete";
        //    objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
        //    objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
        //    objEventTracking.ParametroJson = "{}";
        //    objEventTracking.UbicacionJson = "";
        //    await iBusquedaService.AddEventTrackingAsync(objEventTracking);



        //    menu.Estado = menu.Estado == "A" ? "X" : "A";

        //    if (menu != null  && iMenuService != null)
        //    {
        //        var result = await iMenuService.DeleteMenuAsync(menu.IdHRol, menu.IdHMenu);
        //        if (result)
        //        {
        //            if (menu.Estado == "A")
        //            {
        //                toastService?.CreateToastMessage(ToastType.Success, "Men� activado correctamente.");
        //            }
        //            else
        //            {
        //                toastService?.CreateToastMessage(ToastType.Success, "Men� desactivado correctamente.");
        //            }
        //            navigationManager?.NavigateTo("/menu-config-lista");
        //            await LoadMenus();
        //            listaMenusOriginal = new List<MenuRolDto>(listaMenus);
        //            StateHasChanged();
        //        }
        //        else
        //        {
        //            toastService?.CreateToastMessage(ToastType.Danger, "Error al desactivar el registro.");
        //            navigationManager?.NavigateTo("/menu-config-lista");
        //        }
        //    }
        //}
        //private async Task ActualizarEstado(MenuRolDto menu)
        //{
        //    // Cambia el estado en funci�n del toggle (A = Activo, X = Inactivo)
        //    menu.Estado = menu.EstadoBool ? "A" : "X";

        //    // Simula una llamada API o base de datos para actualizar
        //    var resultado = await Http.PutAsJsonAsync($"api/menu/actualizarEstado/{menu.IdHRol}/{menu.IdHMenu}", menu);

        //    if (resultado.IsSuccessStatusCode)
        //    {
        //        // Se actualiz� correctamente
        //        Console.WriteLine($"Estado actualizado correctamente para el men� {menu.Menu}");
        //    }
        //    else
        //    {
        //        // Manejo de error
        //        Console.WriteLine("Error al actualizar el estado.");
        //    }
        //}

        private string filtroBusqueda = "";

        private void FiltrarLista(ChangeEventArgs e)
        {
            filtroBusqueda = e.Value?.ToString()?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(filtroBusqueda))
            {
                // Restaurar la lista original y la paginaci�n
                listaMenus = new List<MenuRolDto>(listaMenusOriginal);
            }
            else
            {
                // Aplicar el filtro sobre la lista original
                listaMenus = listaMenusOriginal
                    .Where(m => m.Rol.ToLower().Contains(filtroBusqueda) || m.Menu.ToLower().Contains(filtroBusqueda))
                    .ToList();
            }

            // Reiniciar a la primera p�gina para mostrar resultados correctamente
            CurrentPage = 1;
        }


        private string sortColumn = nameof(MenuRolDto.Rol);
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

            listaMenus = sortAscending
                ? listaMenus.OrderBy(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList()
                : listaMenus.OrderByDescending(x => x.GetType().GetProperty(sortColumn)?.GetValue(x, null)).ToList();
        }

        private async Task ExportarExcel()
        {
            objEventTracking.CodigoHomologacionMenu = "/menu-config-lista";
            objEventTracking.NombreAccion = "ExportarExcel";
            objEventTracking.NombreControl = "btnExportarExcel";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listaMenus == null || !listaMenus.Any()) return;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Men�s");

            worksheet.Cells[1, 1].Value = "Rol";
            worksheet.Cells[1, 2].Value = "Men�";
            worksheet.Cells[1, 3].Value = "Estado";

            int row = 2;
            foreach (var menu in listaMenus)
            {
                worksheet.Cells[row, 1].Value = menu.Rol;
                worksheet.Cells[row, 2].Value = menu.Menu;
                worksheet.Cells[row, 3].Value = menu.Estado == "A" ? "Activo" : "Inactivo";
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            var fileBytes = package.GetAsByteArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);
            await JSRuntime.InvokeVoidAsync("downloadExcel", "Menus_Export.xlsx", fileBase64);
        }

        private async Task ExportarPDF()
        {
            objEventTracking.CodigoHomologacionMenu = "/menu-config-lista";
            objEventTracking.NombreAccion = "ExportarPDF";
            objEventTracking.NombreControl = "btnExportarPDF";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listaMenus == null || !listaMenus.Any()) return;

            using var memoryStream = new MemoryStream();
            var document = new Document(iTextSharp.text.PageSize.A4);
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var table = new PdfPTable(3) { WidthPercentage = 100 };
            table.AddCell("Rol");
            table.AddCell("Men�");
            table.AddCell("Estado");

            foreach (var menu in listaMenus)
            {
                table.AddCell(menu.Rol);
                table.AddCell(menu.Menu);
                table.AddCell(menu.Estado == "A" ? "Activo" : "Inactivo");
            }

            document.Add(table);
            document.Close();

            var fileBase64 = Convert.ToBase64String(memoryStream.ToArray());
            await JSRuntime.InvokeVoidAsync("downloadFile", "Menus_Export.pdf", "application/pdf", fileBase64);
        }

    }
}
