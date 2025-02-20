using Microsoft.JSInterop;

namespace ClientApp.Helpers
{
    public static class IJsHelper
    {
        /// <summary>
        /// Clase que ejecuta los mensajes de exito con la propiedad ToastrSuccess
        /// </summary>
        public static async ValueTask ToastrSuccess(this IJSRuntime JSRuntime, string message)
        {
            await JSRuntime.InvokeVoidAsync("ShowToastr", "success", message);
        }

        /// <summary>
        /// Clase que ejecuta los mensajes de error con la propiedad ToastrError
        /// </summary>
        public static async ValueTask ToastrError(this IJSRuntime JSRuntime, string message)
        {
            await JSRuntime.InvokeVoidAsync("ShowToastr", "error", message);
        }
    }

}
