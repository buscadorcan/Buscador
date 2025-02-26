using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Conexion
{

    public partial class Listado
    {
        ToastsPlacement toastsPlacement = ToastsPlacement.TopRight;
        private bool showModal; // Controlar la visibilidad de la ventana modal  
        private string modalMessage;
        private int? selectedIdOna;
        [Inject]
        public Services.ToastService? toastService { get; set; }
        List<ToastMessage> messages = new();

        [Inject]
        private IConexionService? iConexionService { get; set; }
        [Inject]
        private IDynamicService? iDynamicService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }

        private List<ONAConexionDto> listasHevd = new();
        private bool isRolAdmin;

        private EventTrackingDto objEventTracking { get; set; } = new();
        private bool IsLoading { get; set; } = false;
        private int ProgressValue { get; set; } = 0;
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        private IEnumerable<ONAConexionDto> PaginatedItems => listasHevd
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => listasHevd.Count > 0 ? (int)Math.Ceiling((double)listasHevd.Count / PageSize) : 1;

        private bool CanGoPrevious => CurrentPage > 1;
        private bool CanGoNext => CurrentPage < TotalPages;

        /// <summary>
        /// PreviousPage: Previo de las paginas del listado.
        /// </summary>
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;

                objEventTracking.NombrePagina = "/conexion";
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";

                iBusquedaService.AddEventTrackingAsync(objEventTracking);
            }
        }

        /// <summary>
        /// NextPage: Proximas paginas del listado.
        /// </summary>
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// OnInitializedAsync: Iniciado del listado, carga del rol relacionado y de conexiones.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.NombrePagina = "/conexion";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "conexion";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listasHevd != null && iConexionService != null)
            {
                var rolRelacionado = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                isRolAdmin = rolRelacionado == "KEY_USER_CAN";
                if (isRolAdmin)
                {
                    listasHevd = await iConexionService.GetConexionsAsync() ?? new List<ONAConexionDto>();
                }
                else
                {
                    int IdOna = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
                    listasHevd = await iConexionService.GetOnaConexionByOnaListAsync(IdOna) ?? new List<ONAConexionDto>();
                }
            }
            // Ajusta la paginación si la lista está vacía o cambia
            if (listasHevd.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }

        /// <summary>
        /// OnTestconexionClick: Test de la conexión externa, comprobando si la conexion esta en linea.
        /// </summary>
        /// <param name="conexion">
        /// <returns cref="Task"> devuelve un valor true o false dependiendo de la conexion</returns>
        private async Task<bool> OnTestconexionClick(int conexion)
        {
            try
            {
                objEventTracking.NombrePagina = "/conexion";
                objEventTracking.NombreAccion = "OnTestconexionClick";
                objEventTracking.NombreControl = "OnTestconexionClick";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                if (iDynamicService != null && listasHevd != null)
                {
                    // Llamar al método del servicio para probar la conexión
                    bool isConnected = await iDynamicService.TestConnectionAsync(conexion);
                    var toastMessage = new ToastMessage
                    {
                        Type = isConnected ? ToastType.Success : ToastType.Danger,
                        Title = "Mensaje de confirmación",
                        HelpText = $"{DateTime.Now}",
                        Message = isConnected ? "Conexión satisfactoria" : "Conexión fallida",
                    };

                    messages.Add(toastMessage);
                    StateHasChanged();
                    // Configurar el cierre automático después de 5 segundos
                    _ = Task.Delay(5000).ContinueWith(_ =>
                    {
                        messages.Remove(toastMessage);
                        InvokeAsync(StateHasChanged); // Actualizar la UI
                    });

                    if (isConnected)
                    {
                        return true; // Devuelve true si la conexión fue exitosa
                    }
                }
                return false; // Devuelve false si algo falla
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        /// <summary>
        /// OnMigrarClick: Migrar los datos de la ONA desde el servidor externo.
        /// </summary>
        /// <param name="conexion">
        /// <returns> devuelve un valor true o false dependiendo de la migracion</returns>
        private async Task<bool> OnMigrarClick(int conexion)
        {
            objEventTracking.NombrePagina = "/conexion";
            objEventTracking.NombreAccion = "OnMigrarClick";
            objEventTracking.NombreControl = "OnMigrarClick";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (iDynamicService != null && listasHevd != null)
            {
                IsLoading = true;
                ProgressValue = 45;
                StateHasChanged();

                try
                {
                    // Iniciar un temporizador que aumente progresivamente hasta el 50%
                    var progressTask = Task.Run(async () =>
                    {
                        while (ProgressValue < 50)
                        {
                            await Task.Delay(500); // Espera 500ms antes de aumentar
                            ProgressValue += 5; // Aumenta en 5% cada 500ms
                        }
                    });

                    // Ejecutar la migración en paralelo mientras se actualiza la barra de progreso
                    bool migracion = await iDynamicService.MigrarConexionAsync(conexion);

                    // Esperar a que la barra de progreso llegue a 50% antes de completarla
                    await progressTask;

                    // Completar la barra de progreso
                    ProgressValue = 100;
                    StateHasChanged();

                    var toastMessage = new ToastMessage
                    {
                        Type = migracion ? ToastType.Success : ToastType.Danger,
                        Title = "Mensaje de confirmación",
                        HelpText = $"{DateTime.Now}",
                        Message = migracion ? "Migración satisfactoria" : "Migración no realizada",
                    };

                    messages.Add(toastMessage);
                    StateHasChanged();
                    // Ocultar mensaje después de 5 segundos
                    _ = Task.Delay(5000).ContinueWith(_ =>
                    {
                        messages.Remove(toastMessage);
                        InvokeAsync(StateHasChanged);
                    });

                    if (migracion)
                    {
                        return true;
                    }
                }
                finally
                {
                    IsLoading = false;
                    ProgressValue = 0;
                    StateHasChanged();
                }
            }
            return false;
        }

        /// <summary>
        /// OpenDeleteModal: Abre el modal.
        /// </summary>
        private void OpenDeleteModal(int idOna)
        {
            selectedIdOna = idOna;
            showModal = true;
        }

        /// <summary>
        /// CloseModal: Cerrar el modal.
        /// </summary>
        private void CloseModal()
        {
            selectedIdOna = null;
            showModal = false;
        }

        /// <summary>
        /// ConfirmDelete: Elimina la conexion externa de la organizacion.
        /// </summary>
        private async Task ConfirmDelete()
        {
            objEventTracking.NombrePagina = "/conexion";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "btnEliminar";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (selectedIdOna.HasValue && iConexionService != null)
            {
                int idOna = selectedIdOna.Value;
                var respuesta = await iConexionService.EliminarConexion(selectedIdOna.Value);
                if (respuesta.registroCorrecto)
                {
                    CloseModal();
                    listasHevd = listasHevd.Where(c => c.IdONA != idOna).ToList();
                    await OnInitializedAsync();
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar el registro.");
                }
            }

        }

    }
}