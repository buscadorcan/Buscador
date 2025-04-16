using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Infractruture.Interfaces;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedApp.Helpers;
using System.Text;
using SharedApp.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para la página de búsqueda de CAN.
    /// </summary>
    public partial class IndexGrilla : ComponentBase
    {

        /// <summary>
        /// Servicio de homologación.
        /// </summary>
        [Inject] 
        public IBusquedaService BusquedaService { get; set; } = default!;

        /// <summary>
        /// Servicio de homologación.
        /// </summary>
        [Inject]
        private IJSRuntime JS { get; set; } = default!;// 🔹 Inyección de JavaScript

        /// <summary>
        /// Servicio de homologación.
        /// </summary>
        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        /// <summary>
        /// Servicio de homologación.
        /// </summary>
        [Inject]
        public HttpClient Http { get; set; } = default!;

        /// <summary>
        /// Servicio de homologación.
        /// </summary>
        [Inject] public IHomologacionService? iHomologacionService { get; set; }

        /// <summary>
        /// Servicio de JavaScript.
        /// </summary>
        [Inject] public IJSRuntime? iJSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the list data dto.
        /// </summary>
        [Parameter] public List<BuscadorResultadoDataDto>? ListDataDto { get; set; }

        /// <summary>
        /// Gets or sets the list url data dto.
        /// </summary>
        [Parameter] public Dictionary<int, string>? iconUrls { get; set; }

        /// <summary>
        /// Listado de etiquetas de la grilla.
        /// </summary>
        [Parameter] public List<VwGrillaDto>? listaEtiquetasGrilla { get; set; }
        
        /// <summary>
        /// COmponente modal.
        /// </summary>
        private Modal modal = default!;

        /// <summary>
        /// open or close dialog
        /// </summary>
        private bool isDialogOpen = false;

        /// <summary>
        /// url pdf
        /// </summary>
        private string? PdfUrl;
        // private bool isLoading = true;

        private bool isModalOpen = false;

        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
        /// <param name="resultData"></param>
        private async void showModal(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Style = "font-size: 10px !important;";
            modal.Size = ModalSize.ExtraLarge;
            await modal.ShowAsync<EsquemaModal>(title: "Información Detallada", parameters: parameters);
        }

        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
        private async void showModalOna(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Style = "font-size: 10px !important;";
            modal.Size = ModalSize.Regular;
            await modal.ShowAsync<OnaModal>(title: "Información Organizacion", parameters: parameters);
        }

        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
        private async void showModalOEC(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Style = "font-size: 10px !important;";
            modal.Size = ModalSize.Regular;
            await modal.ShowAsync<OECModal>(title: "Información del OEC", parameters: parameters);
        }

        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
        private async void showModalESQ(BuscadorResultadoDataDto resultData)
        {
         
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Style = "font-size: 10px !important;";
            modal.Size = ModalSize.ExtraLarge;
            await modal.ShowAsync<IndvEsquemaModal>(title: "Información Esquema", parameters: parameters);
        }

        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
        private async Task ShowPdfDialog(BuscadorResultadoDataDto resultData)
        {
            // Obtener la URL del certificado
            var pdfUrl = await GetPdfUrlFromEsquema(resultData);

            if (string.IsNullOrWhiteSpace(pdfUrl))
            {
                Console.WriteLine("No se encontró la URL del certificado.");
                return;
            }

            // Llamar a la función JavaScript para abrir la ventana emergente
            await iJSRuntime.InvokeVoidAsync("abrirVentanaPDF", pdfUrl);
        }

        /// <summary>
        /// Método para obtener la URL del certificado.
        /// </summary>
        private async Task<string?> GetPdfUrlFromEsquema(BuscadorResultadoDataDto resultData)
        {
            try
            {
                if (resultData.DataEsquemaJson == null || !resultData.DataEsquemaJson.Any())
                    return null;

                var homologaciones = await iHomologacionService.GetHomologacionsAsync();
                var idHomologacion = homologaciones.FirstOrDefault(x => x.CodigoHomologacion == "KEY_ESQ_CERT")?.IdHomologacion;
               
                if (idHomologacion == null)
                    return null;

                var urlPdf = resultData.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == idHomologacion)?.Data;
                Console.WriteLine("urlPDF:" + urlPdf);
                Console.WriteLine("idHomologacion:" + idHomologacion);
                //urlPdf = SanitizeUrl(urlPdf);
                return urlPdf;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la URL del certificado", ex);
            }
        }

        private async Task HandleModalClose()
        {
            Console.WriteLine("🔹 Modal cerrado automáticamente al hacer clic fuera.");
            isModalOpen = false;
            StateHasChanged(); // 🔄 Forzar actualización de la UI
        }

        private async Task ExportarExcel()
        {
            var datos = ConstruirDatosExportacion();
            var json = JsonConvert.SerializeObject(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Http.PostAsync($"{Inicializar.UrlBaseApi}api/buscador/excel", content);
            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var base64 = Convert.ToBase64String(bytes);
                await JS.InvokeVoidAsync("descargarArchivo", base64, "exportacion.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        private async Task ExportarPDF()
        {
            var datos = ConstruirDatosExportacion();
            var json = JsonConvert.SerializeObject(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Http.PostAsync($"{Inicializar.UrlBaseApi}api/buscador/pdf", content);
            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var base64 = Convert.ToBase64String(bytes);
                await JS.InvokeVoidAsync("descargarArchivo", base64, "exportacion.pdf", "application/pdf");
            }
        }


        private List<Dictionary<string, string>> ConstruirDatosExportacion()
        {
            var resultado = new List<Dictionary<string, string>>();

            if (ListDataDto == null || listaEtiquetasGrilla == null) return resultado;

            foreach (var item in ListDataDto)
            {
                var fila = new Dictionary<string, string>();

                foreach (var etiqueta in listaEtiquetasGrilla)
                {
                    var valor = item.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == etiqueta.IdHomologacion)?.Data ?? "";
                    fila[etiqueta.MostrarWeb ?? $"Columna-{etiqueta.IdHomologacion}"] = valor;
                }

                resultado.Add(fila);
            }

            return resultado;
        }


    }
}