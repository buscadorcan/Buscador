using BlazorBootstrap;
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
        private IConexionService? service { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        private ConexionDto conexion = new ConexionDto();
        private EditContext? editContext = new EditContext(new ConexionDto());
        private IBrowserFile? uploadedFile;
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            uploadedFile = e.File;
            Console.WriteLine("OnInputFileChange method called");
        }
      
        private async Task RegistrarMigracionExcel()
        {
            saveButton.ShowLoading("Guardando...");

            var buffer = new byte[uploadedFile.Size];
            await uploadedFile.OpenReadStream().ReadAsync(buffer);

            using var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(buffer), "file", uploadedFile.Name);

            if (service != null)
            {
              var response = await service.ImportarExcel(content);
              if (response.IsSuccessStatusCode)
              {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
              }
              else
              {
                var errorResult = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {errorResult}");
              }
            }

            saveButton.HideLoading();
        }
    }
}