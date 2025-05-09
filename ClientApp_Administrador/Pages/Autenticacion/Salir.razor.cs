using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;

namespace ClientAppAdministrador.Pages.Autenticacion
{
    /// <summary>
    /// Clase Salir
    /// </summary>
    public partial class Salir : ComponentBase
    {
        /// <summary>
        /// Servicio de autenticación
        /// </summary>
        [Inject] public IServiceAutenticacion servicioAutenticacion { get; set; } = default!;

        /// <summary>
        /// Navegación
        /// </summary>
        [Inject] public NavigationManager navigationManager { get; set; } = default!;

        /// <summary>
        /// Método OnInitializedAsync
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            if (servicioAutenticacion != null)
            {
                await servicioAutenticacion.Salir();
                navigationManager?.NavigateTo("/");
            }
        }
    }
}
