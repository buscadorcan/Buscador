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
        private IMigracionExcelService? service { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        private MigracionExcelDto migracion = new MigracionExcelDto();
        private EditContext? editContext = new EditContext(new MigracionExcelDto());
        private IBrowserFile? uploadedFile;
        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            uploadedFile = e.File;
            Console.WriteLine("OnInputFileChange method called");
        }
      
        private async Task RegistrarMigracionExcel()
        {
            saveButton.ShowLoading("Guardando...");

            var maxFileSize = 10485760; // 10 MB
            var buffer = new byte[uploadedFile.Size];
            await uploadedFile.OpenReadStream(maxFileSize).ReadAsync(buffer);

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
            navigationManager?.NavigateTo("/migracion-excel");
        }
    }
}