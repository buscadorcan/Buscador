using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class paActualizarFiltroRepository : IpaActualizarFiltroRepository
    {
        private readonly string _connectionString;
        public paActualizarFiltroRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Método para ejecutar el procedimiento almacenado
        public async Task<bool> ActualizarFiltroAsync(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
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
