using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.ONA
{
    public partial class Formulario
    {
        private Button saveButton = default!;
        private OnaDto onas = new OnaDto();
        private List<VwPaisDto> paises = new(); // Lista para almacenar pa�ses
        private int? paisSeleccionado; // ID del pa�s seleccionado
        [Inject]
        public IONAService? iONAsService { get; set; }
        [Inject]
        public IUtilitiesService? iUtilService { get; set; }

        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }
   
       
        private IBrowserFile? uploadedFile;
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            try
            {
                // Obtener el archivo subido
                uploadedFile = e.File;

                // Verificar si hay archivo cargado
                if (uploadedFile == null)
                {
                    Console.WriteLine("No se seleccion� ning�n archivo.");
                    return;
                }

                // Validar la extensi�n del archivo
                var fileExtension = Path.GetExtension(uploadedFile.Name).ToLower();
                if (fileExtension != ".png" && fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".svg")
                {
                    Console.WriteLine("Formato de archivo no permitido.");
                    return;
                }

                // Llamar al servicio para subir el archivo
                var uploadedFilePath = await iUtilService.UploadIconAsync(uploadedFile);

                // Actualizar la propiedad con la ruta relativa devuelta por el backend
                onas.UrlIcono = uploadedFilePath;

                Console.WriteLine($"Archivo cargado exitosamente: {uploadedFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el archivo: {ex.Message}");
            }
        }



        protected override async Task OnInitializedAsync()
        {
            paises = await iONAsService.GetPaisesAsync();

            if (Id > 0 && iONAsService != null) {
                onas = await iONAsService.GetONAsAsync(Id.Value);
            } 
        }
        private async Task RegistrarONA()
        {
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

        private void ActualizarPais(ChangeEventArgs e)
        {
            // Obtener el ID seleccionado
            onas.IdHomologacionPais = int.TryParse(e.Value?.ToString(), out var valor) ? valor : null;

        }
        private void Regresar()
        {
            navigationManager?.NavigateTo("/onas");
        }
    }
}
