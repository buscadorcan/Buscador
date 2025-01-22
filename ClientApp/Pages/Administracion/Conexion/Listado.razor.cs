using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Conexion
{
    public partial class Listado
    {
        ToastsPlacement toastsPlacement = ToastsPlacement.TopRight;
        List<ToastMessage> messages = new();
        private Grid<ONAConexionDto>? grid;
        [Inject]
        private IConexionService? iConexionService { get; set; }
        [Inject]
        private IDynamicService? iDynamicService { get; set; }
        private List<ONAConexionDto>? listasHevd = null;
        private async Task<GridDataProviderResult<ONAConexionDto>> ConexionDtoDataProvider(GridDataProviderRequest<ONAConexionDto> request)
        {
            if (listasHevd == null && iConexionService != null)
            {
                listasHevd = await iConexionService.GetConexionsAsync();
            }
            return await Task.FromResult(request.ApplyTo(listasHevd ?? []));
        }
        private async Task OnDeleteClick(int IdONA)
        {
            if (iConexionService != null && listasHevd != null && grid != null)
            {
                var respuesta = await iConexionService.EliminarConexion(IdONA);
                if (respuesta.registroCorrecto)
                {
                    listasHevd = listasHevd.Where(c => c.IdONA != IdONA).ToList();
                    await grid.RefreshDataAsync();
                }
            }
        }

        private async Task<bool> OnTestconexionClick(int conexion)
        {
            if (iDynamicService != null && listasHevd != null && grid != null)
            {
                // Llamar al método del servicio para probar la conexión
                bool isConnected = await iDynamicService.TestConnectionAsync(conexion);
                var toastMessage = new ToastMessage
                {
                    Type = isConnected ? ToastType.Success : ToastType.Danger,
                    Title = "Mensaje de confirmación",
                    HelpText = $"{DateTime.Now}",
                    Message = isConnected ? "Conexión satisfactoria" : "Conexión fallida",
                };

                messages.Add(toastMessage);

                // Configurar el cierre automático después de 5 segundos
                _ = Task.Delay(5000).ContinueWith(_ =>
                {
                    messages.Remove(toastMessage);
                    InvokeAsync(StateHasChanged); // Actualizar la UI
                });

                if (isConnected)
                {
                    await grid.RefreshDataAsync();
                    return true; // Devuelve true si la conexión fue exitosa
                }
            }
            return false; // Devuelve false si algo falla
        }

        private async Task<bool> OnMigrarClick(int conexion)
        {
            if (iDynamicService != null && listasHevd != null && grid != null)
            {
                // Llamar al método del servicio para realizar la migración
                bool migracion = await iDynamicService.MigrarConexionAsync(conexion);

                var toastMessage = new ToastMessage
                {
                    Type = migracion ? ToastType.Success : ToastType.Danger,
                    Title = "Mensaje de confirmación",
                    HelpText = $"{DateTime.Now}",
                    Message = migracion ? "Conexión satisfactoria" : "Conexión fallida",
                };

                messages.Add(toastMessage);

                // Configurar el cierre automático después de 5 segundos
                _ = Task.Delay(5000).ContinueWith(_ =>
                {
                    messages.Remove(toastMessage);
                    InvokeAsync(StateHasChanged); // Actualizar la UI
                });

                if (migracion)
                {
                    await grid.RefreshDataAsync();
                    return true; // Devuelve true si la conexión fue exitosa
                }
            }
            return false; // Migración fallida
        }
      
    }
}