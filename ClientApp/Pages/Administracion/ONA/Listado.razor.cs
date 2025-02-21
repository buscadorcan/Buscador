using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;
using System.Text;

namespace ClientApp.Pages.Administracion.ONA
{
    /// <summary>
    /// P�gina de listado de ONAs (Organismos Nacionales de Acreditaci�n).
    /// Permite visualizar, paginar y eliminar registros de ONAs.
    /// </summary>
    public partial class Listado
    {
        // Controla la visibilidad del modal de eliminaci�n
        private bool showModal = false; // Controla la visibilidad del modal
        // ID del ONA seleccionado para eliminaci�n
        private int? selectedIdONA;    // Almacena el ID del ONA seleccionado
        // Lista de ONAs obtenidos desde el servicio
        private List<OnaDto>? listaONAs; // Lista de registros ONAs
        // Bot�n de guardar con animaci�n de carga
        private Button saveButton = default!;
        // Componente de la grilla para mostrar la lista de ONAs
        private Grid<OnaDto>? grid;
        /// <summary>
        /// Servicio de gesti�n de ONAs.
        /// </summary>
        [Inject]
        public IONAService? iONAservice { get; set; }
        /// <summary>
        /// Servicio de notificaciones Toast.
        /// </summary>
        [Inject]
        public Services.ToastService? toastService { get; set; }
        /// <summary>
        /// Servicio de navegaci�n.
        /// </summary>
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        /// <summary>
        /// Servicio de almacenamiento local en el navegador.
        /// </summary>
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        /// <summary>
        /// Servicio de b�squeda y registro de eventos.
        /// </summary>
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();
        // Indica si el usuario tiene rol de administrador
        private bool isRolAdmin;

        /// <summary>
        /// M�todo que carga la lista de ONAs seg�n el rol del usuario.
        /// </summary>
        private async Task LoadONAs()
        {
            if (iONAservice != null)
            {
                var rolRelacionado = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                isRolAdmin = rolRelacionado == "KEY_USER_CAN";
                if (isRolAdmin)
                {
                    listaONAs = await iONAservice.GetONAsAsync() ?? new List<OnaDto>();
                }
                else
                {
                    int IdOna = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
                    listaONAs = await iONAservice.GetListByONAsAsync(IdOna) ?? new List<OnaDto>();
                }
            }
            // Ajusta la paginaci�n si la lista est� vac�a o cambia
            if (listaONAs.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }
        // Configuraci�n de paginaci�n
        private int PageSize = 10; // Cantidad de registros por p�gina
        private int CurrentPage = 1;

        /// <summary>
        /// Obtiene los elementos paginados para la grilla.
        /// </summary>
        private IEnumerable<OnaDto> PaginatedItems => listaONAs
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        /// <summary>
        /// Calcula el n�mero total de p�ginas basado en el n�mero de registros.
        /// </summary>
        private int TotalPages => listaONAs.Count > 0 ? (int)Math.Ceiling((double)listaONAs.Count / PageSize) : 1;
        /// <summary>
        /// Calcula el n�mero total de p�ginas basado en el n�mero de registros.
        /// </summary>
        private bool CanGoPrevious => CurrentPage > 1;
        /// <summary>
        /// Indica si se puede avanzar a la siguiente p�gina.
        /// </summary>
        private bool CanGoNext => CurrentPage < TotalPages;

        /// <summary>
        /// Cambia a la p�gina anterior en la paginaci�n.
        /// </summary>
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

         /// <summary>
        /// Cambia a la siguiente p�gina en la paginaci�n.
        /// </summary>
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// Proveedor de datos para la grilla de ONAs.
        /// </summary>
        private async Task<GridDataProviderResult<OnaDto>> ONAsDataProvider(GridDataProviderRequest<OnaDto> request)
        {
            try
            {
                if (listaONAs is null && iONAservice != null)
                {
                    await LoadONAs(); // Carga los datos si a�n no est�n cargados
                }

                return await Task.FromResult(request.ApplyTo(listaONAs ?? new List<OnaDto>()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Abre el modal de confirmaci�n de eliminaci�n.
        /// </summary>
        /// <param name="idONA">ID del ONA a eliminar.</param>
        private void OpenDeleteModal(int idONA)
        {
            selectedIdONA = idONA;
            showModal = true;
        }

        /// <summary>
        /// Cierra el modal de confirmaci�n de eliminaci�n.
        /// </summary>
        private void CloseModal()
        {
            selectedIdONA = null;
            showModal = false;
        }

        /// <summary>
        /// Confirma la eliminaci�n de un ONA.
        /// </summary>
        private async Task ConfirmDelete()
        {
            objEventTracking.NombrePagina = "Administraci�n de ONA";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "ConfirmDelete";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Local) + ' ' + iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Apellido_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

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

        /// <summary>
        /// M�todo asincr�nico que se ejecuta al inicializar el componente.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await LoadONAs(); // Carga la lista al iniciar el componente
        }

    }
}
