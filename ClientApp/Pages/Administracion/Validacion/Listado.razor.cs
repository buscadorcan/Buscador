using BlazorBootstrap;
using SharedApp.Models.Dtos;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using System.Text;
using System.Net.Http;
using ClientApp.Services;

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
        [Inject]
        private HttpClient _httpClient { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        private Button saveButton = default!;
        private Button validateButton = default!;
        
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
            nombreSugerido = "";
            listaEsquemasOna = await iEsquemaService.GetEsquemaByOnaAsync(onaSelected.IdONA);

            listasHevd = new List<EsquemaVistaDto>();
            if (grid != null)
                await grid.RefreshDataAsync();
        }
        
        private async Task CambiarSeleccionEsquema(EsquemaVistaOnaDto _esquemaSelected)
        {
            esquemaSelected = _esquemaSelected;
            nombreSugerido = esquemaSelected.VistaOrigen;

            var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
            var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson).OrderBy(c => c.MostrarWebOrden).ToList();

            listasHevd = new List<EsquemaVistaDto>();

            var vistas = await iDynamicService.GetListaValidacionEsquema(onaSelected.IdONA, esquemaSelected.IdEsquemaVista);

            foreach (var c in Columnas)
            {
                var count = vistas.Count(n => n.NombreEsquema != null && n.NombreEsquema.Equals(c.NombreHomologado));
                listasHevd.Add(new EsquemaVistaDto
                {
                    NombreEsquema = c.NombreHomologado,
                    NombreVista = count > 0 ? c.NombreHomologado : "",
                    IsValid = count > 0
                });
            }

            if (grid != null)
                await grid.RefreshDataAsync();
        }

        private async Task<GridDataProviderResult<EsquemaVistaDto>> EsquemaVistaDataProvider(GridDataProviderRequest<EsquemaVistaDto> request)
        {
            return await Task.FromResult(request.ApplyTo(listasHevd));
        }

        private async Task GuardarCambios()
        {
            saveButton.ShowLoading("Guardando...");
            if (onaSelected == null || esquemaSelected == null || string.IsNullOrEmpty(nombreSugerido))
            {
                saveButton.HideLoading();
                return;
            }

            var esquemaRegistro = new EsquemaVistaValidacionDto
            {
                IdEsquemaVista = esquemaSelected.IdEsquemaVista,
                IdOna = onaSelected.IdONA,
                IdEsquema = esquemaSelected.IdEsquema,
                VistaOrigen = nombreSugerido,
                Estado = "A"
            };

            var resultado = await iEsquemaService.GuardarEsquemaVistaValidacionAsync(esquemaRegistro);

            if (resultado != null && resultado.registroCorrecto)
            {
                toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                navigationManager?.NavigateTo("/validacion");
            }
            else
            {
                saveButton.HideLoading();
            }
            saveButton.HideLoading();
        }

        private async Task ValidarDatos()
        {
            validateButton.ShowLoading("Validando...");

            //var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
            //var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson).OrderBy(c => c.MostrarWebOrden).ToList();

            //listasHevd = new List<EsquemaVistaDto>();

            ////var vistas = await iDynamicService.GetProperties(organizacionSelected.CodigoHomologacion, esquemaSelected.EsquemaVista.Trim());
            ////var vistas = await iDynamicService.GetProperties(onaSelected.IdONA, esquemaSelected.EsquemaVista.Trim());

            
            //foreach (var c in Columnas)
            //{
            //    var count = vistas.Count(n => n.NombreColumna != null && n.NombreColumna.Equals(c.NombreHomologado));
            //    listasHevd.Add(new EsquemaVistaDto
            //    {
            //        NombreEsquema = c.NombreHomologado,
            //        NombreVista = count > 0 ? c.NombreHomologado : "",
            //        IsValid = count > 0
            //    });
            //}

            //if (grid != null)
            //    await grid.RefreshDataAsync();
            return;
        }
    }
}
