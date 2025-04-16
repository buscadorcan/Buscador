using BlazorBootstrap;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using Infractruture.Services;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Dtos;

namespace ClientApp.Pages.Administracion.Grupo
{
    /// <summary>
    /// P�gina de listado de homologaciones dentro de un grupo.
    /// Permite visualizar, paginar, eliminar y reordenar los registros de homologaciones.
    /// </summary>
    public partial class Listado
    {
        // Componente de la grilla para mostrar los registros
        private Grid<HomologacionDto>? grid;
        // Lista de homologaciones obtenidas desde el servicio
        private List<HomologacionDto>? listaHomologacions = new List<HomologacionDto>();
        /// <summary>
        /// Servicio de cat�logos, utilizado para obtener la lista de homologaciones.
        /// </summary>
        [Inject]
        private ICatalogosService? iCatalogosService { get; set; }
        /// <summary>
        /// Servicio de homologaci�n, utilizado para gestionar eliminaciones y actualizaciones.
        /// </summary>
        [Inject]
        private IHomologacionService? iHomologacionService { get; set; }
        /// <summary>
        /// Evento que se dispara cuando los datos han sido cargados.
        /// </summary>
        public event Action? DataLoaded;
        /// <summary>
        /// Servicio de b�squeda y registro de eventos.
        /// </summary>
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();
        /// <summary>
        /// Servicio de almacenamiento local en el navegador.
        /// </summary>
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        // Par�metros para la paginaci�n
        private int PageSize = 10; // Cantidad de registros por p�gina
        private int CurrentPage = 1;

        /// <summary>
        /// Obtiene los elementos paginados para la grilla.
        /// </summary>
        private IEnumerable<HomologacionDto> PaginatedItems => listaHomologacions
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        /// <summary>
        /// Calcula el n�mero total de p�ginas basado en el n�mero de registros.
        /// </summary>
        private int TotalPages => listaHomologacions.Count > 0 ? (int)Math.Ceiling((double)listaHomologacions.Count / PageSize) : 1;

        /// <summary>
        /// Indica si se puede retroceder a la p�gina anterior.
        /// </summary>
        private bool CanGoPrevious => CurrentPage > 1;

        /// <summary>
        /// Indica si se puede avanzar a la siguiente p�gina.
        /// </summary>
        private bool CanGoNext => CurrentPage < TotalPages;

        /// <summary>
        /// Cambia a la p�gina anterior en la paginaci�n.
        /// </summary>
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        /// <summary>
        /// Cambia a la siguiente p�gina en la paginaci�n.
        /// </summary>
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// M�todo asincr�nico que se ejecuta al inicializar el componente.
        /// Carga la lista de homologaciones desde el servicio de cat�logos.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/grupos";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "grupos";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (iCatalogosService != null)
            {
                listaHomologacions = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("grupos");
            }
            // Ajusta la paginaci�n si la lista est� vac�a o cambia
            if (listaHomologacions.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }

            //DataLoaded += async () =>
            //{
            //    if (listaHomologacions != null && JSRuntime != null)
            //    {
            //        await Task.Delay(2000);
            //        await JSRuntime.InvokeVoidAsync("initSortable", DotNetObjectReference.Create(this));
            //    }
            //};
        }

        /// <summary>
        /// Proveedor de datos para la grilla de homologaciones.
        /// </summary>
        private async Task<GridDataProviderResult<HomologacionDto>> HomologacionDataProvider(GridDataProviderRequest<HomologacionDto> request)
        {
            if (iCatalogosService != null)
            {
                listaHomologacions = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("grupos");
            }

            DataLoaded?.Invoke();

            return await Task.FromResult(request.ApplyTo(listaHomologacions ?? []));
        }
        /// <summary>
        /// Elimina una homologaci�n de la lista.
        /// </summary>
        /// <param name="IdHomologacion">ID de la homologaci�n a eliminar.</param>
        private async Task OnDeleteClick(int IdHomologacion)
        {
            if (iHomologacionService != null)
            {
                var respuesta = await iHomologacionService.EliminarHomologacion(IdHomologacion);
                if (respuesta.registroCorrecto && grid != null) {
                    await grid.RefreshDataAsync();
                }
            }
        }

        /// <summary>
        /// M�todo invocable desde JavaScript para actualizar el orden de las homologaciones.
        /// </summary>
        /// <param name="sortedIds">Lista ordenada de IDs de homologaciones.</param>

        [JSInvokable]
        public async Task OnDragEnd(string[] sortedIds)
        {
            if (listaHomologacions != null)
            {
                // Actualiza el orden en la lista local
                var ordenados = new List<HomologacionDto>();
                for (int i = 0; i < sortedIds.Length; i++)
                {
                    HomologacionDto? homo = listaHomologacions.FirstOrDefault(h => h.IdHomologacion == int.Parse(sortedIds[i]));
                    if (homo != null)
                    {
                        homo.MostrarWebOrden = i + 1; // Actualiza el orden en memoria
                        ordenados.Add(homo);
                        if (iHomologacionService != null)
                        {
                            await iHomologacionService.RegistrarOActualizar(homo); // Actualiza en el backend
                        }
                    }
                }

                // Reemplaza la lista original con la lista ordenada
                listaHomologacions = ordenados;

                // Refresca el grid
                if (grid != null)
                {
                    await grid.RefreshDataAsync();
                }
            }
        }

    }
}