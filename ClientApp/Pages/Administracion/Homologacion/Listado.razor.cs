using BlazorBootstrap;
using ClientApp.Models;
using SharedApp.Models.Dtos;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace ClientApp.Pages.Administracion.Homologacion
{
    public partial class Listado
    {
        [Inject]
        public ICatalogosService? iCatalogosService { get; set; }
        // [Inject]
        // private IHomologacionService? homologacionRepository { get; set; }
        [Inject]
        private IHomologacionEsquemaService? iHomologacionEsquemaService { get; set; }
        [Inject]
        private IBusquedaService? servicio { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }
        // private Grid<HomologacionEsquemaVistaDto> grid;
        private Button saveButton = default!;
        private List<HomologacionDto>? listaOrganizaciones = new List<HomologacionDto>();
        private List<HomologacionEsquema>? listaHomologacionEsquemas = new List<HomologacionEsquema>();
        private List<HomologacionDto>? listaHomologacions = new List<HomologacionDto>();
        // private List<VistaDto>? listaVistas = new List<VistaDto>();
        private List<PropiedadesTablaDto> propiedadesVista = new List<PropiedadesTablaDto>();
        // private List<HomologacionEsquemaVistaDto> listasHevd = new List<HomologacionEsquemaVistaDto>();
        List<HomologacionDto> Columnas = new List<HomologacionDto>();
        private HomologacionEsquema esquemaSelected;
        private HomologacionDto organizacionSelected;
        // private VistaDto vistaSelected;
        protected override async Task OnInitializedAsync()
        {
            listaOrganizaciones = await iCatalogosService.GetHomologacionDetalleAsync<List<HomologacionDto>>("filtro_detalles", 3);
            listaHomologacionEsquemas = await iHomologacionEsquemaService.GetHomologacionEsquemasAsync();
        }
        private async Task CambiarSeleccionOrganizacion(HomologacionDto _organizacionSelected)
        {
            organizacionSelected = _organizacionSelected;
            // vistaSelected = null;
            esquemaSelected = null;
            // listaVistas = await vistaService.GetFindBySystemAsync(organizacionSelected.IdHomologacion);

            // listasHevd = new List<HomologacionEsquemaVistaDto>();
            // await grid.RefreshDataAsync();
            await Task.CompletedTask;
        }
        private async Task CambiarSeleccionEsquema(HomologacionEsquema _esquemaSelected)
        {
            esquemaSelected = _esquemaSelected;
            var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdHomologacionEsquema);
            Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson).OrderBy(c => c.MostrarWebOrden).ToList();

            // vistaSelected = null;
            // listasHevd = new List<HomologacionEsquemaVistaDto>();

            // await grid.RefreshDataAsync();
            await Task.CompletedTask;
        }
        // private async Task CambiarSeleccionVista(VistaDto _vistaSelected)
        // {
        //     vistaSelected = _vistaSelected;
        //     propiedadesVista = await vistaService.GetPropertiesAsync(vistaSelected.VistaNombre);
        //     listasHevd = new List<HomologacionEsquemaVistaDto>();

        //     await grid.RefreshDataAsync();
        //     await Task.CompletedTask;
        // }
        // private async Task<GridDataProviderResult<HomologacionEsquemaVistaDto>> HomologacionDataProvider(GridDataProviderRequest<HomologacionEsquemaVistaDto> request)
        // {
        //     if (Columnas.Count > 0 && esquemaSelected != null && vistaSelected != null) {
        //         // traer datos ya guardados
        //         var datos = await homologacionEsquemaVistaService.GetFindByVistaEsquemaAsync(vistaSelected.IdVista, esquemaSelected.IdHomologacionEsquema);

        //         foreach(var item in Columnas) {
        //             listasHevd.Add(new HomologacionEsquemaVistaDto {
        //                 IdHomologacion = item.IdHomologacion,
        //                 IdHomologacionEsquema = esquemaSelected.IdHomologacionEsquema,
        //                 NombreHomologado = item.NombreHomologado,
        //                 IdVista = vistaSelected.IdVista,
        //                 VistaColumna = datos.FirstOrDefault(
        //                     c => c.IdHomologacion == item.IdHomologacion &&
        //                         c.IdVista == vistaSelected.IdVista &&
        //                         c.IdHomologacionEsquema == esquemaSelected.IdHomologacionEsquema
        //                 )?.VistaColumna ?? propiedadesVista.FirstOrDefault(
        //                     c => c.NombreColumna.Equals(item.NombreHomologado)
        //                 )?.NombreColumna
        //             });
        //         }
        //     }

        //     return await Task.FromResult(request.ApplyTo(listasHevd));
        // }
        private async Task<AutoCompleteDataProviderResult<PropiedadesTablaDto>> NombreColumnaDataProvider(AutoCompleteDataProviderRequest<PropiedadesTablaDto> request)
        {
            return await Task.FromResult(request.ApplyTo(propiedadesVista.OrderBy(p => p.NombreColumna)));
        }
        // private async Task GuardarHomologacionEsquemaVista()
        // {
        //     saveButton.ShowLoading("Guardando...");

        //     var result = await homologacionEsquemaVistaService.Registrar(listasHevd);
        //     if (result.registroCorrecto)
        //     {
        //         toastService.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
        //     }
        //     else
        //     {
        //         toastService.CreateToastMessage(ToastType.Danger, "Debe llenar todos los campos");
        //     }

        //     saveButton.HideLoading();
        //     await Task.CompletedTask;
        // }
    }
}