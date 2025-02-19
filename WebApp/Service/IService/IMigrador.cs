using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IMigrador
    {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/MigrarAsync: Realiza la migraci�n de datos de manera as�ncrona utilizando la conexi�n ONA especificada.
         */
        Task<Boolean> MigrarAsync(ONAConexion conexion);
    }
}
