using Microsoft.EntityFrameworkCore;
using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IDynamicRepository
    {
        /* 
 * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
 * WebApp/GetProperties: Obtiene las propiedades de una tabla dentro de una vista espec�fica.
 */
        List<PropiedadesTablaDto> GetProperties(int idONA, string viewName);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetValueColumna: Obtiene los valores de una columna espec�fica dentro de una vista.
         */
        List<PropiedadesTablaDto> GetValueColumna(int idONA, string valueColumn, string viewName);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetViewNames: Obtiene la lista de nombres de vistas disponibles en un ONA.
         */
        List<string> GetViewNames(int idONA);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetListaValidacionEsquema: Obtiene la lista de validaciones de un esquema en un ONA.
         */
        List<EsquemaVistaDto> GetListaValidacionEsquema(int idONA, int idEsquemaVista);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetConexion: Obtiene los datos de conexi�n de un ONA espec�fico.
         */
        ONAConexion GetConexion(int idONA);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/TestDatabaseConnection: Prueba la conexi�n a la base de datos de un ONA.
         */
        bool TestDatabaseConnection(ONAConexion conexion);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/MigrarConexionAsync: Realiza la migraci�n de la conexi�n de un ONA de forma as�ncrona.
         */
        Task<bool> MigrarConexionAsync(int idONA);

    }
}
