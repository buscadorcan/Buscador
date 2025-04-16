using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SharedApp.Helpers;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Infractruture.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _cliente;
        private readonly ILocalStorageService _localStorageService;

        public AuthStateProvider(HttpClient cliente, ILocalStorageService localStorageService)
        {
            _cliente = cliente;
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>(Inicializar.Token_Local);
            if (token == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            _cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
        }


        public void NotificarUsuarioLogueado(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotificarUsuarioSalir()
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
