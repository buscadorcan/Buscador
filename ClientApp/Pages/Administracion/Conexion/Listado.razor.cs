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
                // Llamar al m�todo del servicio para probar la conexi�n
                bool isConnected = await iDynamicService.TestConnectionAsync(conexion);
                var toastMessage = new ToastMessage
                {
                    Type = isConnected ? ToastType.Success : ToastType.Danger,
                    Title = "Mensaje de confirmaci�n",
                    HelpText = $"{DateTime.Now}",
                    Message = isConnected ? "Conexi�n satisfactoria" : "Conexi�n fallida",
                };

                messages.Add(toastMessage);

                // Configurar el cierre autom�tico despu�s de 5 segundos
                _ = Task.Delay(5000).ContinueWith(_ =>
                {
                    messages.Remove(toastMessage);
                    InvokeAsync(StateHasChanged); // Actualizar la UI
                });

                if (isConnected)
                {
                    await grid.RefreshDataAsync();
                    return true; // Devuelve true si la conexi�n fue exitosa
                }
            }
            return false; // Devuelve false si algo falla
        }

        private async Task<bool> OnMigrarClick(int conexion)
        {
            if (iDynamicService != null && listasHevd != null && grid != null)
            {
                // Llamar al m�todo del servicio para realizar la migraci�n
                string mensajeMigracion = await iDynamicService.MigrarConexionAsync(conexion);

                if (mensajeMigracion.Contains("�xito", StringComparison.OrdinalIgnoreCase))
                {
                    messages.Add(
                        new ToastMessage()
                        {
                            Type = ToastType.Success,
                            Title = "Mensaje de confirmaci�n",
                            HelpText = $"{DateTime.Now}",
                            Message = mensajeMigracion,
                        });

                    // Actualizar la lista si la migraci�n fue exitosa
                    listasHevd = listasHevd.Where(c => c.IdONA != conexion).ToList();
                    await grid.RefreshDataAsync();
                    return true; // Migraci�n exitosa
                }
                else
                {
                    messages.Add(
                        new ToastMessage()
                        {
                            Type = ToastType.Danger,
                            Title = "Mensaje de confirmaci�n",
                            HelpText = $"{DateTime.Now}",
                            Message = mensajeMigracion,
                        });
                }
            }
            return false; // Migraci�n fallida
        }
        //private async Task<bool> OnTestconexionClick(ONAConexionDto conexion)
        //{
        //    if (iConexionService != null && listasHevd != null && grid != null)
        //    {
        //        var respuesta = await iConexionService.TestConexionAsync(conexion);
        //        if (respuesta.registroCorrecto)
        //        {
        //            messages.Add(
        //                      new ToastMessage()
        //                      {
        //                          Type = ToastType.Success,
        //                          Title = "Mensaje de confirmacion",
        //                          HelpText = $"{DateTime.Now}",
        //                          Message = $"Conexion satisfactoria",
        //                      });
        //            await grid.RefreshDataAsync();
        //            return true; // Devuelve true si la conexi�n fue exitosa
        //        }
        //        else
        //        {
        //            messages.Add(
        //            new ToastMessage()
        //            {
        //                Type = ToastType.Danger,
        //                Title = "Mensaje de confirmacion",
        //                HelpText = $"{DateTime.Now}",
        //                Message = $"Conexion fallida",
        //            });
        //        }
        //    }
        //    return false; // Devuelve false en caso de que las condiciones no se cumplan o haya fallado
        //}

        //private async Task OnDeleteConexionClick(int IdONA)
        //{
        //    if (iConexionService != null && listasHevd != null && grid != null)
        //    {
        //        var respuesta = await iConexionService.DeleteConexionsAsync(IdONA);
        //        if (respuesta.registroCorrecto)
        //        {

        //            listasHevd = listasHevd.Where(c => c.IdONA != IdONA).ToList();
        //            await grid.RefreshDataAsync();
        //        }
        //    }
        //}
    }
}