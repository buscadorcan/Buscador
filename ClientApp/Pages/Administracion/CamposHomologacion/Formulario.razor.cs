using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.CamposHomologacion
{
    public partial class Formulario
    {
        private Button saveButton = default!;
        private HomologacionDto homologacion = new HomologacionDto();
        private HomologacionDto homologacionGrupo = new HomologacionDto();
        private List<VwFiltroDto> filtros = new();
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        [Inject]
        public ICatalogosService? iCatalogoService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Parameter]
        public int? Id { get; set; }
        [Parameter]
        public int? IdPadre { get; set; }
        [Inject]
        public Services.ToastService? toastService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            filtros = await iCatalogoService.GetFiltrosAsync();
            homologacionGrupo = await iHomologacionService.GetHomologacionAsync((int) IdPadre);
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
                toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                navigationManager?.NavigateTo("/campos-homologacion");
            }
            else
            {
                toastService?.CreateToastMessage(ToastType.Danger, "Debe llenar todos los campos");
            }

            saveButton.HideLoading();
        }

        private void OnAutoCompleteChanged(string mascaraDato) {
            homologacion.MascaraDato = mascaraDato;
        }

        private void ActualizarFiltro(ChangeEventArgs e)
        {
            // Obtener el valor seleccionado
            var selectedValue = e.Value?.ToString();

            // Si el valor es "Sin Filtro" (vacío), asignar null a la variable
            if (string.IsNullOrEmpty(selectedValue))
            {
                homologacion.IdHomologacionFiltro = null;
            }
            else
            {
                // Convertir el valor a int, si es válido
                homologacion.IdHomologacionFiltro = int.TryParse(selectedValue, out var valor) ? valor : null;
            }
        }

        private bool isIndexar // Propiedad booleana vinculada al Switch
        {
            get => homologacion.Indexar == "S"; // Convertir "S" a true
            set => homologacion.Indexar = value ? "S" : "N"; // Convertir true a "S"
        }

        private bool isMostrar // Propiedad booleana vinculada al Switch
        {
            get => homologacion.Mostrar == "S"; // Convertir "S" a true
            set => homologacion.Mostrar = value ? "S" : "N"; // Convertir true a "S"
        }
    }
}