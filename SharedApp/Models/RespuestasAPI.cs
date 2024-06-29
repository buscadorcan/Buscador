using System.Net;

namespace SharedApp.Models
{
    public class RespuestasAPI
    {
        public RespuestasAPI()
        {
            ErrorMessages = new List<string>();
            StatusCode = HttpStatusCode.OK;
            IsSuccess = true;
        }

        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; }
        public object? Result { get; set; }
    }
}