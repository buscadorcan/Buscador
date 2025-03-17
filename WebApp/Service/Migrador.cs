
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Newtonsoft.Json;
using Npgsql;
using System.Data;
using System.Diagnostics;
using WebApp.Models;
using WebApp.Repositories.IRepositories;

namespace WebApp.Service.IService
{
    public class Migrador(IEsquemaDataRepository esquemaDataRepository, IEsquemaFullTextRepository esquemaFullTextRepository, IHomologacionRepository homologacionRepository, IEsquemaRepository esquemaRepository, IONAConexionRepository conexionRepository, IConfiguration configuration, IConectionStringBuilderService connectionStringBuilderService, IEsquemaVistaRepository esquemaVistaRepository, IEsquemaVistaColumnaRepository esquemaVistaColumnaRepository, ILogMigracionRepository logMigracionRepository, IpaActualizarFiltroRepository ipaActualizarFiltro) : IMigrador
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
        private IpaActualizarFiltroRepository _ipaActualizarFiltro = ipaActualizarFiltro;
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
            Stopwatch stopwatch = new Stopwatch();
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

                // Inicia el temporizador
                stopwatch.Start();

                //recuperar vistas de la tabla esquemasVista
                viewRegistradas = _repositoryEVRP.FindAll().Where(v => v.IdONA == idOna).ToList();

                bool vistaAp;
                bool columnAp;
                var columnasValidas = new List<EsquemaVistaColumna>();
                
                
                //Eliminados los registros anteriores
                _repositoryDLO.DeleteOldRecords(idOna);


                foreach (var vista in viewRegistradas)
                {
                    vistaAp = await ValidarVistaAsync(connectionString, conexion.OrigenDatos, vista.VistaOrigen, idOna);
                    if (vistaAp)
                    {
                        viewColumns = _repositoryEVCRP.FindByIdEsquemaVista(vista.IdEsquemaVista);
                       
                        foreach (var column in viewColumns)
                        {
                            columnAp = await ValidarColumnaEnVistaAsync(connectionString, conexion.OrigenDatos, vista.VistaOrigen, column.ColumnaVista, idOna);
                            if (columnAp)
                            {
                                columnasValidas.Add(column); // Agregar columna válida a la lista
                            }
                            else
                            {
                                Console.WriteLine("Columna no encontrada verificar Log.");

                                var data = new LogMigracion
                                {
                                    IdONA = idOna,
                                    OrigenDatos = conexion.OrigenDatos,
                                    Observacion = $"Columna '{column.ColumnaEsquema}' no encontrada, verificar."
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

                    if (columnasValidas.Any())
                    {
                        // Llamar a ProcesarMigracionAsync para procesar los datos
                        bool resspuesta = await ProcesarVistaConDatosAsync(connectionString, conexion.OrigenDatos, vista.VistaOrigen, columnasValidas, vista.IdEsquemaVista, idOna);

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
                            columnasValidas.Clear();
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

                var resultadoSP = await _ipaActualizarFiltro.ActualizarFiltroAsync();
                if (resultadoSP)
                {
                    Console.WriteLine("El procedimiento almacenado se ejecutó correctamente.");
                }
                else
                {
                    Console.WriteLine("Error al ejecutar el procedimiento almacenado.");
                }

                // Detiene el temporizador
                stopwatch.Stop();
                TimeSpan tiempoTotal = stopwatch.Elapsed;

                // Guardar el tiempo total en el log
                var logTiempo = new LogMigracion
                {
                    IdONA = idOna,
                    OrigenDatos = conexion.OrigenDatos,
                    Observacion = $"Tiempo total de migración: {tiempoTotal.Hours}h {tiempoTotal.Minutes}m {tiempoTotal.Seconds}s {tiempoTotal.Milliseconds}ms."
                };

                _logMigracion.Create(logTiempo);

                return resultado;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<bool> ValidarVistaAsync(string connectionString, string origenDatos, string vista, int idONA)
        {
            try
            {
                // Crear la conexión según el tipo de base de datos
                var connectionFactories = new Dictionary<string, Func<string, IDbConnection>>
                {
                    { "SQLSERVER", connStr => new SqlConnection(connStr) },
                    { "MYSQL", connStr => new MySqlConnection(connStr) },
                    { "POSTGRES", connStr => new Npgsql.NpgsqlConnection(connStr) },
                    { "SQLLITE", connStr => new Microsoft.Data.Sqlite.SqliteConnection(connStr) }
                };

                if (!connectionFactories.TryGetValue(origenDatos.ToUpper(), out var createConnection))
                {
                    throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.");
                }

                using var connection = createConnection(connectionString);
                connection.Open();

                // Generar la consulta adecuada según el tipo de base de datos
                string query = origenDatos.ToUpper() switch
                {
                    "SQLSERVER" => $"SELECT TOP 1 * FROM {vista}",
                    "MYSQL" => $"SELECT * FROM {vista} LIMIT 1",
                    "POSTGRES" => $"SELECT * FROM {vista} LIMIT 1",
                    "SQLLITE" => $"SELECT * FROM {vista} LIMIT 1",
                    _ => throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.")
                };

                // Ejecutar la consulta para validar la vista
                await connection.QueryAsync(query);

                // Si no hay excepciones, la vista existe
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar la vista '{vista}': {ex.Message}");

                var data = new LogMigracion
                {
                    IdONA = idONA,
                    OrigenDatos = origenDatos,
                    Observacion = "Vista no encontrada: " + vista + " verificar."
                };
                _logMigracion.Create(data);

                return false;
            }
        }


        public async Task<bool> ValidarColumnaEnVistaAsync(string connectionString, string origenDatos, string vista, string columna, int idONA)
        {
            try
            {
                // Crear conexión según el tipo de base de datos
                var connectionFactories = new Dictionary<string, Func<string, IDbConnection>>
                {
                    { "SQLSERVER", connStr => new SqlConnection(connStr) },
                    { "MYSQL", connStr => new MySqlConnection(connStr) },
                    { "POSTGRES", connStr => new Npgsql.NpgsqlConnection(connStr) },
                    { "SQLLITE", connStr => new Microsoft.Data.Sqlite.SqliteConnection(connStr) }
                };

                if (!connectionFactories.TryGetValue(origenDatos.ToUpper(), out var createConnection))
                {
                    throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.");
                }

                using var connection = createConnection(connectionString);
                connection.Open();

                // Escapar los nombres de la vista y la columna según el motor de base de datos
                string escapedVista = origenDatos.ToUpper() switch
                {
                    "SQLSERVER" => $"[{vista}]",
                    "MYSQL" => $"`{vista}`",
                    "POSTGRES" => $"\"{vista}\"",
                    "SQLLITE" => $"\"{vista}\"",
                    _ => throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.")
                };

                string escapedColumna = origenDatos.ToUpper() switch
                {
                    "SQLSERVER" => $"[{columna}]",
                    "MYSQL" => $"`{columna}`",
                    "POSTGRES" => $"\"{columna}\"",
                    "SQLLITE" => $"\"{columna}\"",
                    _ => throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.")
                };

                // Crear la consulta para verificar la columna
                string query = origenDatos.ToUpper() switch
                {
                    "SQLSERVER" => $"SELECT TOP 1 {escapedColumna} FROM {escapedVista}",
                    "MYSQL" => $"SELECT {escapedColumna} FROM {escapedVista} LIMIT 1",
                    "POSTGRES" => $"SELECT {escapedColumna} FROM {escapedVista} LIMIT 1",
                    "SQLLITE" => $"SELECT {escapedColumna} FROM {escapedVista} LIMIT 1",
                    _ => throw new NotSupportedException($"Tipo de base de datos '{origenDatos}' no soportado.")
                };

                // Ejecutar la consulta
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


        public async Task<bool> ProcesarVistaConDatosAsync(string connectionString, string origenDatos, string vista, List<EsquemaVistaColumna> columnas, int idEsquemaVista, int idONA)
        {
            try
            {
                // Si el origen de datos es MySQL, se agregan las opciones necesarias a la cadena de conexión
                if (origenDatos.ToUpper() == "MYSQL" && idONA == 2)
                {
                    connectionString += ";AllowZeroDateTime=True;ConvertZeroDateTime=True";
                }

                // Crear conexión según el tipo de base de datos
                var connectionFactories = new Dictionary<string, Func<string, IDbConnection>>
                {
                    { "SQLSERVER", connStr => new SqlConnection(connStr) },
                    { "MYSQL", connStr => new MySqlConnection(connStr) },
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
                    ? $"`{c.ColumnaVista}`"
                    : $"[{c.ColumnaVista}]"));

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
                                Data = diccionarioFila.ContainsKey(col.ColumnaVista)
                                    ? diccionarioFila[col.ColumnaVista]?.ToString()
                                    : null
                            };
                        })
                        .ToList();

                    var json = JsonConvert.SerializeObject(dataEsquemaJson);

                    // Insertar en la tabla EsquemaData
                    var esquemaData = new EsquemaData
                    {
                        IdEsquemaVista = idEsquemaVista,
                        DataEsquemaJson = json,
                        DataFecha = DateTime.Now
                    };

                    _repositoryDLO.Create(esquemaData);
                    var idEsquemaData = esquemaData.IdEsquemaData;

                    // Obtener la lista de homologaciones indexadas
                    var homologacion = _repositoryH.FindByAll()
                        .Where(x => x.Indexar == "S" && x.IdHomologacionGrupo == 1) // El valor 1 para pruebas
                        .Select(x => x.IdHomologacion)
                        .ToHashSet(); // Usar HashSet para búsquedas rápidas

                    // Filtrar las columnas que tienen un IdHomologacion en la lista de homologaciones indexadas
                    var columnasFiltradas = columnas
                        .Where(col => homologacion.Contains(col.ColumnaEsquemaIdH))
                        .ToList();

                    // Insertar en la tabla EsquemaFullText
                    foreach (var col in columnasFiltradas)
                    {
                        // Usar el diccionario previamente construido
                        var diccionarioFila = (IDictionary<string, object>)fila;

                        var esquemaFullText = new EsquemaFullText
                        {
                            IdEsquemaData = idEsquemaData,
                            IdHomologacion = col.ColumnaEsquemaIdH,
                            FullTextData = diccionarioFila.ContainsKey(col.ColumnaVista)
                                ? diccionarioFila[col.ColumnaVista]?.ToString()
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
                var data = new LogMigracion
                {
                    IdONA = idONA,
                    OrigenDatos = origenDatos,
                    Observacion = "Error al procesar los datos de la vista " + vista + " el error: " + ex.Message
                };
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