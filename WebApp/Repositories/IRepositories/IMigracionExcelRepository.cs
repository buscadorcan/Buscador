using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IMigracionExcelRepository
  {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza un registro de LogMigraci�n en la base de datos.
         */
        bool Update(LogMigracion data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/UpdateAsync: Actualiza un registro de LogMigraci�n de forma as�ncrona en la base de datos.
         */
        Task<bool> UpdateAsync(LogMigracion data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de LogMigraci�n en la base de datos.
         */
        LogMigracion Create(LogMigracion data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateAsync: Crea un nuevo registro de LogMigraci�n de forma as�ncrona en la base de datos.
         */
        Task<LogMigracion> CreateAsync(LogMigracion data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de Migraci�nExcel en la base de datos por su identificador �nico.
         */
        MigracionExcel? FindById(int Id);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de registros de Migraci�nExcel almacenados en la base de datos.
         */
        List<MigracionExcel> FindAll();

    }
}