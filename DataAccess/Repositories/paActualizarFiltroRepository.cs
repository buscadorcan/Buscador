using Dapper;
using DataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccess.Repositories
{
    public class paActualizarFiltroRepository : IpaActualizarFiltroRepository
    {
        private readonly string _connectionString;

        public paActualizarFiltroRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Mssql-CanDb");
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'Mssql-CanDb' no está configurada.");
            }
        }

        // Método para ejecutar el procedimiento almacenado
        public async Task<bool> ActualizarFiltroAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(
                        "[dbo].[paActualizaFiltro]", // Nombre del SP
                        commandType: CommandType.StoredProcedure // Tipo de comando
                    );

                    // Si el procedimiento almacenado devuelve algo, puedes manejarlo aquí
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar el procedimiento almacenado: {ex.Message}");
                return false;
            }
        }
    }
}
