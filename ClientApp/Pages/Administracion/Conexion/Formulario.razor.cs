using BlazorBootstrap;
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
        public NavigationManager? navigationManager { get; set; }
        private ConexionDto conexion = new ConexionDto();
        [Inject]
        public ICatalogosService? iCatalogosService { get; set; }
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        private List<HomologacionDto>? listaOrganizaciones = default;
        private string? homologacionName;
        private List<HomologacionDto>? listaVwHomologacion;
        private IEnumerable<HomologacionDto>? lista = new List<HomologacionDto>();
        protected override async Task OnInitializedAsync()
        {
            if (iHomologacionService != null)
            {
                listaOrganizaciones = await iHomologacionService.GetHomologacionsAsync(3);
            }
            if (listaVwHomologacion == null && iCatalogosService != null)
                listaVwHomologacion = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("dimension");

            if (Id > 0 && service != null)
            {
                conexion = await service.GetConexionAsync(Id.Value);
                try
                {
                    var ids = JsonConvert.DeserializeObject<List<int>>(conexion.Filtros ?? "[]");
                    foreach (var item in ids ?? [])
                    {
                        var h = listaVwHomologacion?.FirstOrDefault(c => c.IdHomologacion == item);
                        if (h != null)
                        {
                            lista = lista?.Append(h).ToList();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                }
            } else
            {
                conexion.Filtros = "[]";
                conexion.Migrar = "N";
            }
        }
        private async Task RegistrarConexion()
        {
            saveButton.ShowLoading("Guardando...");

            if (service != null)
            {
                var idHomologaciones = lista?.Select(s => s.IdHomologacion).ToList();
                conexion.Filtros = JsonConvert.SerializeObject(idHomologaciones);

                var result = await service.RegistrarOActualizar(conexion);
                if (result.registroCorrecto)
                {
                    // toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                    navigationManager?.NavigateTo("/conexion");
                }
                else
                {
                    // toastService?.CreateToastMessage(ToastType.Danger, "Error al registrar en el servidor");
                }
            }

            saveButton.HideLoading();
        }
        private void CambiarSeleccionOrganizacion(int _organizacionSelected)
        {
            conexion.IdSistema = _organizacionSelected;
        }
        private void CambiarSeleccionMotor (string _motorBaseDatos)
        {
            conexion.MotorBaseDatos = _motorBaseDatos;
        }
        private async Task<AutoCompleteDataProviderResult<HomologacionDto>> VwHomologacionDataProvider(AutoCompleteDataProviderRequest<HomologacionDto> request)
        {
            if (listaVwHomologacion == null && iCatalogosService != null)
                listaVwHomologacion = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("dimension");

            return await Task.FromResult(request.ApplyTo((listaVwHomologacion ?? new List<HomologacionDto>()).OrderBy(vmH => vmH.MostrarWebOrden)));
        }
        private void OnAutoCompleteChanged(HomologacionDto vwHomologacionSelected)
        {
            lista = lista?.Append(vwHomologacionSelected).ToList();
        }
    }
}