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
        public IEsquemaService? EsquemaService { get; set; }
        
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
        private EsquemaDto? Esquema = new();
        private List<HomologacionDto>? listaVwHomologacion;
        private IEnumerable<HomologacionDto>? lista = new List<HomologacionDto>();

        protected override async Task OnInitializedAsync()
        {
            if (Esquema != null)
                editContext = new EditContext(Esquema);

            if (HomologacionService != null)
                listaVwHomologacion = await HomologacionService.GetHomologacionsAsync(1);

            if (Id > 0 && EsquemaService != null && EsquemaService != null)
            {
                Esquema = await EsquemaService.GetEsquemaAsync(Id.Value);
                if (Esquema != null)
                {
                    UpdateEditContext(Esquema);
                    lista = JsonConvert.DeserializeObject<List<HomologacionDto>>(Esquema.EsquemaJson ?? "[]");
                }
            }
            else if (Esquema != null)
            {
                Esquema.EsquemaJson = "[]";
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

        private void UpdateEditContext(EsquemaDto newModel)
        {
            editContext = new EditContext(newModel);
        }

        private async Task GuardarEsquema()
        {
            saveButton.ShowLoading("Guardando...");

            if (Esquema != null && editContext != null && editContext.Validate())
            {
                Esquema.EsquemaJson = JsonConvert.SerializeObject(lista?.Select(s => new { s.IdHomologacion }));

                if (EsquemaService != null)
                {
                    var result = await EsquemaService.RegistrarEsquemaActualizar(Esquema);
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
            // Si la lista aún no está cargada, obtén los datos.
            if (listaVwHomologacion == null && HomologacionService != null)
            {
                listaVwHomologacion = await HomologacionService.GetHomologacionsAsync(1);
            }

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
                .ToList();

            return new AutoCompleteDataProviderResult<HomologacionDto>
            {
                Data = resultados,
                TotalCount = resultados.Count
            };
        }
        private void OnAutoCompleteChanged(HomologacionDto vwHomologacionSelected)
        {
            if (vwHomologacionSelected != null)
            {
                vwHomologacionSelected.MostrarWebOrden = lista?.Count() ?? 0;
                lista = lista?.Append(vwHomologacionSelected).ToList();
            }
        }
    }
}