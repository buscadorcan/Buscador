namespace WebApp.Service.IService
{
  
  public interface ISqlServerDbContextFactory
  {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateDbContext: Crea y devuelve una nueva instancia de SqlServerDbContext para la gestión de la base de datos en SQL Server.
         */
        SqlServerDbContext CreateDbContext();
  }
}
