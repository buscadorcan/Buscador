using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Esquemas
{
    public partial class Formulario
    {
        private string? homologacionName;
        private Button saveButton = default!;
        public event Action? DataLoaded;
        private HomologacionEsquema homologacionEsquema = new HomologacionEsquema();
        [Inject]
        public IHomologacionEsquemaService? iHomologacionEsquemaService { get; set; }
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        [Inject]
        public ICatalogosService? iCatalogosService { get; set; }
        private List<HomologacionDto>? listaVwHomologacion;

        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }
        private IEnumerable<HomologacionDto>? lista = new List<HomologacionDto>();
        protected override async Task OnInitializedAsync()
        {
            if (iCatalogosService != null)
            {
                listaVwHomologacion = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("dimension");

                if (Id > 0 && iHomologacionEsquemaService != null) {
                    homologacionEsquema = await iHomologacionEsquemaService.GetHomologacionEsquemaAsync(Id.Value);

                    lista = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson);
                } else {
                    homologacionEsquema.EsquemaJson = "{}";
                }
            }
            DataLoaded += async () => {
                if (!(lista is null) && JSRuntime != null) {
                    await Task.Delay(2000);
                    await JSRuntime.InvokeVoidAsync("initSortable", DotNetObjectReference.Create(this));
                }
            };

            DataLoaded?.Invoke();
        }
        private async Task GuardarHomologacionEsquema()
        {
            saveButton.ShowLoading("Guardando...");

            homologacionEsquema.EsquemaJson = JsonConvert.SerializeObject(lista);

            var result = await iHomologacionEsquemaService.RegistrarOActualizar(homologacionEsquema);
            if (result.registroCorrecto)
            {
                // guardar las nuevas posiciones
                if (lista != null)
                {
                    foreach (var n in lista) {
                        if (iHomologacionService != null)
                            await iHomologacionService.RegistrarOActualizar(new HomologacionDto() {
                                IdHomologacion = n.IdHomologacion,
                                MostrarWebOrden = n.MostrarWebOrden
                            });
                    }

                    toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                    navigationManager?.NavigateTo("/esquemas");
                }
            }
            else
            {
                toastService?.CreateToastMessage(ToastType.Danger, "Debe llenar todos los campos");
            }

            saveButton.HideLoading();
        }
        private void EliminarElemento(int elemento)
        {
            lista = lista.Where(c => c.IdHomologacion != elemento).ToList();
        }
        [JSInvokable]
        public async Task OnDragEnd(string[] sortedIds)
        {
            var tempList = new List<HomologacionDto>();
            for (int i = 0; i < sortedIds.Length; i += 1)
            {
                HomologacionDto homo = lista.FirstOrDefault(h => h.IdHomologacion == Int32.Parse(sortedIds[i]));
                homo.MostrarWebOrden = i + 1;
                tempList.Add(homo);
            }
            lista = tempList;
            await Task.CompletedTask;
        }
        private async Task<AutoCompleteDataProviderResult<HomologacionDto>> VwHomologacionDataProvider(AutoCompleteDataProviderRequest<HomologacionDto> request)
        {
            if (listaVwHomologacion is null)
                listaVwHomologacion = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("dimension");

            return await Task.FromResult(request.ApplyTo(listaVwHomologacion.OrderBy(vmH => vmH.MostrarWebOrden)));
        }
        private void OnAutoCompleteChanged(HomologacionDto _vwHomologacionSelected)
        {
            _vwHomologacionSelected.MostrarWebOrden = lista.Count();
            lista = lista.Append(_vwHomologacionSelected).ToList();
        }
    }
}
