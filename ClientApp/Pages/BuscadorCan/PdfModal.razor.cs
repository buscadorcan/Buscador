using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class PdfModal
    {
        [Parameter]
        public string? PdfUrl { get; set; }
        private Modal? modal;
    }
}
