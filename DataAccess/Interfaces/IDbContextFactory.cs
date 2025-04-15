using Microsoft.EntityFrameworkCore;
using SharedApp.Data;

namespace DataAccess.Interfaces
{
    public interface IDbContextFactory
    {
        /// <summary>
        /// Crea e inicializa una instancia de <see cref="DbContext"/> utilizando una cadena de conexión y el tipo de base de datos especificado.
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a la base de datos.</param>
        /// <param name="databaseType">Tipo de base de datos a utilizar, definido en <see cref="DatabaseType"/>.</param>
        /// <returns>
        /// Devuelve una instancia de <see cref="DbContext"/> configurada con la cadena de conexión y el tipo de base de datos especificado.
        /// </returns>
        /// <exception cref="ArgumentNullException">Lanzado si <paramref name="connectionString"/> es nulo o vacío.</exception>
        /// <exception cref="InvalidOperationException">Lanzado si <paramref name="databaseType"/> no es compatible.</exception>
        DbContext CreateDbContext(string connectionString, DatabaseType databaseType);

    }
}
