using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IMigrador
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/MigrarAsync: Realiza la migración de datos de manera asíncrona utilizando la conexión ONA especificada.
         */
        Task<Boolean> MigrarAsync(ONAConexion conexion);
    }
}
