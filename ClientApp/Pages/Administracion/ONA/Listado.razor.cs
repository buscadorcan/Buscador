using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.ONA
{
    public partial class Listado
    {
        private bool showModal = false; // Controla la visibilidad del modal
        private int? selectedIdONA;    // Almacena el ID del ONA seleccionado
        private List<ONADto>? listaONAs; // Lista de registros ONAs
        private Button saveButton = default!;
        private Grid<ONADto>? grid;
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
        }

        // Proveedor de datos para el grid
        private async Task<GridDataProviderResult<ONADto>> ONAsDataProvider(GridDataProviderRequest<ONADto> request)
        {
            try
            {
                if (listaONAs is null && iONAservice != null)
                {
                    await LoadONAs(); // Carga los datos si aún no están cargados
                }

                return await Task.FromResult(request.ApplyTo(listaONAs ?? new List<ONADto>()));
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
