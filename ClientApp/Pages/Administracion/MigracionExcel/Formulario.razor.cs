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
    /// <summary>
    /// Componente de formulario para la migración de archivos Excel.
    /// Permite la carga y gestión de archivos de migración, seleccionando un ONA asociado.
    /// </summary>
    public partial class Formulario
    {
        /// <summary>
        /// Botón de guardar con animación de carga.
        /// </summary>
        private Button saveButton = default!;
        /// <summary>
        /// ID de la migración (nulo si es una nueva migración).
        /// </summary>
        [Parameter]
        public int? Id { get; set; }
        /// <summary>
        /// Servicio de migración de archivos Excel.
        /// </summary>
        [Inject]
        private IMigracionExcelService? service { get; set; }
        /// <summary>
        /// Servicio de navegación para redirigir a otras páginas.
        /// </summary>
        [Inject]
        public NavigationManager? navigationManager { get; set; }

        /// <summary>
        /// Objeto que almacena la información de la migración a registrar.
        /// </summary>
        private MigracionExcelDto migracion = new MigracionExcelDto();
        /// <summary>
        /// Contexto de edición para la validación del formulario.
        /// </summary>
        private EditContext? editContext = new EditContext(new MigracionExcelDto());
        /// <summary>
        /// Archivo seleccionado para la carga.
        /// </summary>
        private IBrowserFile? uploadedFile;
        /// <summary>
        /// Lista de ONAs disponibles para selección.
        /// </summary>
        private List<OnaDto>? listaONAs;
        /// <summary>
        /// ONA seleccionado por el usuario.
        /// </summary>
        private OnaDto? onaSelected;
        /// <summary>
        /// Esquema seleccionado (puede ser opcional).
        /// </summary>
        private EsquemaDto? esquemaSelected;
        /// <summary>
        /// ID del usuario seleccionado.
        /// </summary>
        private int? selectedIdUsuario;
        /// <summary>
        /// Mensaje mostrado en el modal.
        /// </summary>
        private string modalMessage;
        /// <summary>
        /// Controla la visibilidad de la ventana modal.
        /// </summary>
        private bool showModal; // Controlar la visibilidad de la ventana modal  
        /// <summary>
        /// Servicio de almacenamiento local en el navegador.
        /// </summary>
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        /// <summary>
        /// Servicio para la gestión de ONAs.
        /// </summary>
        [Inject]
        public IONAService? iONAservice { get; set; }
        /// <summary>
        /// Servicio de notificaciones Toast.
        /// </summary>
        [Inject]
        public Services.ToastService? toastService { get; set; }
        /// <summary>
        /// Servicio de búsqueda y registro de eventos.
        /// </summary>
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();

        /// <summary>
        /// Método asincrónico que se ejecuta al inicializar el componente.
        /// Carga la lista de ONAs disponibles y verifica permisos de usuario.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/nueva-migarcion-excel";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "nueva-migarcion-excel";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

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

        /// <summary>
        /// Método para cambiar la selección del ONA.
        /// </summary>
        /// <param name="_onaSelected">ONA seleccionado.</param>
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

        /// <summary>
        /// Carga la lista de ONAs disponibles.
        /// </summary>
        private async Task LoadONAs()
        {
            if (iONAservice != null)
            {
                listaONAs = await iONAservice.GetONAsAsync();
            }
        }

        /// <summary>
        /// Maneja la carga de un archivo en el formulario.
        /// </summary>
        /// <param name="e">Evento de cambio de archivo.</param>
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            uploadedFile = e.File;
            Console.WriteLine("OnInputFileChange method called");
        }

        /// <summary>
        /// Registra la migración de un archivo Excel al sistema.
        /// </summary>
        private async Task RegistrarMigracionExcel()
        {
            try
            {
                objEventTracking.CodigoHomologacionMenu = "/nueva-migarcion-excel";
                objEventTracking.NombreAccion = "RegistrarMigracionExcel";
                objEventTracking.NombreControl = "btnGuardar";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
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

        /// <summary>
        /// Abre el modal de confirmación.
        /// </summary>
        private async Task OpenMigracionModal()
        {
            showModal = true;
        }

        /// <summary>
        /// Cierra el modal de confirmación.
        /// </summary>
        private void CloseModal()
        {
            selectedIdUsuario = null;
            showModal = false;
        }

        /// <summary>
        /// Confirma la carga del archivo y ejecuta la migración.
        /// </summary>
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