using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using System.Net.Http.Headers;
using System.Text;


namespace ClientApp.Services
{
    public class ServiceAutenticacion : IServiceAutenticacion
    {
        private readonly HttpClient _cliente;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _estadoProveedorAutenticacion;

        public ServiceAutenticacion(HttpClient cliente, 
            ILocalStorageService localStorage,
            AuthenticationStateProvider estadoProveedorAutenticacion)
        {
            _cliente = cliente;
            _localStorage = localStorage;
            _estadoProveedorAutenticacion = estadoProveedorAutenticacion;
        }
        public async Task<RespuestasAPI<UsuarioAutenticacionRespuestaDto>> Acceder(UsuarioAutenticacionDto usuarioAutenticacionDto)
        {
            var content = JsonConvert.SerializeObject(usuarioAutenticacionDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _cliente.PostAsync($"{Inicializar.UrlBaseApi}api/usuarios/login", bodyContent);
            var contentTemp = await response.Content.ReadAsStringAsync();
            var respuesta = JsonConvert.DeserializeObject<RespuestasAPI<UsuarioAutenticacionRespuestaDto>>(contentTemp);

            if (response.IsSuccessStatusCode)
            {
                if (respuesta != null) {
                    var result = respuesta.Result;

                    await _localStorage.SetItemAsync(Inicializar.Token_Local, result?.Token);
                    await _localStorage.SetItemAsync(Inicializar.Datos_Usuario_Local, result?.Usuario?.Email);
                    await _localStorage.SetItemAsync(Inicializar.Datos_Usuario_Rol_Local, result?.Usuario?.IdHomologacionRol);
                    ((AuthStateProvider)_estadoProveedorAutenticacion).NotificarUsuarioLogueado(result?.Token ?? "");
                    _cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result?.Token);
                }
            }

            return respuesta ?? new RespuestasAPI<UsuarioAutenticacionRespuestaDto>();
        }
        public async Task<RespuestasAPI<T>?> Recuperar<T>(UsuarioRecuperacionDto usuarioRecuperacionDto) {
            var content = JsonConvert.SerializeObject(usuarioRecuperacionDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _cliente.PostAsync($"{Inicializar.UrlBaseApi}api/usuarios/recuperar", bodyContent);
            var contentTemp = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RespuestasAPI<T>>(contentTemp);
        }

        public async Task Salir()
        {
            await _localStorage.RemoveItemAsync(Inicializar.Token_Local);
            await _localStorage.RemoveItemAsync(Inicializar.Datos_Usuario_Local);
            ((AuthStateProvider)_estadoProveedorAutenticacion).NotificarUsuarioSalir();
            _cliente.DefaultRequestHeaders.Authorization = null;
        }
    }
}
