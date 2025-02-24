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
        private bool modalQuitarSeccion = false;

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

        ///<summary>
        ///ObtenerThesauroAsync: obtiene el objeto del archivo thesaurus
        ///</summary>
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
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el thesaurus: {ex.Message}");
            }
        }

        ///<summary>
        ///NuevaSeccion:evento para crear una nueva sección
        ///</summary>
        public void NuevaSeccion() {

            this.thesauro.Expansions.Add(new ExpansionDto());
            this.thesauroPadre.Expansions.Add(new ExpansionDto());
        }

        ///<summary>
        ///NuevaSeccion:evento para crear una nueva sección
        ///</summary>
        public void QuitarSeccion()
        {
            this.thesauro.Expansions.RemoveAt(this.expansionSeleccionada);
            CerrarModal();
        }
        ///<summary>
        ///EjecutarBat:evento para ejecutar la acción de copiar el archivo thesauros en la carpeta de sqlserver
        ///</summary>
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
                        await Task.Delay(5000);
                    }
                    else {
                        this.isMensajeGuardar = true;
                        this.isMensajeGuardarExitoso = false;
                        this.mensajeGuardar = "ocurrió un error en la ejecución del bat";
                        await Task.Delay(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el thesaurus: {ex.Message}");
            }
        }

        ///<summary>
        ///ResetearqSqlServer:evento para reiniciar el servicio de sqlserver
        ///</summary>
        public async Task ResetearqSqlServer()
        {
            try
            {
                if (service != null)
                {
                    var result = await service.EjecutarBatAsync("reset/sqlserver");
                    if (result == "ok")
                    {
                        this.isMensajeGuardar = true;
                        this.isMensajeGuardarExitoso = true;
                        this.mensajeGuardar = "Se realizó el reseteo del servidor";
                        await Task.Delay(5000);
                    }
                    else
                    {
                        this.isMensajeGuardar = true;
                        this.isMensajeGuardarExitoso = false;
                        this.mensajeGuardar = "ocurrió un error en el reseteo del servidor";
                        await Task.Delay(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el thesaurus: {ex.Message}");
            }
        }

        ///<summary>
        ///AbrirModal:evento para visualizar el modal de registro de un sinonimo
        ///</summary>
        private void AbrirModal(int index)
        {
            expansionSeleccionada = index;
            nuevoSubstituto = "";  // Limpiar el input
            modalAbierto = true;
        }


        ///<summary>
        ///AbrirModalQuitarSinonimo:evento para visualizar el modal de la eliminación de un sinonimo
        ///</summary>
        private void AbrirModalQuitarSinonimo(int seccion, string sinonimo) {
            this.modalQuitarSinonimoAbierto = true;
            expansionSeleccionada = seccion;
            sinonimoQuitar = sinonimo;
        }

        ///<summary>
        ///AbrirModalQuitarSeccion:evento para visualizar el modal de la eliminación de una sección
        ///</summary>
        private void AbrirModalQuitarSeccion(int seccion)
        {
            this.modalQuitarSeccion = true;
            this.modalQuitarSinonimoAbierto = true;
            expansionSeleccionada = seccion;
        }

        ///<summary>
        ///CerrarModal:evento cerrar un modal
        ///</summary>
        private void CerrarModal()
        {
            modalAbierto = false;
            modalQuitarSinonimoAbierto = false;
            modalQuitarSeccion = false;
        }

        ///<summary>
        ///AgregarSubstituto: evento para agregar un nuevo sustituto
        ///</summary>
        private async void AgregarSubstituto()
        {
            this.isMensaje = false;
            this.mensajeGuardar = "";
            if (!ExisteSinonimo(nuevoSubstituto))
            {
                var temp = new ExpansionDto();

                if (this.thesauro.Expansions[expansionSeleccionada].Substitutes.Count > 0)
                {
                    foreach (var item in this.thesauro.Expansions[expansionSeleccionada].Substitutes)
                    {
                        temp.Substitutes.Add(item);
                    }

                }
               

                if (temp.Substitutes.Count == 0){temp.Substitutes.Add(nuevoSubstituto);}

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

        ///<summary>
        ///QuitarSubstituto: evento para quitar un sustituto
        ///</summary>
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

                await Task.Delay(5000);

                this.isMensajeGuardar = false;
                this.isMensajeGuardarExitoso = false;

                

            }
            StateHasChanged();
            expansionSeleccionada = -1;
           
        }

        ///<summary>
        ///ExisteSinonimo: evento para verificar si existe un nuevo sustituto
        ///</summary>
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

        //<summary>
        ///GuardarThesauro: evento para guardar el archivo thesaurus
        ///</summary>
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

                await Task.Delay(5000);

                this.isMensajeGuardar = false;
                this.isMensajeGuardarExitoso = false;

                StateHasChanged();
            }
            catch 
            {

                throw;
            }
        }

        //<summary>
        ///MostarTodos: evento para mostrar la lista de todas las secciones del archivo thesauros
        ///</summary>
        private void MostarTodos(ChangeEventArgs e) {
            if (e.Value.Equals(""))
            {
                this.thesauro.Expansions = this.thesauroPadre.Expansions.ToList();
            }
            
        }

        //<summary>
        ///BuscarSinonimo: evento para mostrar la lista de los sinonimos buscados 
        ///</summary>
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


        //<summary>
        ///OnSearchChanged: evento para buscar un sinonimo
        ///</summary>
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


        //<summary>
        ///FnPredictWordsDtoDataProvider: evento para mostrar los sinonimos existen en una caja para filtro
        ///</summary>
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


        //<summary>
        ///ValidateWords: evento para validar la lista de sinonimos de una sección existentes en la base de datos
        ///</summary>
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
