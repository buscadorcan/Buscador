
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Npgsql;
using System.Data;
using WebApp.Models;
using WebApp.Repositories.IRepositories;

namespace WebApp.Service.IService
{
    public class Migrador(IEsquemaDataRepository esquemaDataRepository, IEsquemaFullTextRepository esquemaFullTextRepository, IHomologacionRepository homologacionRepository, IEsquemaRepository esquemaRepository, IONAConexionRepository conexionRepository, IConfiguration configuration, IConectionStringBuilderService connectionStringBuilderService, IEsquemaVistaRepository esquemaVistaRepository, IEsquemaVistaColumnaRepository esquemaVistaColumnaRepository, ILogMigracionRepository logMigracionRepository) : IMigrador
    {
        private IEsquemaDataRepository _repositoryDLO = esquemaDataRepository;
        private IEsquemaFullTextRepository _repositoryOFT = esquemaFullTextRepository;
        private IEsquemaVistaRepository _repositoryEVRP = esquemaVistaRepository;
        private IEsquemaVistaColumnaRepository _repositoryEVCRP = esquemaVistaColumnaRepository;
        private IHomologacionRepository _repositoryH = homologacionRepository;
        private IEsquemaRepository _repositoryHE = esquemaRepository;
        private IONAConexionRepository _repositoryC = conexionRepository;
        private IConectionStringBuilderService _connectionStringBuilderService = connectionStringBuilderService;
        private ILogMigracionRepository _logMigracion = logMigracionRepository;
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
            List<EsquemaVista> viewRegistradas = new List<EsquemaVista>();
            List<EsquemaVistaColumna> viewColumns = new List<EsquemaVistaColumna>();
            try
            {
                bool resultado = true;
                int idOna = conexion.IdONA;
                // Generar la cadena de conexión
                var connectionString = _connectionStringBuilderService.BuildConnectionString(conexion);
                var isConnectionSuccessful = TestDatabaseConnectionAsync(connectionString, conexion.OrigenDatos);
                if (isConnectionSuccessful)
                {
                    Console.WriteLine("La conexión a la base de datos se probó exitosamente.");
                    var data = new LogMigracion
                    {
                        IdONA = idOna,
                        OrigenDatos = conexion.OrigenDatos,
                        Observacion = "Conexion inicializada con exito"
                    };
                    _logMigracion.Create(data);
                }
                else
                {
                    var data = new LogMigracion
                    {
                        IdONA = idOna,
                        OrigenDatos = conexion.OrigenDatos,
                        Observacion = "Hubo un problema al conectar a la base de datos."
                    };
                    _logMigracion.Create(data);
                    Console.WriteLine("Hubo un problema al conectar a la base de datos.");
                }



                //recuperar vistas de la tabla esquemasVista
                viewRegistradas = _repositoryEVRP.FindAll().Where(v => v.IdONA == idOna).ToList();

                bool vistaAp;
                bool columnAp;
                bool procesar = true;
                foreach (var vista in viewRegistradas)
                {
                    vistaAp = await ValidarVistaAsync(connectionString, conexion.OrigenDatos, vista.VistaOrigen);
                    if (vistaAp)
                    {
                        viewColumns = _repositoryEVCRP.FindByIdEsquemaVista(vista.IdEsquemaVista);
                        foreach (var column in viewColumns)
                        {
                            columnAp = await ValidarColumnaEnVistaAsync(connectionString, conexion.OrigenDatos, vista.VistaOrigen, column.ColumnaEsquema);
                            if (!columnAp)
                            {
                                procesar = false;
                                Console.WriteLine("Columna no encontrada verificar Log.");

                                var data = new LogMigracion
                                {
                                    IdONA = idOna,
                                    OrigenDatos = conexion.OrigenDatos,
                                    Observacion = "Columna no encontrada verificar."
                                };
                                _logMigracion.Create(data);
                            }
                        }
                        if (procesar)
                        {
                            // Llamar a ProcesarMigracionAsync para procesar los datos
                            bool resspuesta = await ProcesarVistaConDatosAsync(connectionString, conexion.OrigenDatos, vista.VistaOrigen, viewColumns, vista.IdEsquemaVista);

                            if (resspuesta)
                            {
                                Console.WriteLine($"Vista '{vista.VistaOrigen}' procesada exitosamente.");
                                var data = new LogMigracion
                                {
                                    IdONA = idOna,
                                    OrigenDatos = conexion.OrigenDatos,
                                    Observacion = "Vista: " + vista.VistaOrigen + " procesada exitosamente."
                                };
                                _logMigracion.Create(data);
                            }
                            else
                            {
                                Console.WriteLine($"Error al procesar la vista '{vista.VistaOrigen}'. Verificar log.");
                                var data = new LogMigracion
                                {
                                    IdONA = idOna,
                                    OrigenDatos = conexion.OrigenDatos,
                                    Observacion = "Error al procesar la vista: " + vista.VistaOrigen + " verificar."
                                };
                                _logMigracion.Create(data);
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("Vista no encontrada verificar Log.");
                        var data = new LogMigracion
                        {
                            IdONA = idOna,
                            OrigenDatos = conexion.OrigenDatos,
                            Observacion = "Vista no encontrada: " + vista.VistaOrigen + " verificar."
                        };
                        _logMigracion.Create(data);
                    }

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
        public async Task<bool> ValidarVistaAsync(string connectionString, string origenDatos, string vista)
        {
            try
            {
                // Crear la conexión según el tipo de base de datos
                var connectionFactories = new Dictionary<string, Func<string, IDbConnection>>
                {
                    { "SQLSERVER", connStr => new SqlConnection(connStr) },
                    { "MYSQL", connStr => new MySql.Data.MySqlClient.MySqlConnection(connStr) },
                    { "POSTGRES", connStr => new Npgsql.NpgsqlConnection(connStr) },
                    { "SQLLITE", connStr => new Microsoft.Data.Sqlite.SqliteConnection(connStr) }
                };

                if (!connectionFactories.TryGetValue(origenDatos.ToUpper(), out var createConnection))
                {
                    throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.");

                }

                using var connection = createConnection(connectionString);
                connection.Open();

                // Realizar el SELECT * para validar la vista
                var query = $"SELECT TOP 1 * FROM {vista}"; // Agrega LIMIT para evitar cargar todos los datos
                await connection.QueryAsync(query);

                // Si no hay excepciones, la vista existe
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar la vista '{vista}': {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidarColumnaEnVistaAsync(string connectionString, string origenDatos, string vista, string columna)
        {
            try
            {
                // Crear conexión según el tipo de base de datos
                var connectionFactories = new Dictionary<string, Func<string, IDbConnection>>
                {
                    { "SQLSERVER", connStr => new SqlConnection(connStr) },
                    { "MYSQL", connStr => new MySql.Data.MySqlClient.MySqlConnection(connStr) },
                    { "POSTGRES", connStr => new Npgsql.NpgsqlConnection(connStr) },
                    { "SQLLITE", connStr => new Microsoft.Data.Sqlite.SqliteConnection(connStr) }
                };

                if (!connectionFactories.TryGetValue(origenDatos.ToUpper(), out var createConnection))
                {
                    throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.");
                }

                using var connection = createConnection(connectionString);
                connection.Open();

                // Consulta básica para validar la columna
                var query = origenDatos.ToUpper() switch
                {
                    "SQLSERVER" => $"SELECT TOP 1 [{columna}] FROM {vista}",
                    "MYSQL" => $"SELECT TOP 1 '{columna}' FROM {vista}",
                    "POSTGRES" => $"SELECT TOP 1 \"{columna}\" FROM {vista}",
                    "SQLLITE" => $"SELECT TOP 1 \"{columna}\" FROM {vista}",
                    _ => throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.")
                };

                // Ejecutar el SELECT
                await connection.QueryAsync(query);

                // Si no hay excepciones, la columna existe
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar la columna '{columna}' en la vista '{vista}': {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ProcesarVistaConDatosAsync(string connectionString, string origenDatos, string vista, List<EsquemaVistaColumna> columnas, int idEsquemaVista)
        {
            try
            {
                // Crear conexión según el tipo de base de datos
                var connectionFactories = new Dictionary<string, Func<string, IDbConnection>>
                {
                    { "SQLSERVER", connStr => new SqlConnection(connStr) },
                    { "MYSQL", connStr => new MySql.Data.MySqlClient.MySqlConnection(connStr) },
                    { "POSTGRES", connStr => new Npgsql.NpgsqlConnection(connStr) },
                    { "SQLLITE", connStr => new Microsoft.Data.Sqlite.SqliteConnection(connStr) }
                };

                if (!connectionFactories.TryGetValue(origenDatos.ToUpper(), out var createConnection))
                {
                    throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.");
                }

                using var connection = createConnection(connectionString);
                connection.Open();

                // Construir el SELECT dinámico
                var columnasQuery = string.Join(", ", columnas.Select(c =>
                    origenDatos.ToUpper() == "MYSQL" || origenDatos.ToUpper() == "POSTGRES"
                    ? $"`{c.ColumnaEsquema}`"
                    : $"[{c.ColumnaEsquema}]"));

                var query = $"SELECT {columnasQuery} FROM {vista}";

                // Ejecutar la consulta
                var filas = (await connection.QueryAsync(query)).ToList();

                if (!filas.Any())
                {
                    Console.WriteLine($"La vista '{vista}' no contiene datos.");
                    return false;
                }

                // Procesar cada fila
                foreach (var fila in filas)
                {
                    // Construir el JSON con IdHomologacion y Data
                    var dataEsquemaJson = columnas
                        .Select(col =>
                        {
                            var diccionarioFila = (IDictionary<string, object>)fila;
                            return new
                            {
                                IdHomologacion = col.ColumnaEsquemaIdH,
                                Data = diccionarioFila.ContainsKey(col.ColumnaEsquema)
                                    ? diccionarioFila[col.ColumnaEsquema]?.ToString()
                                    : null
                            };
                        })
                        .ToList();

                    var json = JsonConvert.SerializeObject(dataEsquemaJson);


                    //Eliminados los registros anteriores
                    _repositoryDLO.DeleteOldRecords(idEsquemaVista);

                    // Insertar en la tabla EsquemaData
                    var esquemaData = new EsquemaData
                    {
                        IdEsquemaVista = idEsquemaVista,
                        DataEsquemaJson = json,
                        DataFecha = DateTime.Now
                    };

                    _repositoryDLO.Create(esquemaData);
                    var idEsquemaData = esquemaData.IdEsquemaData;

                    // Insertar en la tabla EsquemaFullText
                    foreach (var col in columnas)
                    {
                        // Usar el diccionario previamente construido
                        var diccionarioFila = (IDictionary<string, object>)fila;

                        var esquemaFullText = new EsquemaFullText
                        {
                            IdEsquemaData = idEsquemaData,
                            IdHomologacion = col.ColumnaEsquemaIdH,
                            FullTextData = diccionarioFila.ContainsKey(col.ColumnaEsquema)
                                ? diccionarioFila[col.ColumnaEsquema]?.ToString()
                                : null // En caso de que no exista la columna en la fila
                        };

                        _repositoryOFT.Create(esquemaFullText);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar los datos de la vista '{vista}': {ex.Message}");
                return false;
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
                    { "MYSQL", connStr => new MySqlConnection(connStr) },
                    { "POSTGRES", connStr => new NpgsqlConnection(connStr) },
                    { "SQLLITE", connStr => new SqliteConnection(connStr) }
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

    }
}