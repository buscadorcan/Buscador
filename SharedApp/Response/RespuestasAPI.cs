using System.Net;

namespace SharedApp.Response
{
    public class RespuestasAPI<T>
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
        public T? Result { get; set; }
    }
}