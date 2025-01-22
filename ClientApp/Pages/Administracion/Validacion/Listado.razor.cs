using BlazorBootstrap;
using SharedApp.Models.Dtos;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using System.Text;
using System.Net.Http;
using ClientApp.Services;

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
        public Services.ToastService? toastService { get; set; }
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Inject]
        private IConexionService? iConexionService { get; set; }
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

        private async Task CambiarSeleccionOna(OnaDto _onaSelected)
        {
            if (esquemaSelected != null)
            {
                onaSelected = _onaSelected;
                //esquemaSelected = null;
                nombreSugerido = "";
                await CambiarSeleccionEsquema(esquemaSelected);

                //listaEsquemasOna = await iEsquemaService.GetEsquemaByOnaAsync(onaSelected.IdONA);

                //listasHevd = new List<EsquemaVistaDto>();
                //if (grid != null)
                //    await grid.RefreshDataAsync();
            }
            else
            {
                nombreSugerido = "";
                onaSelected = _onaSelected;
            }

        }
        private async Task CambiarSeleccionEsquema(EsquemaDto _esquemaSelected)
        {
            var rol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            bool accessRol = rol == "KEY_USER_CAN";

            if (accessRol)
            {
                currentConexion = await iConexionService.GetOnaConexionByOnaAsync(onaSelected.IdONA);
                if (currentConexion != null && currentConexion.OrigenDatos.Equals("EXCEL"))
                {
                    origenDatosValidar = currentConexion.OrigenDatos;
                    enabledCeldas = true;
                }
                else
                {
                    origenDatosValidar = currentConexion.OrigenDatos;
                    enabledCeldas = false;
                }
            }
            else
            {
                int IdOna = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_IdOna_Local);
                currentConexion = await iConexionService.GetOnaConexionByOnaAsync(IdOna);

                if (currentConexion != null && currentConexion.OrigenDatos.Equals("EXCEL"))
                {
                    origenDatosValidar = currentConexion.OrigenDatos;
                    enabledCeldas = true;
                }
                else
                {
                    origenDatosValidar = currentConexion.OrigenDatos;
                    enabledCeldas = false;
                }
            }

            
            esquemaSelected = _esquemaSelected;
            nombreSugerido = esquemaSelected.EsquemaVista;

            var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
            var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson).OrderBy(c => c.MostrarWebOrden).ToList();

            listasHevd = new List<EsquemaVistaDto>();

            var vistas = await iDynamicService.GetListaValidacionEsquema(onaSelected.IdONA, esquemaSelected.IdEsquema);

            foreach (var c in Columnas)
            {
                var vistaCorrespondiente = vistas.FirstOrDefault(n => n.NombreEsquema != null && n.NombreEsquema.Equals(c.NombreHomologado));
                // Contar cuántas vistas cumplen la condición
                var count = vistas.Count(n => n.NombreVista != null && n.NombreVista.Equals(c.NombreHomologado));

                listasHevd.Add(new EsquemaVistaDto
                {
                    NombreEsquema = c.NombreHomologado,
                    NombreVista = vistaCorrespondiente?.NombreVista ?? c.NombreHomologado.Trim(), // Asignar NombreVista de la vista correspondiente
                    IsValid = count > 0 // Asignar IsValid con base en count
                });
            }

            if (grid != null)
                await grid.RefreshDataAsync();
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
            saveButton.ShowLoading("Guardando...");
            if (onaSelected == null || esquemaSelected == null || string.IsNullOrEmpty(nombreSugerido))
            {
                saveButton.HideLoading();
                return;
            }

            var esquemaRegistro = new EsquemaVistaValidacionDto
            {
                //IdEsquemaVista = esquemaSelected.IdEsquemaVista,
                IdOna = onaSelected.IdONA,
                IdEsquema = esquemaSelected.IdEsquema,
                VistaOrigen = nombreSugerido,
                Estado = "A"
            };

            var resultado = await iEsquemaService.GuardarEsquemaVistaValidacionAsync(esquemaRegistro);
            if (resultado != null && resultado.registroCorrecto)
            {
                var success = await iEsquemaService.EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(esquemaRegistro);
                if (success)
                {
                    listaEsquemaVistaColumna = new List<EsquemaVistaColumnaDto>();
                    var vistas = listasHevd.Select(item => new EsquemaVistaDto
                    {
                        NombreEsquema = item.NombreEsquema,
                        NombreVista = item.NombreVista,
                        IsValid = item.IsValid
                    }).ToList();
                    
                    var homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(esquemaSelected.IdEsquema);
                    var Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson).OrderBy(c => c.MostrarWebOrden).ToList();
                    
                    foreach (var c in Columnas)
                    {
                        // Buscar el elemento correspondiente en vistas por NombreEsquema
                        var vistaCorrespondiente = vistas.FirstOrDefault(v => v.NombreEsquema != null && v.NombreEsquema.Equals(c.NombreHomologado));
                    
                        listaEsquemaVistaColumna.Add(new EsquemaVistaColumnaDto
                        {
                            //IdEsquemaVista = esquemaSelected.IdEsquemaVista,
                            ColumnaEsquemaIdH = c.IdHomologacion,
                            ColumnaEsquema = vistaCorrespondiente?.NombreEsquema,
                            ColumnaVista = vistaCorrespondiente?.NombreVista, // Asigna el NombreVista correspondiente
                            ColumnaVistaPK = false,
                            Estado = "A"
                        });
                    }
                    
                    var successRows = await iEsquemaService.GuardarListaEsquemaVistaColumna(listaEsquemaVistaColumna, onaSelected.IdONA, esquemaSelected.IdEsquema);
                    if (successRows.registroCorrecto)
                    {
                        toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                        navigationManager?.NavigateTo("/validacion");
                    }
                    else
                    {
                        toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                        navigationManager?.NavigateTo("/validacion");
                        saveButton.HideLoading();
                    }                  
                }
                else
                {
                    toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                    navigationManager?.NavigateTo("/validacion");
                    saveButton.HideLoading();
                }
            }
            else
            {
                toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                navigationManager?.NavigateTo("/validacion");
                saveButton.HideLoading();
            }
            saveButton.HideLoading();
        }

        private async Task ValidarDatos()
        {
            try
            {
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
                            await CambiarSeleccionEsquema(esquemaSelected);
                            saveButton.HideLoading();
                            validateButton.HideLoading();
                        }
                        else
                        {
                            toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                            navigationManager?.NavigateTo("/validacion");
                            saveButton.HideLoading();
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
                                await CambiarSeleccionEsquema(esquemaSelected);
                                saveButton.HideLoading();
                                validateButton.HideLoading();
                            }
                            else
                            {
                                toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                                navigationManager?.NavigateTo("/validacion");
                                saveButton.HideLoading();
                                validateButton.HideLoading();
                            }
                        }
                        else
                        {
                            navigationManager?.NavigateTo("/validacion");
                            saveButton.HideLoading();
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
                    //            toastService?.CreateToastMessage(ToastType.Warning, $"No se pudo guardar las siguientes filas no coinciden o tienen espacios en blanco: {string.Join(", ", filasNoCoinciden)}");
                    //            navigationManager?.NavigateTo("/validacion");
                    //        }
                    //        await CambiarSeleccionEsquema(esquemaSelected);
                    //        saveButton.HideLoading();
                    //        validateButton.HideLoading();
                    //    }
                    //    else
                    //    {
                    //        toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                    //        navigationManager?.NavigateTo("/validacion");
                    //        saveButton.HideLoading();
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
                    //                toastService?.CreateToastMessage(ToastType.Warning, $"No se pudo guardar las siguientes filas no coinciden o tienen espacios en blanco: {string.Join(", ", filasNoCoinciden)}");
                    //                navigationManager?.NavigateTo("/validacion");
                    //            }
                    //            await CambiarSeleccionEsquema(esquemaSelected);
                    //            saveButton.HideLoading();
                    //            validateButton.HideLoading();
                    //        }
                    //        else
                    //        {
                    //            toastService?.CreateToastMessage(ToastType.Danger, "No se pudo guardar");
                    //            navigationManager?.NavigateTo("/validacion");
                    //            saveButton.HideLoading();
                    //            validateButton.HideLoading();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        navigationManager?.NavigateTo("/validacion");
                    //        saveButton.HideLoading();
                    //        validateButton.HideLoading();
                    //    }
                    //}
                    navigationManager?.NavigateTo("/validacion");
                    saveButton.HideLoading();
                    validateButton.HideLoading(); 
                }
            }
            catch (Exception)
            {
                saveButton.HideLoading();
                validateButton.HideLoading();
            }
            
        }

    }
}
