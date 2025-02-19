using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Pages.Administracion.Esquemas;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.MigracionExcel
{
    public partial class Formulario
    {
        private Button saveButton = default!;
        [Parameter]
        public int? Id { get; set; }
        [Inject]
        private IMigracionExcelService? service { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        private MigracionExcelDto migracion = new MigracionExcelDto();
        private EditContext? editContext = new EditContext(new MigracionExcelDto());
        private IBrowserFile? uploadedFile;
        private List<OnaDto>? listaONAs;
        private OnaDto? onaSelected;
        private EsquemaDto? esquemaSelected;
        private int? selectedIdUsuario;
        private string modalMessage;
        private bool showModal; // Controlar la visibilidad de la ventana modal  

        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        [Inject]
        public IONAService? iONAservice { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            var onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            var rol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            bool accessRol = rol == "KEY_USER_CAN";
            if (accessRol)
            {
                if (listaONAs is null && iONAservice != null)
                {
                    await LoadONAs();
                }
            }
            else
            {
                await LoadONAs();
                listaONAs = listaONAs.Where(onas => onas.IdONA == onaPais).ToList();
            }
        }
        private async Task CambiarSeleccionOna(OnaDto _onaSelected)
        {
            if (esquemaSelected != null)
            {
                onaSelected = _onaSelected;
            }
            else
            {
                onaSelected = _onaSelected;
            }
            showDropdown = false;
            StateHasChanged();
        }
        private async Task LoadONAs()
        {
            if (iONAservice != null)
            {
                listaONAs = await iONAservice.GetONAsAsync();
            }
        }
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            uploadedFile = e.File;
            Console.WriteLine("OnInputFileChange method called");
        }
        private async Task RegistrarMigracionExcel()
        {
            try
            {
                objEventTracking.NombrePagina = "Archivo migración";
                objEventTracking.NombreAccion = "RegistrarMigracionExcel";
                objEventTracking.NombreControl = "RegistrarMigracionExcel";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Local) + ' ' + iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Apellido_Local);
                objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
                objEventTracking.ParametroJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                if (onaSelected != null && onaSelected.IdONA > 0)
                {
                    if (uploadedFile == null)
                    {
                        toastService?.CreateToastMessage(ToastType.Warning, "Debe seleccionar un archivo");
                        navigationManager?.NavigateTo("/nueva-migarcion-excel");
                        return;
                    }
                    saveButton.ShowLoading("Guardando...");

                    var maxFileSize = 10485760; // 10 MB
                    var buffer = new byte[uploadedFile.Size];
                    await uploadedFile.OpenReadStream(maxFileSize).ReadAsync(buffer);

                    using var content = new MultipartFormDataContent();
                    content.Add(new ByteArrayContent(buffer), "file", uploadedFile.Name);

                    if (service != null)
                    {
                        var response = await service.ImportarExcel(content, onaSelected.IdONA);
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(result);
                        }
                        else
                        {
                            saveButton.HideLoading();
                            var errorResult = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Error: {errorResult}");
                        }
                    }

                    saveButton.HideLoading();
                    navigationManager?.NavigateTo("/migracion-excel");
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Warning, "Seleccione un Ona");
                    navigationManager?.NavigateTo("/nueva-migarcion-excel");
                }
                
            }
            catch (Exception)
            {
                saveButton.HideLoading();
                navigationManager?.NavigateTo("/migracion-excel");
            }
            
        }
        private async Task OpenDeleteModal()
        {
            showModal = true;
        }

        // Cierra el modal
        private void CloseModal()
        {
            selectedIdUsuario = null;
            showModal = false;
        }

        // Confirmar carga del archivo
        private async Task ConfirmCarga()
        {
            if (service != null)
            {
                CloseModal();
                await RegistrarMigracionExcel();
            }

        }
    }
}