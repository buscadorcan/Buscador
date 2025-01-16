using BlazorBootstrap;
using SharedApp.Models.Dtos;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using ClientApp.Services;
using Blazored.LocalStorage;
using ClientApp.Helpers;

namespace ClientApp.Pages.Administracion.Validacion
{
    public partial class Listado
    {
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        [Inject]
        private IHomologacionEsquemaService? iHomologacionEsquemaService { get; set; }
        [Inject]
        private IBusquedaService? servicio { get; set; }
        [Inject]
        public IDynamicService? iDynamicService { get; set; }
        [Inject]
        public IONAService? iONAservice { get; set; }
        [Inject]
        public IEsquemaService? iEsquemaService { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        private Grid<EsquemaVistaDto>? grid = default;
        private List<HomologacionDto>? listaOrganizaciones = new List<HomologacionDto>();
        private List<OnaDto>? listaONAs;
        private List<HomologacionEsquemaDto>? listaHomologacionEsquemas = new List<HomologacionEsquemaDto>();
        private EsquemaVistaOnaDto? esquemaSelected;
        private HomologacionDto? organizacionSelected;
        private OnaDto? onaSelected;
        private List<EsquemaVistaDto> listasHevd = new List<EsquemaVistaDto>();
        public string nombreSugerido = "";
        private List<EsquemaVistaOnaDto>? listaEsquemasOna = new List<EsquemaVistaOnaDto>();
        private List<string> NombresVistas { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            var rol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            bool accessRol = rol == "KEY_USER_CAN";
            if (accessRol)
            {
                if (listaONAs is null && iONAservice != null)
                {
                    await LoadONAs();
                }
            }
            else
            {
                await LoadONAs();
                listaONAs = listaONAs.Where(onas => onas.IdONA == onaPais).ToList();
            }


        }
        private async Task LoadONAs()
        {
            if (iONAservice != null)
            {
                listaONAs = await iONAservice.GetONAsAsync();
            }
        }
        private async Task CambiarSeleccionOna(OnaDto _onaSelected)
        {
            onaSelected = _onaSelected;
            esquemaSelected = null;

            listaEsquemasOna = await iEsquemaService.GetEsquemaByOnaAsync(onaSelected.IdONA);

            listasHevd = new List<EsquemaVistaDto>();
            if (grid != null)
                await grid.RefreshDataAsync();
        }
        private async Task CambiarSeleccionEsquema(EsquemaVistaOnaDto _esquemaSelected)
        {
            esquemaSelected = _esquemaSelected;
            nombreSugerido = esquemaSelected.EsquemaVista;
            
            listasHevd = new List<EsquemaVistaDto>();

            var columnas= await iDynamicService.GetListaValidacionEsquema(onaSelected.IdONA, esquemaSelected.IdEsquemaVista);

            if (columnas != null)
            {
                foreach (var c in columnas)
                {
                    var count = columnas.Count(n => n.NombreEsquema != null && n.NombreEsquema.Equals(c.NombreEsquema));
                    listasHevd.Add(new EsquemaVistaDto
                    {
                        NombreEsquema = c.NombreEsquema,
                        NombreVista = count > 0 ? c.NombreVista : "",
                        IsValid = count > 0
                    });
                }
            }

            if (grid != null)
                await grid.RefreshDataAsync();
        }
        private async Task<GridDataProviderResult<EsquemaVistaDto>> EsquemaVistaDataProvider(GridDataProviderRequest<EsquemaVistaDto> request)
        {
            return await Task.FromResult(request.ApplyTo(listasHevd));
        }
    }
}