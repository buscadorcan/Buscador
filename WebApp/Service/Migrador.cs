
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
//using Npgsql;
//using Microsoft.Data.Sqlite;
using Dapper;
using Newtonsoft.Json;

namespace WebApp.Service.IService
{
    public class Migrador(IEsquemaDataRepository esquemaDataRepository, IEsquemaFullTextRepository esquemaFullTextRepository, IHomologacionRepository homologacionRepository, IEsquemaRepository esquemaRepository, IONAConexionRepository conexionRepository, IConfiguration configuration, IConectionStringBuilderService connectionStringBuilderService, IEsquemaVistaRepository esquemaVistaRepository) : IMigrador
    {
        private IEsquemaDataRepository _repositoryDLO = esquemaDataRepository;
        private IEsquemaFullTextRepository _repositoryOFT = esquemaFullTextRepository;
        private IEsquemaVistaRepository _repositoryEVRP = esquemaVistaRepository;
        private IHomologacionRepository _repositoryH = homologacionRepository;
        private IEsquemaRepository _repositoryHE = esquemaRepository;
        private IONAConexionRepository _repositoryC = conexionRepository;
        private IConectionStringBuilderService _connectionStringBuilderService = connectionStringBuilderService;
        private string connectionString = configuration.GetConnectionString("Mssql-CanDb") ?? throw new InvalidOperationException("La cadena de conexión 'Mssql-CanDb' no está configurada.");
        private ONAConexion? currentConexion = null;
        private int executionIndex = 0;
        private string[] views = [];
        private string[] schemas = [];
        private int[] hids = [];
        private int[] heids = [];
        private string[] vids = [];
        private int[] filters = [];
        private bool deleted = false;
        private bool saveIdVista = false;
        private bool saveIdEnte = false;
        List<string> lstViewNoRegistradas = new List<string>();
        List<string> lstViewRegistradas = new List<string>();
        List<string> lstColumnsNoRegistradas = new List<string>();
        List<string> lstColumnsRegistradas = new List<string>();
        public async Task<bool> MigrarAsync(ONAConexion conexion)
        {

            try
            {
                bool resultado = true;
                // Generar la cadena de conexión
                var connectionString = _connectionStringBuilderService.BuildConnectionString(conexion);
                var isConnectionSuccessful = TestDatabaseConnectionAsync(connectionString, conexion.OrigenDatos);
                if (isConnectionSuccessful)
                {
                    Console.WriteLine("La conexión a la base de datos se probó exitosamente.");
                }
                else
                {
                    Console.WriteLine("Hubo un problema al conectar a la base de datos.");
                }

                List<Esquema> viewRegistradas = new List<Esquema>();

                //recuperar vistas de la tabla esquemas 
                viewRegistradas = _repositoryHE.FindAllWithViews();


                //recuperar vistas externas de acuerdo a la conexion.
                var vistasExternas = await GetExternalViewsAsync(connectionString, conexion.OrigenDatos);

                // Encontrar vistas externas que están en la base de datos
                var vistasRegistradas = vistasExternas.Select(v => v.ToUpper()).Intersect(viewRegistradas.Select(v => v.EsquemaVista.ToUpper())).ToList();

                // Encontrar vistas externas que no están registradas
                var vistasNoRegistradas = vistasExternas.Select(v => v.ToUpper()).Except(viewRegistradas.Select(v => v.EsquemaVista.ToUpper())).ToList();


                foreach (var vista in vistasRegistradas)
                {
                    
                    // Leer las columnas de cada vista registrada
                    var columnas = await GetViewColumnsAsync(connectionString, conexion.OrigenDatos, vista);


                    //Convertir las columnas en formato Json:
                    string columnasJson = JsonConvert.SerializeObject(columnas, Formatting.Indented);

                    int idEsquema = viewRegistradas.Where(v => v.EsquemaVista == vista).FirstOrDefault().IdEsquema;
                    int idOna = conexion.IdONA;

                    //Insertamos los datos en esquemaVista
                    //EsquemaVista esquemaVista = new EsquemaVista
                    //{
                    //    IdONA = idOna,
                    //    IdEsquema = idEsquema,
                    //    VistaOrigen = vista,
                    //    Estado = "A"
                    //};

                    //_repositoryEVRP.Create(esquemaVista);

                    // Recuperar el ID generado
                    //int newIdEsqVista = esquemaVista.IdEsquema;

                    //Insertamos los datos en esquemaData
                    //EsquemaData esquemaData = new EsquemaData
                    //{
                    //    IdEsquemaVista = newIdEsqVista,
                    //    VistaFK = "",
                    //    VistaPK = "1",
                    //    DataEsquemaJson =
                    //};


                    //Insertamos los datos en esquemaFullText
                    EsquemaFullText esquemaFullText = new EsquemaFullText();




                    //lstViewRegistradas.Add(vista);
                }
                // Almacenar las vistas que no están registradas para luego mostrar mensajes
                vistasNoRegistradas.AddRange(lstViewNoRegistradas);

                // Mostrar las vistas no registradas (opcional, para notificación)
                if (vistasNoRegistradas.Any())
                {
                    Console.WriteLine("Vistas no registradas:");
                    vistasNoRegistradas.ForEach(vista => Console.WriteLine(vista));
                }


                return resultado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Importacion de vistas externas de acuerdo a la conexion.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="origenDatos"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<List<string>> GetExternalViewsAsync(string connectionString, string origenDatos)
        {
            try
            {
                // Determinar la consulta según el tipo de base de datos
                var query = origenDatos.ToUpper() switch
                {
                    "SQLSERVER" => "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS",
                    "MYSQL" => "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = DATABASE()",
                    "POSTGRES" => "SELECT table_name FROM information_schema.views WHERE table_schema = 'public'",
                    "SQLLITE" => "SELECT name FROM sqlite_master WHERE type = 'view'",
                    _ => throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.")
                };

                // Crear la conexión según el tipo de base de datos
                var connectionFactories = new Dictionary<string, Func<string, IDbConnection>>
                {
                    { "SQLSERVER", connStr => new SqlConnection(connStr) },
                    { "MYSQL", connStr => new MySqlConnection(connStr) }
                    //{ "POSTGRES", connStr => new NpgsqlConnection(connStr) },
                    //{ "SQLLITE", connStr => new SqliteConnection(connStr) }
                };

                if (connectionFactories.TryGetValue(origenDatos.ToUpper(), out var createConnection))
                {
                    using var connection = createConnection(connectionString);
                    connection.Open();  // Abrir la conexión

                    // Ejecutar la consulta
                    var vistas = await connection.QueryAsync<string>(query);
                    return vistas.ToList();
                }
                else
                {
                    throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener vistas: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetViewColumnsAsync(string connectionString, string origenDatos, string nombreVista)
        {
            try
            {
                // Determinar la consulta según el tipo de base de datos
                var query = origenDatos.ToUpper() switch
                {
                    "SQLSERVER" => $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{nombreVista}'",
                    "MYSQL" => $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '{nombreVista}'",
                    "POSTGRES" => $"SELECT column_name FROM information_schema.columns WHERE table_schema = 'public' AND table_name = '{nombreVista}'",
                    "SQLLITE" => $"SELECT name FROM pragma_table_info('{nombreVista}')",
                    _ => throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.")
                };

                // Crear la conexión según el tipo de base de datos
                var connectionFactories = new Dictionary<string, Func<string, IDbConnection>>
                {
                    { "SQLSERVER", connStr => new SqlConnection(connStr) },
                    { "MYSQL", connStr => new MySqlConnection(connStr) }
                    //{ "POSTGRES", connStr => new NpgsqlConnection(connStr) },
                    //{ "SQLLITE", connStr => new SqliteConnection(connStr) }
                };

                if (connectionFactories.TryGetValue(origenDatos.ToUpper(), out var createConnection))
                {
                    using var connection = createConnection(connectionString);
                    connection.Open();  // Abrir la conexión

                    // Ejecutar la consulta
                    var columnas = await connection.QueryAsync<string>(query);
                    return columnas.ToList();
                }
                else
                {
                    throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener columnas: {ex.Message}");
                return new List<string>();
            }
        }



        public bool TestDatabaseConnectionAsync(string connectionString, string origenDatos)
        {
            try
            {
                // Crear la conexión según el tipo de base de datos
                var connectionFactories = new Dictionary<string, Func<string, IDbConnection>>
                {
                    { "SQLSERVER", connStr => new SqlConnection(connStr) },
                    { "MYSQL", connStr => new MySqlConnection(connStr) }
                    //{ "POSTGRES", connStr => new NpgsqlConnection(connStr) },
                    //{ "SQLLITE", connStr => new SqliteConnection(connStr) }
                };

                if (connectionFactories.TryGetValue(origenDatos.ToUpper(), out var createConnection))
                {
                    using var connection = createConnection(connectionString);
                    connection.Open();  // Abrir la conexión

                    if (connection.State == ConnectionState.Open)
                    {
                        Console.WriteLine("Conexión establecida correctamente.");
                        return true;  // Si la conexión se abrió correctamente
                    }
                    else
                    {
                        Console.WriteLine("No se pudo abrir la conexión.");
                        return false;  // Si la conexión no se pudo abrir
                    }
                }
                else
                {
                    throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al probar la conexión: {ex.Message}");
                return false;
            }
        }
        //public async Task<bool> ProcesarAsync(string viewName)
        //{

        //}

    }
}