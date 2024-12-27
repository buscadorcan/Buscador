using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.ONA
{
    public partial class Formulario
    {
        private Button saveButton = default!;
        private ONADto onas = new ONADto();
        [Inject]
        public IONAService? iONAsService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Id > 0 && iONAsService != null) {
                onas = await iONAsService.GetONAsAsync(Id.Value);

            } 
        }
        private async Task RegistrarONA()
        {
            saveButton.ShowLoading("Guardando...");

            if (iONAsService != null)
            {
                var result = await iONAsService.RegistrarONAsActualizar(onas);
                if (result.registroCorrecto)
                {
                    toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                    navigationManager?.NavigateTo("/onas");
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al registrar en el servidor");
                }
            }

            saveButton.HideLoading();
        }

    }
}
