using BlazorBootstrap;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Blazored.LocalStorage;
using SharedApp.Helpers;
using System.Text;
using System.Net.Http;
using Infractruture.Services;
using SharedApp.Dtos;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.JSInterop;
using OfficeOpenXml;

namespace ClientApp.Pages.Administracion.Validacion
{
    public partial class Listado
    {
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        [Inject]
        private IHomologacionEsquemaService? iHomologacionEsquemaService { get; set; }
        [Inject]
        private IBusquedaService? servicio { get; set; }
        [Inject]
        public IDynamicService? iDynamicService { get; set; }
        [Inject]
        public IONAService? iONAservice { get; set; }
        [Inject]
        public IEsquemaService? iEsquemaService { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        [Inject]
        private HttpClient _httpClient { get; set; }
        [Inject]
        public Infractruture.Services.ToastService? toastService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Inject]
        private IConexionService? iConexionService { get; set; }
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();

        private Button saveButton = default!;
        private Button validateButton = default!;
        
        private Grid<EsquemaVistaDto>? grid = default;
        private List<HomologacionDto>? listaOrganizaciones = new List<HomologacionDto>();
        private List<OnaDto>? listaONAs;
        private List<HomologacionEsquemaDto>? listaHomologacionEsquemas = new List<HomologacionEsquemaDto>();
        //private EsquemaVistaOnaDto? esquemaSelected;
        private EsquemaDto? esquemaSelected;
        private bool enabledCeldas;
        private HomologacionDto? organizacionSelected;
        private OnaDto? onaSelected;
        private List<EsquemaVistaDto> listasHevd = new List<EsquemaVistaDto>();
        private List<EsquemaVistaColumnaDto> listaEsquemaVistaColumna = new List<EsquemaVistaColumnaDto>();

        public string nombreSugerido = "";
        public string origenDatosValidar = "";
        //private List<EsquemaVistaOnaDto>? listaEsquemasOna = new List<EsquemaVistaOnaDto>();
        private List<EsquemaDto>? listaEsquemasOna = new List<EsquemaDto>();

        private List<string> NombresVistas { get; set; }
        private ONAConexionDto? currentConexion = null;
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/validacion";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "validacion";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            var onaPais = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
            var rol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            bool accessRol = rol == "KEY_USER_CAN";
            if (accessRol)
            {
                if (listaONAs is null && iONAservice != null)
                {
                    await LoadONAs();
                    listaEsquemasOna = await iEsquemaService.GetListEsquemasAsync();
                }
            }
            else
            {
                await LoadONAs();
                listaONAs = listaONAs.Where(onas => onas.IdONA == onaPais).ToList();
                listaEsquemasOna = await iEsquemaService.GetListEsquemasAsync();
            }
        }

        private async Task LoadONAs()
        {
            if (iONAservice != null)
            {
                listaONAs = await iONAservice.GetONAsAsync();
            }
        }

        private async Task CambiarSeleccionOna(ChangeEventArgs e)
        {
            var selectedId = Convert.ToInt32(e.Value);

            if (listaONAs == null || !listaONAs.Any())
            {
                toastService?.CreateToastMessage(ToastType.Warning, "No hay ONAs disponibles.");
                navigationManager?.NavigateTo("/validacion");
                return;
            }

            // Busca el objeto correspondiente en la lista
            onaSelected = listaONAs?.FirstOrDefault(o => o.IdONA == selectedId);

            if (onaSelected != null)
            {
                if (esquemaSelected != null)
                {
                    await CambiarSeleccionEsquema(esquemaSelected);
                }
                StateHasChanged();
            }
        }
        private async Task HandleEsquemaSelectionChange(ChangeEventArgs e)
        {
            Console.WriteLine($"Evento onchange detectado con valor: {e.Value}");

            if (e.Value is null)
            {
                toastService?.CreateToastMessage(ToastType.Warning, "Debe seleccionar un esquema válido.");
                return;
            }

            // Convierte el valor a un número
            if (!int.TryParse(e.Value.ToString(), out int selectedId))
            {
                toastService?.CreateToastMessage(ToastType.Warning, "El ID del esquema seleccionado no es válido.");
                return;
            }

            // Busca el esquema en la lista
            var selectedEsquema = listaEsquemasOna?.FirstOrDefault(es => es.IdEsquema == selectedId);

            if (selectedEsquema != null)
            {
                // Asignamos el esquema seleccionado
                esquemaSelected = selectedEsquema;

                // Llamamos al método asincrónico
                await CambiarSeleccionEsquema(esquemaSelected);

                // Refrescamos la UI
                StateHasChanged();
            }
            else
            {
                toastService?.CreateToastMessage(ToastType.Warning, "Esquema seleccionado no encontrado.");
            }
        }

        //private async Task HandleEsquemaSelectionChange(ChangeEventArgs e)
        //{
        //    var selectedId = Convert.ToInt32(e.Value);

        //    // Encuentra el esquema seleccionado en la lista
        //    var selectedEsquema = listaEsquemasOna?.FirstOrDefault(es => es.IdEsquema == selectedId);

        //    if (selectedEsquema != null)
        //    {
        //        if (esquemaSelected != null)
        //        {
        //            await CambiarSeleccionEsquema(esquemaSelected);
        //        }
        //        StateHasChanged();
        //    }
        //}

        private async Task CambiarSeleccionEsquema(EsquemaDto _esquemaSelected)
        {
            // Obtiene el rol del usuario desde el LocalStorage
            var rol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            bool accessRol = rol == "KEY_USER_CAN";

            // Configura `currentConexion` y `enabledCeldas` dependiendo del rol
            if (accessRol)
            {
                if (onaSelected == null)
                {
                    toastService?.CreateToastMessage(ToastType.Warning, "Debe seleccionar una ONA antes de continuar.");
                    navigationManager?.NavigateTo("/validacion");
                    return;
                }
                currentConexion = await iConexionService.GetOnaConexionByOnaAsync(onaSelected.IdONA);
            }
            else
            {
                int IdOna = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
                currentConexion = await iConexionService.GetOnaConexionByOnaAsync(IdOna);
            }

            if (currentConexion?.OrigenDatos == "EXCEL")
            {
                origenDatosValidar = currentConexion.OrigenDatos;
                enabledCeldas = true;
            }
            else
            {
                origenDatosValidar = currentConexion.OrigenDatos;
                enabledCeldas = false;
            }

            // Actualiza el esquema seleccionado y el nombre sugerido
            esquemaSelected = _esquemaSelected;
            nombreSugerido = esquemaSelected.EsquemaVista;

            // Lógica para obtener y procesar las columnas del esquema
            List<HomologacionDto> Columnas;
            var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
            //if (string.IsNullOrWhiteSpace(homologacionEsquema.EsquemaJson))
            //{
            //    toastService?.CreateToastMessage(ToastType.Danger, "Error: No hay datos en el esquema seleccionado.");
            //    return;
            //}
            if (homologacionEsquema == null || string.IsNullOrWhiteSpace(homologacionEsquema.EsquemaJson))
            {
                toastService?.CreateToastMessage(ToastType.Danger, "Error: No hay datos en el esquema seleccionado.");
                navigationManager?.NavigateTo("/validacion");
                return;
            }

            Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson)
                .OrderBy(c => c.MostrarWebOrden)
                .ToList();

            listasHevd = new List<EsquemaVistaDto>();

            var vistas = await iDynamicService.GetListaValidacionEsquema(onaSelected.IdONA, esquemaSelected.IdEsquema);

            foreach (var c in Columnas)
            {
                var vistaCorrespondiente = vistas.FirstOrDefault(n => n.NombreEsquema != null && n.NombreEsquema.Equals(c.NombreHomologado));
                var count = vistas.Count(n => n.NombreVista != null && n.NombreVista.Equals(c.NombreHomologado));

                listasHevd.Add(new EsquemaVistaDto
                {
                    NombreEsquema = c.NombreHomologado,
                    NombreVista = vistaCorrespondiente?.NombreVista ?? c.NombreHomologado.Trim(),
                    IsValid = count > 0
                });
            }

            // Refresca la tabla si es necesario
            //if (grid != null)
            //{
            //    await grid.RefreshDataAsync();
            //}
        }


        //private async Task CambiarSeleccionEsquema(EsquemaVistaOnaDto _esquemaSelected)
        //{
        //    esquemaSelected = _esquemaSelected;
        //    nombreSugerido = esquemaSelected.VistaOrigen;



        //    var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
        //    var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson).OrderBy(c => c.MostrarWebOrden).ToList();

        //    listasHevd = new List<EsquemaVistaDto>();

        //    var vistas = await iDynamicService.GetListaValidacionEsquema(onaSelected.IdONA, esquemaSelected.IdEsquemaVista);

        //    foreach (var c in Columnas)
        //    {
        //        var vistaCorrespondiente = vistas.FirstOrDefault(n => n.NombreEsquema != null && n.NombreEsquema.Equals(c.NombreHomologado));
        //        // Contar cuántas vistas cumplen la condición
        //        var count = vistas.Count(n => n.NombreVista != null && n.NombreVista.Equals(c.NombreHomologado));

        //        listasHevd.Add(new EsquemaVistaDto
        //        {
        //            NombreEsquema = c.NombreHomologado,
        //            NombreVista = vistaCorrespondiente?.NombreVista ?? "", // Asignar NombreVista de la vista correspondiente
        //            IsValid = count > 0 // Asignar IsValid con base en count
        //        });
        //    }

        //    if (grid != null)
        //        await grid.RefreshDataAsync();
        //}


        private async Task<GridDataProviderResult<EsquemaVistaDto>> EsquemaVistaDataProvider(GridDataProviderRequest<EsquemaVistaDto> request)
        {
            return await Task.FromResult(request.ApplyTo(listasHevd));
        }

        private async Task GuardarCambios()
        {
            try
            {
                saveButton.ShowLoading("Guardando...");

                if (onaSelected == null)
                {
                    toastService?.CreateToastMessage(ToastType.Warning, "Por favor, selecciona una ONA antes de guardar.");
                    navigationManager?.NavigateTo("/validacion");
                    saveButton.HideLoading();
                    return;
                }

                if (esquemaSelected == null)
                {
                    toastService?.CreateToastMessage(ToastType.Warning, "Por favor, selecciona un esquema antes de guardar.");
                    navigationManager?.NavigateTo("/validacion");
                    saveButton.HideLoading();
                    return;
                }

                if (string.IsNullOrEmpty(nombreSugerido))
                {
                    toastService?.CreateToastMessage(ToastType.Warning, "El nombre sugerido no puede estar vacío.");
                    navigationManager?.NavigateTo("/validacion");
                    saveButton.HideLoading();
                    return;
                }

                var esquemaRegistro = new EsquemaVistaValidacionDto
                {
                    IdOna = onaSelected.IdONA,
                    IdEsquema = esquemaSelected.IdEsquema,
                    VistaOrigen = nombreSugerido,
                    Estado = "A"
                };

                var resultado = await iEsquemaService.GuardarEsquemaVistaValidacionAsync(esquemaRegistro);

                if (resultado?.registroCorrecto != true)
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al guardar el esquema.");
                    navigationManager?.NavigateTo("/validacion");
                    saveButton.HideLoading();
                    return;
                }

                // Elimina columnas asociadas
                var success = await iEsquemaService.EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(esquemaRegistro);
                if (!success)
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al eliminar columnas existentes.");
                    navigationManager?.NavigateTo("/validacion");
                    saveButton.HideLoading();
                    return;
                }

                // Procesa columnas
                listaEsquemaVistaColumna = new List<EsquemaVistaColumnaDto>();
                var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
                var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson)
                    .OrderBy(c => c.MostrarWebOrden)
                    .ToList();

                foreach (var c in Columnas)
                {
                    var vistaCorrespondiente = listasHevd.FirstOrDefault(v => v.NombreEsquema == c.NombreHomologado);

                    listaEsquemaVistaColumna.Add(new EsquemaVistaColumnaDto
                    {
                        ColumnaEsquemaIdH = c.IdHomologacion,
                        ColumnaEsquema = c.NombreHomologado,
                        ColumnaVista = vistaCorrespondiente?.NombreVista,
                        ColumnaVistaPK = false,
                        Estado = "A"
                    });
                }

                var successRows = await iEsquemaService.GuardarListaEsquemaVistaColumna(listaEsquemaVistaColumna, onaSelected.IdONA, esquemaSelected.IdEsquema);

                if (successRows?.registroCorrecto == true)
                {
                    toastService?.CreateToastMessage(ToastType.Success, "Guardado exitosamente.");
                    navigationManager?.NavigateTo("/validacion");
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "Error al guardar columnas.");
                    navigationManager?.NavigateTo("/validacion");
                }
            }
            catch (Exception ex)
            {
                toastService?.CreateToastMessage(ToastType.Danger, $"Error inesperado: {ex.Message}");
                navigationManager?.NavigateTo("/validacion");
            }
            finally
            {
                saveButton.HideLoading();
            }
        }


        private async Task ValidarDatos()
        {
            try
            {
                objEventTracking.CodigoHomologacionMenu = "/validacion";
                objEventTracking.NombreAccion = "ValidarDatos";
                objEventTracking.NombreControl = "btnGuardar";
                objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                validateButton.ShowLoading("Validando...");

                int IdOna = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
                currentConexion = await iConexionService.GetOnaConexionByOnaAsync(IdOna);

                if (origenDatosValidar.Equals("EXCEL"))
                {
                    var esquemaRegistro = new EsquemaVistaValidacionDto
                    {
                        IdOna = onaSelected.IdONA,
                        IdEsquema = esquemaSelected.IdEsquema,
                        VistaOrigen = nombreSugerido,
                        Estado = "A"
                    };
                    
                    var success = await iEsquemaService.EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(esquemaRegistro);
                    if (success)
                    {
                        listaEsquemaVistaColumna = new List<EsquemaVistaColumnaDto>();

                        var vistas = listasHevd.Select(item => new EsquemaVistaDto
                        {
                            NombreEsquema = item.NombreEsquema,
                            NombreVista = item.NombreVista,
                            IsValid = false
                        }).ToList();

                        var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
                        var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson)
                            .OrderBy(c => c.MostrarWebOrden).ToList();

                        var filasNoCoinciden = new List<string>();

                        foreach (var c in Columnas)
                        {
                            // Buscar el elemento correspondiente en vistas
                            var vistaCorrespondiente = vistas.FirstOrDefault(v => v.NombreEsquema != null && v.NombreEsquema.Equals(c.NombreHomologado));

                            // Validar coincidencias y agregar solo las filas que coincidan
                            if (vistaCorrespondiente != null && vistaCorrespondiente.NombreVista == c.NombreHomologado)
                            {
                                vistaCorrespondiente.IsValid = true;

                                listaEsquemaVistaColumna.Add(new EsquemaVistaColumnaDto
                                {
                                    ColumnaEsquemaIdH = c.IdHomologacion,
                                    ColumnaEsquema = vistaCorrespondiente.NombreEsquema,
                                    ColumnaVista = vistaCorrespondiente.NombreVista,
                                    ColumnaVistaPK = false,
                                    Estado = "A"
                                });
                            }
                            else
                            {
                                filasNoCoinciden.Add(c.NombreHomologado);
                            }
                        }

                        var successRows = await iEsquemaService.GuardarListaEsquemaVistaColumna(listaEsquemaVistaColumna, onaSelected.IdONA, esquemaSelected.IdEsquema);

                        if (successRows.registroCorrecto)
                        {
                            toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                            navigationManager?.NavigateTo("/validacion");

                            if (filasNoCoinciden.Any())
                            {
                                toastService?.CreateToastMessage(ToastType.Warning, $"No se pudo guardar las siguientes filas no coinciden o tienen espacios en blanco: {string.Join(", ", filasNoCoinciden)}");
                                navigationManager?.NavigateTo("/validacion");
                            }
                            if (esquemaSelected != null)
                            {
                                await CambiarSeleccionEsquema(esquemaSelected);
                            }
                            validateButton.HideLoading();
                            validateButton.HideLoading();
                        }
                        else
                        {
                            toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                            navigationManager?.NavigateTo("/validacion");
                            validateButton.HideLoading();
                        }
                    }
                    else
                    {
                        var resultado = await iEsquemaService.GuardarEsquemaVistaValidacionAsync(esquemaRegistro);
                        if (resultado != null && resultado.registroCorrecto)
                        {
                            listaEsquemaVistaColumna = new List<EsquemaVistaColumnaDto>();

                            var vistas = listasHevd.Select(item => new EsquemaVistaDto
                            {
                                NombreEsquema = item.NombreEsquema,
                                NombreVista = item.NombreVista,
                                IsValid = false
                            }).ToList();

                            var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
                            var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson)
                                .OrderBy(c => c.MostrarWebOrden).ToList();

                            var filasNoCoinciden = new List<string>();

                            foreach (var c in Columnas)
                            {
                                // Buscar el elemento correspondiente en vistas
                                var vistaCorrespondiente = vistas.FirstOrDefault(v => v.NombreEsquema != null && v.NombreEsquema.Equals(c.NombreHomologado));

                                // Validar coincidencias y agregar solo las filas que coincidan
                                if (vistaCorrespondiente != null && vistaCorrespondiente.NombreVista == c.NombreHomologado)
                                {
                                    vistaCorrespondiente.IsValid = true;

                                    listaEsquemaVistaColumna.Add(new EsquemaVistaColumnaDto
                                    {
                                        ColumnaEsquemaIdH = c.IdHomologacion,
                                        ColumnaEsquema = vistaCorrespondiente.NombreEsquema,
                                        ColumnaVista = vistaCorrespondiente.NombreVista,
                                        ColumnaVistaPK = false,
                                        Estado = "A"
                                    });
                                }
                                else
                                {
                                    filasNoCoinciden.Add(c.NombreHomologado);
                                }
                            }

                            var successRows = await iEsquemaService.GuardarListaEsquemaVistaColumna(listaEsquemaVistaColumna, onaSelected.IdONA, esquemaSelected.IdEsquema);

                            if (successRows.registroCorrecto)
                            {
                                toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                                navigationManager?.NavigateTo("/validacion");

                                if (filasNoCoinciden.Any())
                                {
                                    toastService?.CreateToastMessage(ToastType.Warning, $"No se pudo guardar las siguientes filas no coinciden o tienen espacios en blanco: {string.Join(", ", filasNoCoinciden)}");
                                    navigationManager?.NavigateTo("/validacion");
                                }
                                if (esquemaSelected != null)
                                {
                                    await CambiarSeleccionEsquema(esquemaSelected);
                                }
                                validateButton.HideLoading();
                            }
                            else
                            {
                                toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                                navigationManager?.NavigateTo("/validacion");
                                validateButton.HideLoading();
                            }
                        }
                        else
                        {
                            navigationManager?.NavigateTo("/validacion");
                            validateButton.HideLoading();
                        }


                    }
                }
                else
                {
                    //var esquemaRegistro = new EsquemaVistaValidacionDto
                    //{
                    //    IdOna = onaSelected.IdONA,
                    //    IdEsquema = esquemaSelected.IdEsquema,
                    //    VistaOrigen = nombreSugerido,
                    //    Estado = "A"
                    //};

                    //var success = await iEsquemaService.EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(esquemaRegistro);
                    //if (success)
                    //{
                    //    listaEsquemaVistaColumna = new List<EsquemaVistaColumnaDto>();

                    //    var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
                    //    var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson)
                    //        .OrderBy(c => c.MostrarWebOrden).ToList();

                    //    var filasNoCoinciden = new List<string>();

                    //    foreach (var c in Columnas)
                    //    {
                    //        var vistas = await iDynamicService.GetValueColumna(onaSelected.IdONA, c.NombreHomologado, nombreSugerido);
                    //        // Buscar el elemento correspondiente en vistas
                    //        var vistaCorrespondiente = vistas.FirstOrDefault(v => v.NombreColumna != null && v.NombreColumna.Equals(c.NombreHomologado));

                    //        // Validar coincidencias y agregar solo las filas que coincidan
                    //        if (vistaCorrespondiente != null && vistaCorrespondiente.NombreColumna == c.NombreHomologado)
                    //        {
                    //            vistaCorrespondiente.IsValid = true;

                    //            listaEsquemaVistaColumna.Add(new EsquemaVistaColumnaDto
                    //            {
                    //                ColumnaEsquemaIdH = c.IdHomologacion,
                    //                ColumnaEsquema = vistaCorrespondiente.NombreColumna,
                    //                ColumnaVista = vistaCorrespondiente.NombreColumna,
                    //                ColumnaVistaPK = false,
                    //                Estado = "A"
                    //            });
                    //        }
                    //        else
                    //        {
                    //            filasNoCoinciden.Add(c.NombreHomologado);
                    //        }
                    //    }

                    //    var successRows = await iEsquemaService.GuardarListaEsquemaVistaColumna(listaEsquemaVistaColumna, onaSelected.IdONA, esquemaSelected.IdEsquema);

                    //    if (successRows.registroCorrecto)
                    //    {
                    //        toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                    //        navigationManager?.NavigateTo("/validacion");

                    //        if (filasNoCoinciden.Any())
                    //        {
                    //            toastService?.CreateToastMessage(ToastType.Warning, $"No se pudo guardar las siguientes filas, no existen en la base de datos, no coinciden o tienen espacios en blanco:: {string.Join(", ", filasNoCoinciden)}");
                    //            navigationManager?.NavigateTo("/validacion");
                    //        }
                    //        if (esquemaSelected != null)
                    //        {
                    //            await CambiarSeleccionEsquema(esquemaSelected);
                    //        }
                    //        validateButton.HideLoading();
                    //    }
                    //    else
                    //    {
                    //        toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                    //        navigationManager?.NavigateTo("/validacion");
                    //        validateButton.HideLoading();
                    //    }
                    //}
                    //else
                    //{
                    //    var resultado = await iEsquemaService.GuardarEsquemaVistaValidacionAsync(esquemaRegistro);
                    //    if (resultado != null && resultado.registroCorrecto)
                    //    {
                    //        listaEsquemaVistaColumna = new List<EsquemaVistaColumnaDto>();

                    //        var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
                    //        var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson)
                    //            .OrderBy(c => c.MostrarWebOrden).ToList();

                    //        var filasNoCoinciden = new List<string>();

                    //        foreach (var c in Columnas)
                    //        {
                    //            var vistas = await iDynamicService.GetValueColumna(onaSelected.IdONA, c.NombreHomologado, nombreSugerido);
                    //            // Buscar el elemento correspondiente en vistas
                    //            var vistaCorrespondiente = vistas.FirstOrDefault(v => v.NombreColumna != null && v.NombreColumna.Equals(c.NombreHomologado));

                    //            // Validar coincidencias y agregar solo las filas que coincidan
                    //            if (vistaCorrespondiente != null && vistaCorrespondiente.NombreColumna == c.NombreHomologado)
                    //            {
                    //                vistaCorrespondiente.IsValid = true;

                    //                listaEsquemaVistaColumna.Add(new EsquemaVistaColumnaDto
                    //                {
                    //                    ColumnaEsquemaIdH = c.IdHomologacion,
                    //                    ColumnaEsquema = vistaCorrespondiente.NombreColumna,
                    //                    ColumnaVista = vistaCorrespondiente.NombreColumna,
                    //                    ColumnaVistaPK = false,
                    //                    Estado = "A"
                    //                });
                    //            }
                    //            else
                    //            {
                    //                filasNoCoinciden.Add(c.NombreHomologado);
                    //            }
                    //        }

                    //        var successRows = await iEsquemaService.GuardarListaEsquemaVistaColumna(listaEsquemaVistaColumna, onaSelected.IdONA, esquemaSelected.IdEsquema);

                    //        if (successRows.registroCorrecto)
                    //        {
                    //            toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                    //            navigationManager?.NavigateTo("/validacion");

                    //            if (filasNoCoinciden.Any())
                    //            {
                    //                toastService?.CreateToastMessage(ToastType.Warning, $"No se pudo guardar las siguientes filas, no existen en la base de datos, no coinciden o tienen espacios en blanco: {string.Join(", ", filasNoCoinciden)}");
                    //                navigationManager?.NavigateTo("/validacion");
                    //            }
                    //            if (esquemaSelected != null)
                    //            {
                    //                await CambiarSeleccionEsquema(esquemaSelected);
                    //            }
                    //            validateButton.HideLoading();
                    //        }
                    //        else
                    //        {
                    //            toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                    //            navigationManager?.NavigateTo("/validacion");
                    //            validateButton.HideLoading();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        navigationManager?.NavigateTo("/validacion");
                    //        validateButton.HideLoading();
                    //    }
                    //}
                    navigationManager?.NavigateTo("/validacion");
                    validateButton.HideLoading();
                }
            }
            catch (Exception ex)
            {
                validateButton.HideLoading();
            }
            
        }

        private async Task ExportarExcel()
        {
            objEventTracking.CodigoHomologacionMenu = "/validacion";
            objEventTracking.NombreAccion = "ExportarExcel";
            objEventTracking.NombreControl = "btnExportarExcel";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (listasHevd == null || !listasHevd.Any())
            {
                return;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Configurar licencia para EPPlus

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Esquemas");

            // Agregar encabezados
            worksheet.Cells[1, 1].Value = "Nombre Campo Esquema";
            worksheet.Cells[1, 2].Value = "Nombre Campo Vista";
            worksheet.Cells[1, 3].Value = "Existe";

            int row = 2;
            foreach (var esquema in listasHevd)
            {
                worksheet.Cells[row, 1].Value = esquema.NombreEsquema;
                worksheet.Cells[row, 2].Value = esquema.NombreVista;
                worksheet.Cells[row, 3].Value = esquema.IsValid ? "Sí" : "No";
                row++;
            }

            worksheet.Cells.AutoFitColumns(); // Ajustar automáticamente las columnas

            var fileName = "Esquemas_Export.xlsx";
            var fileBytes = package.GetAsByteArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);

            await JSRuntime.InvokeVoidAsync("downloadExcel", fileName, fileBase64);
        }
        private async Task ExportarPDF()
        {
            if (listasHevd == null || !listasHevd.Any())
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            var document = new Document(iTextSharp.text.PageSize.A4);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var table = new PdfPTable(3) { WidthPercentage = 100 };

            foreach (var header in new[] { "Nombre Campo Esquema", "Nombre Campo Vista", "Existe" })
            {
                table.AddCell(new Phrase(header, font));
            }

            foreach (var esquema in listasHevd)
            {
                table.AddCell(esquema.NombreEsquema ?? "-");
                table.AddCell(esquema.NombreVista ?? "-");
                table.AddCell(esquema.IsValid ? "Sí" : "No");
            }

            document.Add(table);
            document.Close();

            var fileName = "Esquemas_Export.pdf";
            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, "application/pdf", Convert.ToBase64String(memoryStream.ToArray()));
        }

    }
}
