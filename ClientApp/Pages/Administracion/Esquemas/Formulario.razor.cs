using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Esquemas
{
    public partial class Formulario
    {
        private EditContext? editContext;
        private Button saveButton = default!;
        public event Action? DataLoaded;
        
        [Parameter]
        public int? Id { get; set; }
        
        [Inject]
        public IHomologacionEsquemaService? HomologacionEsquemaService { get; set; }
        
        [Inject]
        public IBusquedaService? BusquedaService { get; set; }
        
        [Inject]
        public IHomologacionService? HomologacionService { get; set; }
        
        [Inject]
        public ICatalogosService? CatalogosService { get; set; }
        
        [Inject]
        public NavigationManager? NavigationManager { get; set; }
        
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        
        [Inject]
        public Services.ToastService? ToastService { get; set; }
        
        private string? homologacionName;
        private HomologacionEsquemaDto? homologacionEsquema = new();
        private List<HomologacionDto>? listaVwHomologacion;
        private IEnumerable<HomologacionDto>? lista = new List<HomologacionDto>();

        protected override async Task OnInitializedAsync()
        {
            if (homologacionEsquema != null)
                editContext = new EditContext(homologacionEsquema);

            if (CatalogosService != null)
                listaVwHomologacion = await CatalogosService.GetHomologacionAsync<List<HomologacionDto>>("dimension");

            if (Id > 0 && HomologacionEsquemaService != null && HomologacionEsquemaService != null)
            {
                homologacionEsquema = await HomologacionEsquemaService.GetHomologacionEsquemaAsync(Id.Value);
                if (homologacionEsquema != null)
                {
                    UpdateEditContext(homologacionEsquema);
                    lista = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson ?? "[]");
                }
            }
            else if (homologacionEsquema != null)
            {
                homologacionEsquema.EsquemaJson = "[]";
            }

            DataLoaded += async () =>
            {
                if (lista != null && JSRuntime != null)
                {
                    await Task.Delay(2000);
                    await JSRuntime.InvokeVoidAsync("initSortable", DotNetObjectReference.Create(this));
                }
            };

            DataLoaded?.Invoke();
        }

        private void UpdateEditContext(HomologacionEsquemaDto newModel)
        {
            editContext = new EditContext(newModel);
        }

        private async Task GuardarHomologacionEsquema()
        {
            saveButton.ShowLoading("Guardando...");

            if (homologacionEsquema != null && editContext != null && editContext.Validate())
            {
                homologacionEsquema.EsquemaJson = JsonConvert.SerializeObject(lista?.Select(s => new { s.IdHomologacion }));

                if (HomologacionEsquemaService != null)
                {
                    var result = await HomologacionEsquemaService.RegistrarOActualizar(homologacionEsquema);
                    if (result.registroCorrecto)
                    {
                        if (lista != null)
                        {
                            foreach (var n in lista)
                            {
                                if (HomologacionService != null)
                                    await HomologacionService.RegistrarOActualizar(new HomologacionDto { IdHomologacion = n.IdHomologacion, MostrarWebOrden = n.MostrarWebOrden });
                            }

                            ToastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                            NavigationManager?.NavigateTo("/esquemas");
                        }
                    }
                    else
                    {
                        ToastService?.CreateToastMessage(ToastType.Danger, "Debe llenar todos los campos");
                    }
                }
            }
            saveButton.HideLoading();
        }

        private void EliminarElemento(int elemento)
        {
            lista = lista?.Where(c => c.IdHomologacion != elemento).ToList();
        }

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
        private async Task<AutoCompleteDataProviderResult<HomologacionDto>> VwHomologacionDataProvider(AutoCompleteDataProviderRequest<HomologacionDto> request)
        {
            if (listaVwHomologacion == null && CatalogosService != null)
                listaVwHomologacion = await CatalogosService.GetHomologacionAsync<List<HomologacionDto>>("dimension");

            return await Task.FromResult(request.ApplyTo((listaVwHomologacion ?? new List<HomologacionDto>()).OrderBy(vmH => vmH.MostrarWebOrden)));
        }
        private void OnAutoCompleteChanged(HomologacionDto vwHomologacionSelected)
        {
            vwHomologacionSelected.MostrarWebOrden = lista?.Count() ?? 0;
            lista = lista?.Append(vwHomologacionSelected).ToList();
        }
    }
}