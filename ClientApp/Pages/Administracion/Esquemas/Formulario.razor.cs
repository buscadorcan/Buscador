using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Esquemas
{
    /// <summary>
    /// Componente de formulario para la gestión de esquemas de homologación.
    /// Permite crear y actualizar esquemas, asociando homologaciones.
    /// </summary>
    public partial class Formulario
    {
        // Contexto de edición para validación de formularios
        private EditContext? editContext;
        // Botón con animación de carga
        private Button saveButton = default!;
        // Evento que se dispara cuando los datos han sido cargados
        public event Action? DataLoaded;
        /// <summary>
        /// ID del esquema, nulo si es un nuevo registro.
        /// </summary>
        [Parameter]
        public int? Id { get; set; }
        // Servicios inyectados
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
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }

        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        // Nombre de la homologación seleccionada
        private string? homologacionName;
        // Objeto del esquema a registrar/actualizar
        private EsquemaDto? Esquema = new();
        // Lista de homologaciones disponibles
        private List<HomologacionDto>? listaVwHomologacion;
        // Lista de homologaciones asociadas al esquema
        private IEnumerable<HomologacionDto>? lista = new List<HomologacionDto>();


        /// <summary>
        /// Método asincrónico que inicializa la página cargando la lista de homologaciones y el esquema si se edita.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {

            if (Esquema != null)
                editContext = new EditContext(Esquema);

            if (HomologacionService != null)
                listaVwHomologacion = await HomologacionService.GetHomologacionsAsync();

            if (Id > 0 && EsquemaService != null && EsquemaService != null)
            {
                objEventTracking.CodigoHomologacionMenu = "/nuevo-esquema";
                objEventTracking.NombreAccion = "OnInitializedAsync";
                objEventTracking.NombreControl = "editar-esquema";
                objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

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

        /// <summary>
        /// Método que actualiza el contexto de edición del esquema.
        /// </summary>
        /// <param name="newModel">Modelo de esquema a actualizar.</param>
        private void UpdateEditContext(EsquemaDto newModel)
        {
            editContext = new EditContext(newModel);
        }

        /// <summary>
        /// Método que guarda o actualiza un esquema en la base de datos.
        /// </summary>
        private async Task GuardarEsquema()
        {
            objEventTracking.CodigoHomologacionMenu = "/editar-esquema";
            objEventTracking.NombreAccion = "GuardarEsquema";
            objEventTracking.NombreControl = "btnGuardar";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

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
                                    await HomologacionService.RegistrarOActualizar(new HomologacionDto { IdHomologacion = n.IdHomologacion, MostrarWebOrden = n.MostrarWebOrden  });
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

        /// <summary>
        /// Método que elimina un elemento de la lista de homologaciones del esquema.
        /// </summary>
        /// <param name="elemento">ID de la homologación a eliminar.</param>
        private void EliminarElemento(int elemento)
        {
            lista = lista?.Where(c => c.IdHomologacion != elemento).ToList();
        }

        /// <summary>
        /// Método invocable desde JavaScript para actualizar el orden de las filas en la lista de homologaciones.
        /// </summary>
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

        /// <summary>
        /// Método que filtra y proporciona datos para el componente AutoComplete.
        /// </summary>
        private async Task<AutoCompleteDataProviderResult<HomologacionDto>> VwHomologacionDataProvider(AutoCompleteDataProviderRequest<HomologacionDto> request)
        {
            // Si la lista aún no está cargada, obtén los datos.
            if (listaVwHomologacion == null && HomologacionService != null)
            {
                listaVwHomologacion = await HomologacionService.GetHomologacionsAsync();
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
                .Take(10) //como utilizar Top 10 en consulta SQL
                .ToList();

            return new AutoCompleteDataProviderResult<HomologacionDto>
            {
                Data = resultados,
                TotalCount = resultados.Count
            };
        }

        /// <summary>
        /// Maneja el evento cuando se selecciona un elemento en el AutoComplete.
        /// Agrega el elemento a la lista y le asigna el siguiente orden disponible.
        /// </summary>
        /// <param name="vwHomologacionSelected">Elemento seleccionado en el AutoComplete.</param>
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