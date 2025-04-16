namespace Infractruture.Models
{
    /// <summary>
    /// Modelo de datos para obtener las respuesta, estado o errores al consumir los servicios.
    /// que llaman a los endpoints.
    /// registroCorrecto: Nos mandara un true o false de acuerdo a lo que se haga en el repositorio y controlador del back end
    /// Errores: No mandara el error que proviene del back
    /// mensajeError: nos mandara el mensaje de error respectivo si es que hubiera.
    /// </summary>
    public class RespuestaRegistro
    {
        public bool registroCorrecto { get; set; }
        public IEnumerable<string> Errores { get; set; }
        public string mensajeError { get; set; }
    }
}
