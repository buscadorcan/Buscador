namespace ClientApp.Models
{
    public class RespuestaRegistro
    {
        public bool registroCorrecto { get; set; }
        public IEnumerable<string> Errores { get; set; }
        public string mensajeError { get; set; }
    }
}
