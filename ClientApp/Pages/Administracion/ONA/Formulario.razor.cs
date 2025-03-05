using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.ONA
{
    /// <summary>
    /// Componente de formulario para la gestión de ONAs (Organismos Nacionales de Acreditación).
    /// Permite registrar y actualizar ONAs, así como la gestión de carga de archivos y países asociados.
    /// </summary>
    public partial class Formulario
    {
        // Botón de guardar con animación de carga
        private Button saveButton = default!;
        // Objeto que almacena la información del ONA a registrar o actualizar
        private OnaDto onas = new OnaDto();
        // Lista de países disponibles
        private List<VwPaisDto> paises = new(); // Lista para almacenar países
        // ID del país seleccionado
        private int? paisSeleccionado; // ID del país seleccionado
        /// <summary>
        /// Servicio de gestión de ONAs.
        /// </summary>
        [Inject]
        public IONAService? iONAsService { get; set; }
        /// <summary>
        /// Servicio de utilidades para la gestión de archivos e iconos.
        /// </summary>
        [Inject]
        public IUtilitiesService? iUtilService { get; set; }
        /// <summary>
        /// Servicio de navegación.
        /// </summary>

        [Inject]
        public NavigationManager? navigationManager { get; set; }

        /// <summary>
        /// ID del ONA a editar, nulo si se está creando uno nuevo.
        /// </summary>
        [Parameter]
        public int? Id { get; set; }
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
        /// Servicio de almacenamiento local en el navegador.
        /// </summary>
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }

        // Archivo cargado por el usuario
        private IBrowserFile? uploadedFile;

        /// <summary>
        /// Método para manejar la carga de archivos y validar formatos permitidos (.png y .svg).
        /// </summary>
        /// <param name="e">Evento que contiene la información del archivo subido.</param>
        /// <param name="idOna">ID del ONA al que se asociará el archivo.</param>
        private async Task OnInputFileChange(InputFileChangeEventArgs e, int idOna)
        {
            try
            {
                uploadedFile = e.File;

                if (uploadedFile == null)
                {
                    Console.WriteLine("No se seleccionó ningún archivo.");
                    return;
                }

                // Validar la extensión del archivo
                var fileExtension = Path.GetExtension(uploadedFile.Name).ToLower();
                if (fileExtension != ".png" && fileExtension != ".svg")
                {
                    Console.WriteLine("Formato de archivo no permitido.");
                    return;
                }

                // Llamar al servicio para subir el archivo con el idOna
                var uploadedFilePath = await iUtilService.UploadIconAsync(uploadedFile, idOna);

                // Actualizar la propiedad con la ruta relativa devuelta por el backend
                onas.UrlIcono = uploadedFilePath;

                Console.WriteLine($"Archivo cargado exitosamente: {uploadedFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el archivo: {ex.Message}");
            }
        }

        /// <summary>
        /// Método asincrónico que inicializa el formulario cargando la lista de países y el ONA si se está editando.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            paises = await iONAsService.GetPaisesAsync();

            if (Id > 0 && iONAsService != null)
            {
                objEventTracking.CodigoHomologacionMenu = "/editar-ona";
                objEventTracking.NombreAccion = "OnInitializedAsync";
                objEventTracking.NombreControl = "editar-ona";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                onas = await iONAsService.GetONAsAsync(Id.Value);
            }
            else
            {
                objEventTracking.CodigoHomologacionMenu = "/nuevo-ona";
                objEventTracking.NombreAccion = "OnInitializedAsync";
                objEventTracking.NombreControl = "nuevo-ona";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);
            }
        }

        /// <summary>
        /// Método que registra o actualiza la información de un ONA en la base de datos.
        /// </summary>
        private async Task RegistrarONA()
        {
            objEventTracking.CodigoHomologacionMenu = "/nuevo-ona";
            objEventTracking.NombreAccion = "RegistrarONA";
            objEventTracking.NombreControl = "btnGuardar";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            saveButton.ShowLoading("Guardando...");

            if (iONAsService != null)
            {
                var result = await iONAsService.RegistrarONAsActualizar(onas);
                if (result.registroCorrecto)
                {
                    toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                    navigationManager?.NavigateTo("/onas");
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al registrar en el servidor");
                }
            }

            saveButton.HideLoading();
        }

        /// <summary>
        /// Método que actualiza el país seleccionado para el ONA.
        /// </summary>
        /// <param name="e">Evento de cambio en la selección del país.</param>
        private void ActualizarPais(ChangeEventArgs e)
        {
            // Obtener el ID seleccionado
            onas.IdHomologacionPais = int.TryParse(e.Value?.ToString(), out var valor) ? valor : null;

        }
        /// <summary>
        /// Método que redirige a la lista de ONAs sin guardar cambios.
        /// </summary>
        private void Regresar()
        {
            navigationManager?.NavigateTo("/onas");
        }
    }
}
