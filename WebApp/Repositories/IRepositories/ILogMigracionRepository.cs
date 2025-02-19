using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface ILogMigracionRepository
  {
        /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/Update: Actualiza un registro de LogMigración en la base de datos.
     */
        bool Update(LogMigracion data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/UpdateAsync: Actualiza un registro de LogMigración de forma asíncrona en la base de datos.
         */
        Task<bool> UpdateAsync(LogMigracion data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de LogMigración en la base de datos.
         */
        LogMigracion Create(LogMigracion data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateAsync: Crea un nuevo registro de LogMigración de forma asíncrona en la base de datos.
         */
        Task<LogMigracion> CreateAsync(LogMigracion data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de LogMigración en la base de datos por su identificador único.
         */
        LogMigracion? FindById(int Id);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de registros de LogMigración almacenados en la base de datos.
         */
        List<LogMigracion> FindAll();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/UpdateDetalle: Actualiza un registro de detalle de LogMigración en la base de datos.
         */
        bool UpdateDetalle(LogMigracionDetalle data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateDetalle: Crea un nuevo registro de detalle de LogMigración en la base de datos.
         */
        LogMigracionDetalle CreateDetalle(LogMigracionDetalle data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateDetalleAsync: Crea un nuevo registro de detalle de LogMigración de forma asíncrona en la base de datos.
         */
        Task<LogMigracionDetalle> CreateDetalleAsync(LogMigracionDetalle data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAllDetalle: Obtiene la lista completa de registros de detalles de LogMigración almacenados en la base de datos.
         */
        List<LogMigracionDetalle> FindAllDetalle();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindDetalleById: Busca los detalles de un registro de LogMigración en la base de datos por su identificador.
         */
        List<LogMigracionDetalle> FindDetalleById(int Id);

    }
}
