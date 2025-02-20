using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.CamposHomologacion
{
    /// <summary>
    /// Page: Formulario de Esquema
    /// Concepto: Formulario que registra un nuevo campo de homologacion o lo edita.
    /// </summary>
    public partial class Formulario
    {
        private Button saveButton = default!;
        private HomologacionDto homologacion = new HomologacionDto();
        private HomologacionDto homologacionGrupo = new HomologacionDto();
        private List<VwFiltroDto> filtros = new();
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        [Inject]
        public ICatalogosService? iCatalogoService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Parameter]
        public int? IdPadre { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        /// <summary>
        /// Page: Formulario campos de homologacion
        /// Concepto: Metodo que inicializa la clase campos de homologacion.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            filtros = await iCatalogoService.GetFiltrosAsync();
            homologacionGrupo = await iHomologacionService.GetHomologacionAsync((int) IdPadre);
            if (Id > 0) {
                homologacion = await iHomologacionService.GetHomologacionAsync(Id.Value);
            } else {
                homologacion.IdHomologacionGrupo = IdPadre;
                homologacion.InfoExtraJson = "{}";
                homologacion.MascaraDato = "TEXTO";
                homologacion.CodigoHomologacion = "";
                homologacion.SiNoHayDato = "";
            }
        }
        /// <summary>
        /// Page: Formulario campos de homologacion
        /// GuardarHomologacion: Metodo que registra / actualiza los campos de homologacion.
        /// </summary>
        private async Task GuardarHomologacion()
        {
            objEventTracking.NombrePagina = "Actualizar / Registrar";
            objEventTracking.NombreAccion = "GuardarHomologacion";
            objEventTracking.NombreControl = "GuardarHomologacion";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Local) + ' ' + iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Apellido_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            saveButton.ShowLoading("Guardando...");

            var result = await iHomologacionService.RegistrarOActualizar(homologacion);
            if (result.registroCorrecto)
            {
                toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                navigationManager?.NavigateTo("/campos-homologacion");
            }
            else
            {
                toastService?.CreateToastMessage(ToastType.Danger, "Debe llenar todos los campos");
            }

            saveButton.HideLoading();
        }

        /// <summary>
        /// Page: Formulario campos de homologacion
        /// OnAutoCompleteChanged: Metodo que hace el autocomplete en el cambio del campo.
        /// </summary>
        private void OnAutoCompleteChanged(string mascaraDato) {
            homologacion.MascaraDato = mascaraDato;
        }

        /// <summary>
        /// Page: Formulario campos de homologacion
        /// ActualizarFiltro: Actualiza el filtro de los campos de homologacion.
        /// </summary>
        private void ActualizarFiltro(ChangeEventArgs e)
        {
            // Obtener el valor seleccionado
            var selectedValue = e.Value?.ToString();

            // Si el valor es "Sin Filtro" (vacío), asignar null a la variable
            if (string.IsNullOrEmpty(selectedValue))
            {
                homologacion.IdHomologacionFiltro = null;
            }
            else
            {
                // Convertir el valor a int, si es válido
                homologacion.IdHomologacionFiltro = int.TryParse(selectedValue, out var valor) ? valor : null;
            }
        }

        /// <summary>
        /// Page: Formulario campos de homologacion
        /// isIndexar: Propiedad booleana que hace la indexacion del campo vinculada al Switch
        /// </summary>
        private bool isIndexar // Propiedad booleana vinculada al Switch
        {
            get => homologacion.Indexar == "S"; // Convertir "S" a true
            set => homologacion.Indexar = value ? "S" : "N"; // Convertir true a "S"
        }

        /// <summary>
        /// Page: Formulario campos de homologacion
        /// isMostrar: Propiedad booleana que registra el campo mostrar vinculada al Switch
        /// </summary>
        private bool isMostrar // Propiedad booleana vinculada al Switch
        {
            get => homologacion.Mostrar == "S"; // Convertir "S" a true
            set => homologacion.Mostrar = value ? "S" : "N"; // Convertir true a "S"
        }
    }
}