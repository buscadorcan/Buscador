using Microsoft.AspNetCore.Components;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Clase InputPaginations
    /// </summary>
    public partial class InputPaginations : ComponentBase
    {
        /// <summary>
        /// Gets or sets the total items.
        /// </summary>
        [Parameter] public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        [Parameter] public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the display pages.
        /// </summary>
        [Parameter] public int DisplayPages { get; set; }

        /// <summary>
        /// Gets or sets the active page number.
        /// </summary>
        [Parameter] public int ActivePageNumber { get; set; }

        /// <summary>
        /// Gets or sets the active page number changed.
        /// </summary>
        [Parameter] public EventCallback<int> ActivePageNumberChanged { get; set; }

        /// <summary>
        /// Gets or sets the display iterms for pages changed.
        /// </summary>
        [Parameter] public EventCallback<int> DisplayPagesChanged { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <param name="newDisplayPages"></param>
        private async Task HandleDisplayPagesChange(int newDisplayPages)
        {
            await DisplayPagesChanged.InvokeAsync(newDisplayPages);
        }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        private async Task OnPageChangedAsync(int newPageNumber)
        {
            await ActivePageNumberChanged.InvokeAsync(newPageNumber);
        }
    }
}