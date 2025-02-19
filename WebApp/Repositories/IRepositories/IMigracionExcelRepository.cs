using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IMigracionExcelRepository
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
         * WebApp/FindById: Busca un registro de MigraciónExcel en la base de datos por su identificador único.
         */
        MigracionExcel? FindById(int Id);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de registros de MigraciónExcel almacenados en la base de datos.
         */
        List<MigracionExcel> FindAll();

    }
}