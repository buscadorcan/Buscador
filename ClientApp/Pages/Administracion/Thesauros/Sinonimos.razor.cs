using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using SharedApp.Data;
using SharedApp.Models.Dtos;
using System.Security.Cryptography.X509Certificates;

namespace ClientApp.Pages.Administracion.Thesauros
{
    public partial class Sinonimos
    {
        [Inject]
        public ILocalStorageService LocalStorageService { get; set; }

        [Inject]
        public IThesaurusService service { get; set; }

        [Inject]
        public IBusquedaService? iBusquedaService { get; set; }

        public ThesaurusDto thesauroPadre { get; set; }
        public ThesaurusDto thesauro { get; set; }
        private BuscarRequest buscarRequest = new BuscarRequest();
        private List<FnPredictWordsDto> ListFnPredictWordsDto = new List<FnPredictWordsDto>();

        private bool modalAbierto = false;
        private bool modalQuitarSinonimoAbierto = false;
        private bool isMensaje = false;
        private bool isMensajeGuardar = false;
        private bool isMensajeGuardarExitoso = false;

        private string sinonimoBuscar = "";
        private string nuevoSubstituto = "";
        private string sinonimoQuitar = "";
        private int expansionSeleccionada = -1;
        private string mensajeGuardar = "";
        private string usuarioLogin = "";

        protected override async Task OnInitializedAsync()
        {
            usuarioLogin = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
           
            await this.ObtenerThesauroAsync();

        }
        public async Task ObtenerThesauroAsync() {
            try
            {
                if (service != null)
                {
                    var result = await service.GetThesaurusAsync("obtener/thesaurus");
                    if (result != null)
                    {
                        this.thesauro = result;

                        this.thesauroPadre = new ThesaurusDto()
                        {
                            DiacriticsSensitive = result.DiacriticsSensitive,
                            Expansions = new List<ExpansionDto>(result.Expansions),
                            Replacements = new List<ReplacementDto>(result.Replacements)
                        };

                        /*
                            public int DiacriticsSensitive { get; set; }
                            public List<ExpansionDto> Expansions { get; set; } = new();
                            public List<ReplacementDto> Replacements { get; set; } = new();
                        */
                        
                        
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el thesaurus: {ex.Message}");
            }
        }

        public async Task EjecutarBat()
        {
            try
            {
                if (service != null)
                {
                    var result = await service.EjecutarBatAsync("ejecutar/bat");
                    if (result == "ok")
                    {
                        this.isMensajeGuardar = true;
                        this.isMensajeGuardarExitoso = true;
                        this.mensajeGuardar = "Se realizó la ejecución del archivo bat";
                    }
                    else {
                        this.isMensajeGuardar = true;
                        this.isMensajeGuardarExitoso = false;
                        this.mensajeGuardar = "ocurrió un error en la ejecución del bat";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el thesaurus: {ex.Message}");
            }
        }

        private void AbrirModal(int index)
        {
            expansionSeleccionada = index;
            nuevoSubstituto = "";  // Limpiar el input
            modalAbierto = true;
        }

        private void AbrirModalQuitarSinonimo(int seccion, string sinonimo) {
            this.modalQuitarSinonimoAbierto = true;
            expansionSeleccionada = seccion;
            sinonimoQuitar = sinonimo;
        }

        private void CerrarModal()
        {
            modalAbierto = false;
            modalQuitarSinonimoAbierto = false;
        }

        private async void AgregarSubstituto()
        {
            if (!ExisteSinonimo(nuevoSubstituto))
            {
                var temp = this.thesauro.Expansions[expansionSeleccionada];
                bool ExisteBD = await this.ValidateWords(temp.Substitutes);

                if (ExisteBD)
                {
                    this.thesauro.Expansions[expansionSeleccionada].sectionValidate = true;
                    this.isMensaje = false;
                    if (!string.IsNullOrWhiteSpace(nuevoSubstituto) && expansionSeleccionada >= 0)
                    {
                        this.thesauro.Expansions[expansionSeleccionada].Substitutes.Add(nuevoSubstituto);
                    }
                    expansionSeleccionada = -1;

                    CerrarModal();

                }
                else
                {
                    this.isMensaje=true;
                    this.mensajeGuardar = "Agregué por lo menos un registro de la base de datos";
                }


               
                

            }
            else
            {
                this.isMensaje = true;
                this.mensajeGuardar = "El sinonimo ya existe en la base de datos";

            }
            StateHasChanged();
        }

        private async void QuitarSubstituto()
        {
            this.thesauro.Expansions[expansionSeleccionada].Substitutes.Remove(sinonimoQuitar);
            

            var temp = this.thesauro.Expansions[expansionSeleccionada];
            bool ExisteBD = await this.ValidateWords(temp.Substitutes);
            CerrarModal();

            if (!ExisteBD)
            {
                this.isMensajeGuardar = true;
                this.isMensajeGuardarExitoso = false;
                this.mensajeGuardar = "Tiene que haber por lo menos un elemento de la base de datos en la sección";

                this.thesauro.Expansions[expansionSeleccionada].Substitutes.Add(sinonimoQuitar);

                StateHasChanged();

                await Task.Delay(3000);

                this.isMensajeGuardar = false;
                this.isMensajeGuardarExitoso = false;

                

            }
            StateHasChanged();
            expansionSeleccionada = -1;
           
        }

        private bool ExisteSinonimo(string valor)
        {
            foreach (var item in this.thesauro.Expansions)
            {
                var result = item.Substitutes.Find(x => x.Equals(valor));
                if (!string.IsNullOrEmpty(result))
                {
                    return true;
                }
            }

            return false;
        }

        private async void GuardarThesauro() {
            try
            {
                if (service != null)
                {
                    //verificamos las secciones a guardar
                    var temp = this.thesauro.Expansions;
                    List<ExpansionDto> eliminar = new List<ExpansionDto>();
                    foreach (var item in temp)
                    {
                        if (!item.sectionValidate)
                        {
                            eliminar.Add(item);
                        }
                    }

                    if (eliminar.Count > 0)
                    {
                        foreach (var item in eliminar)
                        {
                            this.thesauro.Expansions.Find(x => x == item).Substitutes.Clear();
                        }
                    }
                   
                    var result = await service.UpdateExpansionAsync("actualizar/expansion",this.thesauro.Expansions);
                    if (result.registroCorrecto)
                    {
                        this.isMensajeGuardar = true;
                        this.isMensajeGuardarExitoso = true;
                        this.mensajeGuardar = "Los datos se guardaron correctamente";

                    }
                    else {
                        this.isMensajeGuardar = true;
                        this.isMensajeGuardarExitoso = false;
                        this.mensajeGuardar = "Error al guardar los datos";



                    }
                }
                StateHasChanged();

                await Task.Delay(3000);

                this.isMensajeGuardar = false;
                this.isMensajeGuardarExitoso = false;

                StateHasChanged();
            }
            catch 
            {

                throw;
            }
        }

        private void MostarTodos(ChangeEventArgs e) {
            if (e.Value.Equals(""))
            {
                this.thesauro.Expansions = this.thesauroPadre.Expansions.ToList();
            }
            
        }

        private void BuscarSinonimo() {
            try
            {
                if (!string.IsNullOrEmpty(this.sinonimoBuscar))
                {
                    var temp = this.thesauroPadre;
                    var resultado = temp.Expansions
                    .Where(x => x.Substitutes.Any(s => s.Contains(this.sinonimoBuscar, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                    if (resultado.Any())
                    {
                        this.thesauro.Expansions = resultado;
                    }
                    else
                    {
                        this.thesauro.Expansions.Clear();
                    }
                }
                else {
                    this.thesauro.Expansions = this.thesauroPadre.Expansions.ToList();
                }

                StateHasChanged();
            }
            catch
            {

                throw;
            }
        }

        private async Task OnSearchChanged(ChangeEventArgs e)
        {
            nuevoSubstituto = e.Value?.ToString();

            if (!string.IsNullOrWhiteSpace(nuevoSubstituto))
            {
                // Crear un FilterItem con los parámetros requeridos
                var filterItem = new FilterItem(
                    propertyName: "Word",                  // Nombre de la propiedad a filtrar
                    value: nuevoSubstituto,                     // Valor de búsqueda
                    @operator: FilterOperator.Contains,    // Operador de comparación
                    stringComparison: StringComparison.OrdinalIgnoreCase // Tipo de comparación
                );

                // Crear la solicitud de autocompletado con el filtro
                var request = new AutoCompleteDataProviderRequest<FnPredictWordsDto>
                {
                    Filter = filterItem
                };

                var result = await FnPredictWordsDtoDataProvider(request);
                ListFnPredictWordsDto = result.Data.ToList();
            }
        }

        private async Task<AutoCompleteDataProviderResult<FnPredictWordsDto>> FnPredictWordsDtoDataProvider(AutoCompleteDataProviderRequest<FnPredictWordsDto> request)
        {
            if (request.Filter == null || string.IsNullOrWhiteSpace(request.Filter.Value))
            {
                return new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = [], TotalCount = 0 };
            }

            buscarRequest.TextoBuscar = request.Filter.Value;

            if (iBusquedaService != null)
            {
                var words = await iBusquedaService.FnPredictWords(request.Filter.Value);
                return new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = words, TotalCount = words.Count() };
            }

            return new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = [], TotalCount = 0 };
        }

        private async Task<bool>  ValidateWords(List<string> request)
        {
            if (request.Count == 0)
            {
                return true;
            }

            return await iBusquedaService.ValidateWords(request); ;
        }

    }
}
