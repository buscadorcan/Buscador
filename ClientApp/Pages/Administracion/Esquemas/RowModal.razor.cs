using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Esquemas
{

    public partial class RowModal
    {
        [Parameter] public List<HomologacionDto> columnas { get; set; }
        [Parameter] public List<HomologacionDto> listaVwHomologacion { get; set;}

        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        private IEnumerable<HomologacionDto> PaginatedItems => columnas
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => columnas.Count > 0 ? (int)Math.Ceiling((double)columnas.Count / PageSize) : 1;

        private bool CanGoPrevious => CurrentPage > 1;
        private bool CanGoNext => CurrentPage < TotalPages;

        /// <summary>
        /// PreviousPage: Navegar los registros anteriores
        /// </summary>
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        /// <summary>
        /// NextPage: Navegar los registros posteriores
        /// </summary>
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// OnInitializedAsync: Ajusta la paginación si la lista está vacía o cambia
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            
            if (columnas.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }
    }
    
}