using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Esquemas
{
    /// <summary>
    /// Componente modal para mostrar una tabla paginada de homologaciones.
    /// </summary>
    public partial class RowModal
    {
        /// <summary>
        /// Lista de columnas que se mostrar�n en la tabla.
        /// </summary>
        [Parameter] public List<HomologacionDto> columnas { get; set; }
        /// <summary>
        /// Lista de homologaciones disponibles para selecci�n.
        /// </summary>
        [Parameter] public List<HomologacionDto> listaVwHomologacion { get; set;}
        /// <summary>
        /// N�mero de elementos por p�gina en la tabla.
        /// </summary>
        private int PageSize = 10; // Cantidad de registros por p�gina
        /// <summary>
        /// P�gina actual de la tabla.
        /// </summary>
        private int CurrentPage = 1;

        /// <summary>
        /// Obtiene los elementos paginados para mostrar en la tabla.
        /// </summary>
        private IEnumerable<HomologacionDto> PaginatedItems => columnas
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        /// <summary>
        /// Calcula el n�mero total de p�ginas en funci�n del tama�o de la lista.
        /// </summary>
        private int TotalPages => columnas.Count > 0 ? (int)Math.Ceiling((double)columnas.Count / PageSize) : 1;
        /// <summary>
        /// Determina si se puede retroceder a la p�gina anterior.
        /// </summary>
        private bool CanGoPrevious => CurrentPage > 1;
        /// <summary>
        /// Determina si se puede avanzar a la siguiente p�gina.
        /// </summary>
        private bool CanGoNext => CurrentPage < TotalPages;

        /// <summary>
        /// M�todo para navegar a la p�gina anterior en la tabla.
        /// </summary>
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        /// <summary>
        /// Metodo para navegar a la siguiente p�gina en la tabla.
        /// </summary>
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// M�todo asincr�nico que ajusta la paginaci�n cuando la lista de columnas cambia.
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