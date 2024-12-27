using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.ONA
{
    public partial class Listado
    {
        private List<ONADto>? listaONAs;
        [Inject]
        IONAService? iONAservice { get; set; }
        private async Task<GridDataProviderResult<ONADto>> ONAsDataProvider(GridDataProviderRequest<ONADto> request)
        {
            try
            {
                if (listaONAs is null && iONAservice != null)
                {
                    listaONAs = await iONAservice.GetONAsAsync();
                }

                return await Task.FromResult(request.ApplyTo(listaONAs ?? []));
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}