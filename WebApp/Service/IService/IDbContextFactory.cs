using Microsoft.EntityFrameworkCore;
using SharedApp.Data;

namespace WebApp.Service.IService
{
    public interface IDbContextFactory
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateDbContext: Crea una instancia de DbContext utilizando una cadena de conexión y el tipo de base de datos especificado.
         */
        DbContext CreateDbContext(string connectionString, DatabaseType databaseType);
    }
}
