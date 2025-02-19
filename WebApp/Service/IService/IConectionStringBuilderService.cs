
using WebApp.Models;

namespace WebApp.Service.IService
{
  public interface IConectionStringBuilderService
  {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/BuildConnectionString: Construye una cadena de conexión en base a los datos proporcionados de una conexión ONA.
         */
        string BuildConnectionString(ONAConexion conexion);
  }
}
