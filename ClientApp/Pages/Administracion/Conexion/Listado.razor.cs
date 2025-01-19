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
            if (iConexionService != null && listasHevd != null && grid != null)
            {
                var respuesta = await iConexionService.testConexion(conexion);
                if (respuesta.registroCorrecto)
                {
                    messages.Add(
                              new ToastMessage()
                              {
                                  Type = ToastType.Success,
                                  Title = "Mensaje de confirmacion",
                                  HelpText = $"{DateTime.Now}",
                                  Message = $"Conexion satisfactoria",
                              });
                    await grid.RefreshDataAsync();
                    return true; // Devuelve true si la conexión fue exitosa
                }
                else
                {
                    messages.Add(
                    new ToastMessage()
                    {
                        Type = ToastType.Danger,
                        Title = "Mensaje de confirmacion",
                        HelpText = $"{DateTime.Now}",
                        Message = $"Conexion fallida",
                    });
                }
            }
            return false; // Devuelve false en caso de que las condiciones no se cumplan o haya fallado
        }

        private async Task OnDeleteConexionClick(int IdONA)
        {
            if (iConexionService != null && listasHevd != null && grid != null)
            {
                var respuesta = await iConexionService.DeleteConexionsAsync(IdONA);
                if (respuesta.registroCorrecto)
                {

                    listasHevd = listasHevd.Where(c => c.IdONA != IdONA).ToList();
                    await grid.RefreshDataAsync();
                }
            }
        }
    }
}