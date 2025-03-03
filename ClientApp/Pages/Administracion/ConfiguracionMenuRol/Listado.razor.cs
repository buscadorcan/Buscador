using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ClientApp.Pages.Administracion.ConfiguracionMenuRol
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
        public Services.ToastService? toastService { get; set; }

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

        //Modificación: No recargar toda la lista después de eliminar un elemento
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
                // Guardamos la página actual antes de la modificación
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

                    // Restauramos la página actual
                    CurrentPage = paginaActual;

                    CloseModal();
                    if (menuModificado.Estado == "A")
                    {
                        toastService?.CreateToastMessage(ToastType.Success, "Menú activado correctamente.");
                    }
                    else
                    {
                        toastService?.CreateToastMessage(ToastType.Success, "Menú desactivado correctamente.");
                    }

                    // Eliminamos la recarga innecesaria de toda la lista y la navegación
                    // await LoadMenus();
                    // navigationManager?.NavigateTo("/menu-config-lista");

                    StateHasChanged(); // Forzar actualización sin recargar toda la página
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al desactivar el registro.");
                }
            }
        }

        //private async Task ConfirmDelete(MenuRolDto menu)
        //{
        //    objEventTracking.CodigoHomologacionMenu = "Administración de Menú";
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
        //                toastService?.CreateToastMessage(ToastType.Success, "Menú activado correctamente.");
        //            }
        //            else
        //            {
        //                toastService?.CreateToastMessage(ToastType.Success, "Menú desactivado correctamente.");
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
        //    // Cambia el estado en función del toggle (A = Activo, X = Inactivo)
        //    menu.Estado = menu.EstadoBool ? "A" : "X";

        //    // Simula una llamada API o base de datos para actualizar
        //    var resultado = await Http.PutAsJsonAsync($"api/menu/actualizarEstado/{menu.IdHRol}/{menu.IdHMenu}", menu);

        //    if (resultado.IsSuccessStatusCode)
        //    {
        //        // Se actualizó correctamente
        //        Console.WriteLine($"Estado actualizado correctamente para el menú {menu.Menu}");
        //    }
        //    else
        //    {
        //        // Manejo de error
        //        Console.WriteLine("Error al actualizar el estado.");
        //    }
        //}

    }
}
