using BlazorBootstrap;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Conexion
{
    public partial class Formulario
    {
        private Button saveButton = default!;
        [Parameter]
        public int? Id { get; set; }
        [Inject]
        private IConexionService? service { get; set; }
        [Inject]
        private IONAService? iOnaService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        private ONAConexionDto conexion = new ONAConexionDto();
        [Inject]
        public IHomologacionService? HomologacionService { get; set; }
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }

        [Inject]
        public Services.ToastService? ToastService { get; set; }

        private List<OnaDto>? listaOrganizaciones = default;
        private string? homologacionName;
        private List<HomologacionDto>? listaVwHomologacion;
        private IEnumerable<HomologacionDto>? lista = new List<HomologacionDto>();

        protected override async Task OnInitializedAsync()
        {
            if (iOnaService != null)
            {
                listaOrganizaciones = await iOnaService.GetONAsAsync();
            }

            if (listaVwHomologacion == null)
                listaVwHomologacion = await iHomologacionService.GetHomologacionsAsync();

            if (Id > 0 && service != null)
            {
                conexion = await service.GetConexionAsync(Id.GetValueOrDefault());
            }
            
           
        }
        private async Task RegistrarConexion()
        {
            saveButton.ShowLoading("Guardando...");

            if (service != null)
            {
                try
                {
                    var result = await service.RegistrarOActualizar(conexion);
                    if (result.registroCorrecto)
                    {
                        //Mensaje de éxito
                        ToastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                        navigationManager?.NavigateTo("/conexion");
                    }
                    else
                    {
                        // Mensaje de error
                        ToastService?.CreateToastMessage(ToastType.Danger, "Error al registrar en el servidor");
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine($"Error al registrar conexión: {ex.Message}");
                }
            }

            saveButton.HideLoading();
        }
        private void CambiarSeleccionOrganizacion(string _organizacionSelected)
        {
            var conexion = _organizacionSelected;
        }
        private void CambiarSeleccionMotor(ChangeEventArgs e)
        {
            conexion.BaseDatos = e.Value?.ToString();
            conexion.OrigenDatos = e.Value?.ToString();
        }
        private async Task<AutoCompleteDataProviderResult<HomologacionDto>> VwHomologacionDataProvider(AutoCompleteDataProviderRequest<HomologacionDto> request)
        {
            if (listaVwHomologacion == null)
                listaVwHomologacion = await HomologacionService.GetHomologacionsAsync();
            // Devuelve una lista vacía si no hay datos.
            if (listaVwHomologacion == null || !listaVwHomologacion.Any())
            {
                return new AutoCompleteDataProviderResult<HomologacionDto>
                {
                    Data = new List<HomologacionDto>(),
                    TotalCount = 0
                };
            }

            // Aplica el filtro ingresado en el AutoComplete.
            var filtro = request.Filter.Value.ToLowerInvariant();
            var resultados = listaVwHomologacion
                .Where(h => string.IsNullOrEmpty(filtro) ||
                            (h.MostrarWeb?.ToLowerInvariant().Contains(filtro) ?? false))
                .OrderBy(h => h.MostrarWebOrden)
                .Take(10) //como utilizar Top 10 en consulta SQL
                .ToList();

            return new AutoCompleteDataProviderResult<HomologacionDto>
            {
                Data = resultados,
                TotalCount = resultados.Count
            };
        }

        private bool isMigrar // Propiedad booleana vinculada al Switch
        {
            get => conexion.Migrar == "S"; // Convertir "S" a true
            set => conexion.Migrar = value ? "S" : "N"; // Convertir true a "S"
        }

        private void OnAutoCompleteChanged(HomologacionDto vwHomologacionSelected)
        {
            lista = lista?.Append(vwHomologacionSelected).ToList();
        }
        
    }
}