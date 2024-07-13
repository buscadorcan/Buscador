using BlazorBootstrap;
using SharedApp.Models.Dtos;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace ClientApp.Pages.Administracion.Validacion
{
    public partial class Listado
    {
        [Inject]
        public ICatalogosService? iCatalogosService { get; set; }
        [Inject]
        private IHomologacionEsquemaService? iHomologacionEsquemaService { get; set; }
        [Inject]
        private IBusquedaService? servicio { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }
        private Grid<EsquemaVista>? grid = default;
        private List<HomologacionDto>? listaOrganizaciones = new List<HomologacionDto>();
        private List<HomologacionEsquemaDto>? listaHomologacionEsquemas = new List<HomologacionEsquemaDto>();
        private List<HomologacionDto>? listaHomologacions = new List<HomologacionDto>();
        private List<PropiedadesTablaDto> propiedadesVista = new List<PropiedadesTablaDto>();
        private List<HomologacionDto> Columnas = new List<HomologacionDto>();
        private HomologacionEsquemaDto? esquemaSelected;
        private HomologacionDto? organizacionSelected;
        private List<EsquemaVista> listasHevd = new List<EsquemaVista>();
        protected override async Task OnInitializedAsync()
        {
            if (iCatalogosService != null && iHomologacionEsquemaService != null)
            {
                listaOrganizaciones = await iCatalogosService.GetHomologacionDetalleAsync<List<HomologacionDto>>("filtro_detalles", 3);
                listaHomologacionEsquemas = await iHomologacionEsquemaService.GetHomologacionEsquemasAsync();
            }
        }
        private async Task CambiarSeleccionOrganizacion(HomologacionDto _organizacionSelected)
        {
            organizacionSelected = _organizacionSelected;
            esquemaSelected = null;

            listasHevd = new List<EsquemaVista>();
            if (grid != null)
                await grid.RefreshDataAsync();
        }
        private async Task CambiarSeleccionEsquema(HomologacionEsquemaDto _esquemaSelected)
        {
            esquemaSelected = _esquemaSelected;
            var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdHomologacionEsquema);
            Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson).OrderBy(c => c.MostrarWebOrden).ToList();

            listasHevd = new List<EsquemaVista>();

            foreach(var c in Columnas)
            {
                listasHevd.Add(new EsquemaVista {
                    NombreEsquema = c.NombreHomologado,
                    IsValid = false
                });
            }

            if (grid != null)
                await grid.RefreshDataAsync();
        }
        private async Task<AutoCompleteDataProviderResult<PropiedadesTablaDto>> NombreColumnaDataProvider(AutoCompleteDataProviderRequest<PropiedadesTablaDto> request)
        {
            return await Task.FromResult(request.ApplyTo(propiedadesVista.OrderBy(p => p.NombreColumna)));
        }
        private async Task<GridDataProviderResult<EsquemaVista>> EsquemaVistaDataProvider(GridDataProviderRequest<EsquemaVista> request)
        {
            return await Task.FromResult(request.ApplyTo(listasHevd));
        }
    }
}