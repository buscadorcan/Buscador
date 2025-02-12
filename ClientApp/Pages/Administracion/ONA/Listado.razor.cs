using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;
using System.Text;

namespace ClientApp.Pages.Administracion.ONA
{
    public partial class Listado
    {
        private bool showModal = false; // Controla la visibilidad del modal
        private int? selectedIdONA;    // Almacena el ID del ONA seleccionado
        private List<OnaDto>? listaONAs; // Lista de registros ONAs
        private Button saveButton = default!;
        private Grid<OnaDto>? grid;
        [Inject]
        public IONAService? iONAservice { get; set; }

        [Inject]
        public Services.ToastService? toastService { get; set; }

        [Inject]
        public NavigationManager? navigationManager { get; set; }

        // Método para cargar la lista de ONAs
        private async Task LoadONAs()
        {
            if (iONAservice != null)
            {
                listaONAs = await iONAservice.GetONAsAsync();
            }
            // Ajusta la paginación si la lista está vacía o cambia
            if (listaONAs.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }
        //private List<LogMigracionDto> listasHevd = new();
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        private IEnumerable<OnaDto> PaginatedItems => listaONAs
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => listaONAs.Count > 0 ? (int)Math.Ceiling((double)listaONAs.Count / PageSize) : 1;

        private bool CanGoPrevious => CurrentPage > 1;
        private bool CanGoNext => CurrentPage < TotalPages;

        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }
        // Proveedor de datos para el grid
        private async Task<GridDataProviderResult<OnaDto>> ONAsDataProvider(GridDataProviderRequest<OnaDto> request)
        {
            try
            {
                if (listaONAs is null && iONAservice != null)
                {
                    await LoadONAs(); // Carga los datos si aún no están cargados
                }

                return await Task.FromResult(request.ApplyTo(listaONAs ?? new List<OnaDto>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Abre el modal
        private void OpenDeleteModal(int idONA)
        {
            selectedIdONA = idONA;
            showModal = true;
        }

        // Cierra el modal
        private void CloseModal()
        {
            selectedIdONA = null;
            showModal = false;
        }

        // Confirmar eliminación del registro
        private async Task ConfirmDelete()
        {
            if (selectedIdONA.HasValue && iONAservice != null)
            {
                var result = await iONAservice.DeleteONAAsync(selectedIdONA.Value);

                if (result)
                {
                    CloseModal(); // Cierra el modal
                    toastService?.CreateToastMessage(ToastType.Success, "Registro eliminado exitosamente.");
                    await LoadONAs(); // Actualiza la lista
                    await grid?.RefreshDataAsync(); //resfresca la grilla
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar el registro.");
                }
            }

        }

        // Método de inicialización
        protected override async Task OnInitializedAsync()
        {
            await LoadONAs(); // Carga la lista al iniciar el componente
        }

    }
}
