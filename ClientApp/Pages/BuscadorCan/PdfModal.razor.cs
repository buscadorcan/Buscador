using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Pages.BuscadorCan
{

    public partial class PdfModal
    {
        [Parameter]
        public string? PdfUrl { get; set; }
        private Modal? modal;

        public async Task ShowAsync(string pdfUrl)
        {
            PdfUrl = pdfUrl; // Establece la URL del PDF
            if (modal != null)
            {
                await modal.ShowAsync(); // Muestra el modal
            }
        }

        public async Task HideAsync()
        {
            if (modal != null)
            {
                await modal.HideAsync(); // Cierra el modal
            }
        }
    }
}
