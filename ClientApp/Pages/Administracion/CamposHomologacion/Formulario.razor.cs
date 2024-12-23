using BlazorBootstrap;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.CamposHomologacion
{
    public partial class Formulario
    {
        private Button saveButton = default!;
        private HomologacionDto homologacion = new HomologacionDto();
        private HomologacionDto? homologacionGrupo = new HomologacionDto();
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Inject] protected ToastService ToastService { get; set; } = default!;
        [Parameter]
        public int? Id { get; set; }
        [Parameter]
        public int? IdPadre { get; set; }
        protected override async Task OnInitializedAsync()
        {
            homologacionGrupo = await iHomologacionService.GetHomologacionAsync(IdPadre ?? 0);
            if (Id > 0) {
                homologacion = await iHomologacionService.GetHomologacionAsync(Id.Value);
            } else {
                homologacion.IdHomologacionGrupo = IdPadre;
                homologacion.InfoExtraJson = "{}";
                homologacion.MascaraDato = "TEXTO";
                homologacion.CodigoHomologacion = "";
                homologacion.SiNoHayDato = "";
            }
        }
        private async Task GuardarHomologacion()
        {
            saveButton.ShowLoading("Guardando...");

            var result = await iHomologacionService.RegistrarOActualizar(homologacion);
            if (result.registroCorrecto)
            {
                ToastService.Notify(new(ToastType.Success, "Registrado exitosamente"));
                // toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                navigationManager?.NavigateTo("/campos-homologacion");
            }
            else
            {
                // toastService?.CreateToastMessage(ToastType.Danger, "Debe llenar todos los campos");
                ToastService.Notify(new(ToastType.Danger, "Debe llenar todos los campos"));
            }

            saveButton.HideLoading();
        }
        private void OnAutoCompleteChanged(string mascaraDato) {
            homologacion.MascaraDato = mascaraDato;
        }
    }
}