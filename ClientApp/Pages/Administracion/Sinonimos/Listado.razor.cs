using BlazorBootstrap;
using SharedApp.Models.Dtos;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace ClientApp.Pages.Administracion.Sinonimos
{
    public partial class Listado
    {
        // [Inject]
        // public IHomologacionService? iHomologacionService { get; set; }
        // [Inject]
        // private IHomologacionEsquemaService? iHomologacionEsquemaService { get; set; }
        // [Inject]
        // private IBusquedaService? servicio { get; set; }
        // [Inject]
        // public IDynamicService? iDynamicService { get; set; }
        // private Grid<EsquemaVista>? grid = default;
        // private List<HomologacionDto>? listaOrganizaciones = new List<HomologacionDto>();
        // private List<HomologacionEsquemaDto>? listaHomologacionEsquemas = new List<HomologacionEsquemaDto>();
        // private HomologacionEsquemaDto? esquemaSelected;
        // private HomologacionDto? organizacionSelected;
        // private List<EsquemaVista> listasHevd = new List<EsquemaVista>();
        // private List<string> NombresVistas { get; set; }
        protected override async Task OnInitializedAsync()
        {
            // if (iHomologacionService != null && iHomologacionEsquemaService != null)
            // {
            //     listaOrganizaciones = await iHomologacionService.GetHomologacionsAsync(3);
            //     listaHomologacionEsquemas = await iHomologacionEsquemaService.GetHomologacionEsquemasAsync();
            // }
        }
        // private async Task CambiarSeleccionOrganizacion(HomologacionDto _organizacionSelected)
        // {
            // organizacionSelected = _organizacionSelected;
            // esquemaSelected = null;

            // NombresVistas = await iDynamicService.GetViewNames(organizacionSelected.IdHomologacion);

            // listasHevd = new List<EsquemaVista>();
            // if (grid != null)
            //     await grid.RefreshDataAsync();
        // }
        // private async Task CambiarSeleccionEsquema(HomologacionEsquemaDto _esquemaSelected)
        // {
            // esquemaSelected = _esquemaSelected;
            // var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdHomologacionEsquema);
            // var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson).OrderBy(c => c.MostrarWebOrden).ToList();

            // listasHevd = new List<EsquemaVista>();

            // var vistas = await iDynamicService.GetProperties(organizacionSelected.IdHomologacion, esquemaSelected.VistaNombre.Trim());

            // foreach(var c in Columnas)
            // {
            //     var count = vistas.Count(n => n.NombreColumna != null && n.NombreColumna.Equals(c.NombreHomologado));
            //     listasHevd.Add(new EsquemaVista {
            //         NombreEsquema = c.NombreHomologado,
            //         NombreVista = count > 0 ? c.NombreHomologado : "",
            //         IsValid = count > 0
            //     });
            // }

            // if (grid != null)
            //     await grid.RefreshDataAsync();
        // }
        // private async Task<GridDataProviderResult<EsquemaVista>> EsquemaVistaDataProvider(GridDataProviderRequest<EsquemaVista> request)
        // {
            // return await Task.FromResult(request.ApplyTo(listasHevd));
        // }
    }
}