using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Pages
{
    /// <summary>
    /// Componente parcial para redireccionar al acceso.
    /// </summary>
    public partial class RedireccionarAlAcceso : ComponentBase
    {
        /// <summary>
        /// Navegación.
        /// </summary>
        [Inject] private NavigationManager navigationManager { get; set; } = default!;

        /// <summary>
        /// Estado del proveedor de autenticación.
        /// </summary>
        [CascadingParameter] private Task<AuthenticationState> estadoProveedorAutenticacion { get; set; } = default!;

        /// <summary>
        /// Indicador de no autorizado.
        /// </summary>
        private bool noAutorizado { get; set; } = false;

        /// <summary>
        /// Método de inicialización de datos.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            var estadoAutorizacion = await estadoProveedorAutenticacion;

            if (estadoAutorizacion.User == null)
            {
                var returnUrl = navigationManager.ToBaseRelativePath(navigationManager.Uri);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    navigationManager.NavigateTo("Acceder", true);
                }
                else
                {
                    navigationManager.NavigateTo($"Acceder?returnUrl={returnUrl}", true);
                }
            }
            else
            {
                noAutorizado = true;
            }
        }
    }
}
