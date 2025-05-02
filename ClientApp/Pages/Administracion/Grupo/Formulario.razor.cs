using BlazorBootstrap;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using Infractruture.Services;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Dtos;

namespace ClientApp.Pages.Administracion.Grupo
{
    /// <summary>
    /// Componente de formulario para la gesti�n de homologaciones dentro de un grupo.
    /// Permite registrar y actualizar registros de homologaci�n.
    /// </summary>
    public partial class Formulario
    {
        // Bot�n de guardar con animaci�n de carga
        private Button saveButton = default!;
        // Objeto que almacena la informaci�n de la homologaci�n a registrar o actualizar
        private HomologacionDto homologacion = new HomologacionDto();
        /// <summary>
        /// Servicio de homologaciones, utilizado para registrar y actualizar datos.
        /// </summary>
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        /// <summary>
        /// Servicio de navegaci�n entre p�ginas.
        /// </summary>
        [Inject]
        public NavigationManager? navigationManager { get; set; }

        /// <summary>
        /// ID del grupo de homologaciones, nulo si se est� creando un nuevo registro.
        /// </summary>
        [Parameter]
        public int? Id { get; set; }
        /// <summary>
        /// Servicio de notificaciones Toast.
        /// </summary>
        [Inject]
        public Infractruture.Services.ToastService? toastService { get; set; }
        /// <summary>
        /// Servicio de b�squeda y registro de eventos.
        /// </summary>
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        /// <summary>
        /// Servicio de almacenamiento local en el navegador.
        /// </summary>
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }

        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();

        // Lista de homologaciones asociadas al grupo
        private IEnumerable<HomologacionDto>? lista = new List<HomologacionDto>();

        /// <summary>
        /// M�todo asincr�nico que se ejecuta al inicializar el componente.
        /// Carga la informaci�n de la homologaci�n si se est� editando.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/editar-grupos";
            objEventTracking.NombreAccion = "editar-grupos";
            objEventTracking.NombreControl = "OnInitializedAsync";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (Id > 0 && iHomologacionService != null) {
                homologacion = await iHomologacionService.GetHomologacionAsync(Id.Value);
            } else {
                homologacion.InfoExtraJson = "{}";
            }
        }

        /// <summary>
        /// M�todo invocable desde JavaScript para actualizar el orden de las homologaciones en la lista.
        /// </summary>
        /// <param name="sortedIds">Lista ordenada de IDs de homologaciones.</param>
        [JSInvokable]
        public async Task OnDragEnd(string[] sortedIds)
        {
            var tempList = new List<HomologacionDto>();
            for (int i = 0; i < sortedIds.Length; i++)
            {
                var homo = lista?.FirstOrDefault(h => h.IdHomologacion == int.Parse(sortedIds[i]));
                if (homo != null)
                {
                    homo.MostrarWebOrden = i + 1;
                    tempList.Add(homo);
                }
            }
            lista = tempList;
            await Task.CompletedTask;
        }

        /// <summary>
        /// M�todo que guarda o actualiza una homologaci�n en la base de datos.
        /// </summary>
        private async Task GuardarHomologacion()
        {
            objEventTracking.CodigoHomologacionMenu = "/editar-grupos";
            objEventTracking.NombreAccion = "GuardarHomologacion";
            objEventTracking.NombreControl = "btnGuardar";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            saveButton.ShowLoading("Guardando...");

            if (iHomologacionService != null)
            {
                var result = await iHomologacionService.RegistrarOActualizar(homologacion);
                if (result.registroCorrecto)
                {
                    toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                    navigationManager?.NavigateTo("/grupos");
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Debe llenar todos los campos");
                }
            }

            saveButton.HideLoading();
        }
        private void Regresar()
        {
            NavigationManager.NavigateTo("/grupos");
        }
    }
}
