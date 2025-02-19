using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IONAConexionRepository
    {
        /* 
 * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
 * WebApp/Update: Actualiza un registro de ONAConexion en la base de datos.
 */
        bool Update(ONAConexion data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de ONAConexion en la base de datos.
         */
        bool Create(ONAConexion data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de ONAConexion en la base de datos por su identificador único.
         */
        ONAConexion? FindById(int Id);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByIdONA: Busca un registro de ONAConexion en la base de datos por el identificador de ONA.
         */
        ONAConexion? FindByIdONA(int IdONA);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByIdONAAsync: Busca de forma asíncrona un registro de ONAConexion en la base de datos por el identificador de ONA.
         */
        Task<ONAConexion?> FindByIdONAAsync(int IdONA);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de registros de ONAConexion almacenados en la base de datos.
         */
        List<ONAConexion> FindAll();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetOnaConexionByOnaListAsync: Obtiene de forma asíncrona una lista de conexiones de ONA por su identificador.
         */
        List<ONAConexion> GetOnaConexionByOnaListAsync(int IdONA);

    }
}
