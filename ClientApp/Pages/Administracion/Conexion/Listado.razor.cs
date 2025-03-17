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

                objEventTracking.CodigoHomologacionMenu = "/conexion";
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
            objEventTracking.CodigoHomologacionMenu = "/conexion";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "conexion";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
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
                objEventTracking.CodigoHomologacionMenu = "/conexion";
                objEventTracking.NombreAccion = "OnTestconexionClick";
                objEventTracking.NombreControl = "OnTestconexionClick";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                if (iDynamicService != null && listasHevd != null)
                {
                    IsLoading = true;
                    ProgressValue = 0;
                    StateHasChanged();

                    try
                    {
                        // 🔹 Iniciar la prueba de conexión en un Task separado
                        var connectionTask = iDynamicService.TestConnectionAsync(conexion);

                        // 🔥 Simular el progreso en intervalos de 500ms, pero limitándolo a 95% antes de que termine la conexión
                        while (!connectionTask.IsCompleted)
                        {
                            await Task.Delay(500); // Espera 500ms antes de aumentar
                            if (ProgressValue < 95)
                            {
                                ProgressValue += 5; // Aumenta en 5% cada 500ms hasta 95%
                                StateHasChanged();
                            }
                        }

                        // 🔥 Esperar el resultado de la prueba de conexión
                        bool isConnected = await connectionTask;

                        // 🔹 Asegurar que la barra llegue al 100% solo cuando la prueba de conexión termine
                        ProgressValue = 100;
                        StateHasChanged();

                        var toastMessage = new ToastMessage
                        {
                            Type = isConnected ? ToastType.Success : ToastType.Danger,
                            Title = "Mensaje de confirmación",
                            HelpText = $"{DateTime.Now}",
                            Message = isConnected ? "Conexión satisfactoria" : "Conexión fallida",
                        };

                        messages.Add(toastMessage);
                        StateHasChanged();

                        // Mantener el mensaje visible por más tiempo (5 segundos)
                        await Task.Delay(5000);

                        // Remover mensaje después de la espera
                        messages.Remove(toastMessage);
                        InvokeAsync(StateHasChanged);

                        return isConnected;
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
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en OnTestconexionClick: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// OnMigrarClick: Migrar los datos de la ONA desde el servidor externo.
        /// </summary>
        /// <param name="conexion">
        /// <returns> devuelve un valor true o false dependiendo de la migracion</returns>
        private async Task<bool> OnMigrarClick(int conexion)
        {
            objEventTracking.CodigoHomologacionMenu = "/conexion";
            objEventTracking.NombreAccion = "OnMigrarClick";
            objEventTracking.NombreControl = "OnMigrarClick";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (iDynamicService != null && listasHevd != null)
            {
                IsLoading = true;
                ProgressValue = 0;
                StateHasChanged();

                try
                {
                    // 🔹 Iniciar la migración en un Task separado para permitir la actualización de la UI
                    var migrationTask = iDynamicService.MigrarConexionAsync(conexion);

                    // 🔥 Simular el progreso en intervalos de 500ms, pero limitándolo a 95% antes de que termine la migración
                    while (!migrationTask.IsCompleted)
                    {
                        await Task.Delay(500); // Espera 500ms antes de aumentar
                        if (ProgressValue < 95)
                        {
                            ProgressValue += 5; // Aumenta en 5% cada 500ms hasta 95%
                            StateHasChanged();
                        }
                    }

                    // 🔥 Esperar el resultado de la migración
                    bool migracion = await migrationTask;

                    // 🔹 Asegurar que la barra llegue al 100% solo cuando termine la migración
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

                    // Mantener el mensaje visible por más tiempo (7 segundos)
                    await Task.Delay(7000);

                    // Remover mensaje después de la espera
                    messages.Remove(toastMessage);
                    InvokeAsync(StateHasChanged);

                    return migracion;
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
            objEventTracking.CodigoHomologacionMenu = "/conexion";
            objEventTracking.NombreAccion = "ConfirmDelete";
            objEventTracking.NombreControl = "btnEliminar";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
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