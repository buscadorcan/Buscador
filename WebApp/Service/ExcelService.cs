
using ExcelDataReader;
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;

namespace WebApp.Service.IService
{
    public class ExcelService(
      IONARepository onaRepository,
      IEsquemaRepository esquemaRepository,
      IEsquemaVistaRepository esquemaVistaRepository,
      IEsquemaVistaColumnaRepository esquemaVistaColumnaRepository,
      IEsquemaDataRepository esquemaDataRepository,
      IEsquemaFullTextRepository esquemaFullTextRepository,
      IHomologacionRepository homologacionRepository,
      IMigracionExcelRepository migracionExcelRepository,
      ILogMigracionRepository logMigracionRepository,
      IONAConexionRepository conexionRepository,
      IpaActualizarFiltroRepository ipaActualizarFiltro
      ) : IExcelService
    {
        private IONARepository _repositoryO = onaRepository;
        private IEsquemaRepository _repositoryE = esquemaRepository;
        private IEsquemaVistaRepository _repositoryEV = esquemaVistaRepository;
        private IEsquemaVistaColumnaRepository _repositoryEVC = esquemaVistaColumnaRepository;
        private IEsquemaDataRepository _repositoryED = esquemaDataRepository;
        private IEsquemaFullTextRepository _repositoryEFT = esquemaFullTextRepository;
        private IHomologacionRepository _repositoryH = homologacionRepository;
        private IMigracionExcelRepository _repositoryME = migracionExcelRepository;
        private ILogMigracionRepository _repositoryLM = logMigracionRepository;
        private IONAConexionRepository _repositoryOC = conexionRepository;
        private int migration_cnt = 0;
        private int executionIndex = 0;
        private bool deleted = false;
        private JArray currentSchema = new JArray();
        private List<EsquemaVistaColumna> currentFields = new List<EsquemaVistaColumna>();
        private LogMigracion? currentLogMigracion = null;
        private LogMigracionDetalle? currentLogMigracionDetalle = null;
        private ONA? currentONA = null;
        private ONAConexion? currentConexion = null;
        Esquema? currentEsquema = null;
        private string idEnteName = " IdOrganizacion";
        private string[] errors = Array.Empty<string>();
        private IpaActualizarFiltroRepository _ipaActualizarFiltro = ipaActualizarFiltro;

        //public Boolean ImportarExcel(string path, MigracionExcel? migracion) 
        //{
        //  try {
        //    if (migracion == null) {
        //      migracion = new MigracionExcel();
        //      migracion.MigracionEstado = "PROCESSING";
        //      migracion.ExcelFileName = path.Split("/").Last();
        //      migracion = _repositoryME.Create(migracion);
        //    } else {
        //      migracion.MigracionEstado = "PROCESSING";
        //      // var result = true;
        //      _repositoryME.Update(migracion);
        //    }
        //    var result = Leer(path);
        //    if(result) {
        //      migracion.MigracionEstado = "SUCCESS";
        //    } else {
        //      migracion.MigracionEstado = "ERROR";
        //      migracion.MensageError = "Algo sslió mal en la migración";
        //    }
        //    _repositoryME.Update(migracion);

        //    return result;
        //  } catch (Exception e) {
        //    Console.WriteLine(e);
        //    errors = errors.Append(e.Message).ToArray();
        //    migracion.MigracionEstado = "ERROR";
        //    migracion.MensageError = string.Join(", ", errors);
        //    _repositoryME.Update(migracion);
        //    if (currentLogMigracion != null) {
        //      currentLogMigracion.Final = DateTime.Now;
        //      currentLogMigracion.Estado = "ERROR";
        //      currentLogMigracion.Observacion = string.Join(", ", errors);
        //      currentLogMigracion.EsquemaFilas = migration_cnt;
        //      _repositoryLM.Update(currentLogMigracion);
        //    }
        //    return false;
        //  }
        //}
        public async Task<Boolean> ImportarExcel(string path, LogMigracion? migracion, int idOna)
        {
            try
            {
                if (migracion == null)
                {
                    migracion = new LogMigracion();
                    migracion.Estado = "PROCESSING";
                    migracion.ExcelFileName = path.Split("/").Last();
                    migracion = _repositoryME.Create(migracion);
                }
                else
                {
                    migracion.Estado = "PROCESSING";
                    migracion.ExcelFileName = path.Split("/").Last();
                    // var result = true;
                    _repositoryME.Update(migracion);
                }
                var result = await Leer(path, idOna);
                if (result)
                {
                    migracion.ExcelFileName = path.Split("/").Last();
                    migracion.Estado = "SUCCESS";
                }
                else
                {
                    migracion.ExcelFileName = path.Split("/").Last();
                    migracion.Estado = "ERROR";
                    migracion.Observacion = "Algo sslió mal en la migración";
                }
                _repositoryME.Update(migracion);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                errors = errors.Append(e.Message).ToArray();
                migracion.Estado = "ERROR";
                migracion.Observacion = string.Join(", ", errors);
                _repositoryME.Update(migracion);
                if (currentLogMigracion != null)
                {
                    currentLogMigracion.Final = DateTime.Now;
                    currentLogMigracion.Estado = "ERROR";
                    currentLogMigracion.Observacion = string.Join(", ", errors);
                    currentLogMigracion.EsquemaFilas = migration_cnt;
                    _repositoryLM.Update(currentLogMigracion);
                }
                return false;
            }
        }
        public async Task<Boolean> Leer(string fileSrc, int idOna)
        {
            try
            {
                bool resultado = true;
                Stopwatch stopwatch = new Stopwatch();

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = File.Open(fileSrc, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var configuration = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        };

                        var DataSet = reader.AsDataSet(configuration);

                        if (DataSet.Tables.Count > 0)
                        {
                            DateTime StartTime = DateTime.Now;
                            var migrationValue = DataSet.Tables[1].Rows[0][0].ToString();
                            //currentONA = _repositoryO.FindBySiglas(migrationValue);
                            currentONA = _repositoryO.FindById(idOna);
                            if (currentONA == null)
                            {

                                throw new Exception($"Error: ONA {migrationValue} no encontrada en la base de datos");
                            }
                            Console.WriteLine("Current ONA: " + currentONA.RazonSocial);
                            currentConexion = _repositoryOC.FindByIdONA(currentONA.IdONA);
                            if (currentConexion == null)
                            {
                                throw new Exception($"Error: Conexion no encontrada en la base de datos para ONA {currentONA.RazonSocial}");
                            }
                            foreach (DataTable dataTable in DataSet.Tables)
                            {
                                LogMigracion logMigracion = new LogMigracion();
                                string sheetName = dataTable.TableName;
                                currentEsquema = _repositoryE.FindByViewName(sheetName);
                                if (currentEsquema == null)
                                {
                                    errors = errors.Append($"Error: Esquema {sheetName} no encontrado en la base de datos para ONA {currentONA.RazonSocial}").ToArray();
                                    continue;
                                }
                                //EsquemaVista? esquemaVista = repositoryEV.FindByIdEsquema(currentEsquema.IdEsquema);
                                EsquemaVista? esquemaVista = _repositoryEV._FindByIdEsquema(currentEsquema.IdEsquema, currentONA.IdONA);

                                if (esquemaVista == null)
                                {
                                    errors = errors.Append($"Error: Esquema Vista {sheetName} no encontrado en la base de datos para ONA {currentONA.RazonSocial}").ToArray();
                                    continue;
                                }

                                logMigracion.IdONA = currentConexion.IdONA;
                                logMigracion.VistaOrigen = esquemaVista.VistaOrigen;
                                logMigracion.VistaFilas = dataTable.Rows.Count;
                                logMigracion.EsquemaFilas = 0;
                                logMigracion.EsquemaId = currentEsquema.IdEsquema;
                                logMigracion.EsquemaVista = currentEsquema.EsquemaVista;
                                logMigracion.Inicio = StartTime;

                                logMigracion.ExcelFileName = fileSrc.Split("/").Last();
                                currentLogMigracion = _repositoryLM.Create(logMigracion);

                                //currentFields = _repositoryEVC.FindByIdEsquemaVista(currentEsquema.IdEsquema);
                                currentFields = _repositoryEVC.FindByIdEsquemaVistaOna(esquemaVista.IdEsquemaVista, currentONA.IdONA);

                                if (currentFields.Count == 0)
                                {
                                    errors = errors.Append($"Error: No se ha encontrado campos a migrar para vista {esquemaVista.VistaOrigen} configurados para ONA {currentONA.RazonSocial}").ToArray();
                                    continue;
                                }

                                executionIndex = DataSet.Tables.IndexOf(dataTable);
                                Console.WriteLine("Execution Index: " + executionIndex + " Sheet: " + dataTable.TableName);

                                DeleteDataAntigua(idOna);

                                for (int i = 0; i < dataTable.Rows.Count; i++)
                                {
                                    // Se borra los datos antiguos de todo el esquema que se está migrando y se los vuelve a cargar
                                    // Console.WriteLine($"Deleting old records for {esquema.IdHomologacionEsquema}");

                                    //deleteOldRecords(esquemaVista.IdEsquemaVista);
                                    EsquemaData esquemaData = addEsquemaData(dataTable, i, esquemaVista.IdEsquemaVista);
                                    addEsquemaFullText(dataTable, i, esquemaData.IdEsquemaData);
                                    currentLogMigracion.Final = DateTime.Now;
                                    currentLogMigracion.Estado = "OK";
                                    currentLogMigracion.EsquemaFilas = migration_cnt;
                                    _repositoryLM.Update(currentLogMigracion);
                                }
                            }

                            #region Funcion para actualizar
                            bool resultadoSP = await _ipaActualizarFiltro.ActualizarFiltroAsync();
                            if (resultadoSP)
                            {
                                Console.WriteLine("El procedimiento paActualizaFiltro almacenado se ejecutó correctamente.");
                            }
                            else
                            {
                                Console.WriteLine("Error al ejecutar el procedimiento almacenado.");
                            }

                            // Detiene el temporizador
                            //stopwatch.Stop();
                            //TimeSpan tiempoTotal = stopwatch.Elapsed;

                            //// Guardar el tiempo total en el log
                            //var logTiempo = new LogMigracion
                            //{
                            //    IdONA = idOna,
                            //    OrigenDatos = currentConexion.OrigenDatos,
                            //    Observacion = $"Tiempo total de migración: {tiempoTotal.Hours}h {tiempoTotal.Minutes}m {tiempoTotal.Seconds}s {tiempoTotal.Milliseconds}ms."
                            //};
                            //_repositoryLM.Create(logTiempo);
                            #endregion

                            return resultado;
                        }
                        else
                        {
                            Console.WriteLine("No tables found");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        EsquemaData addEsquemaData(DataTable dataTable, int row, int esquemaVistaId)
        {
            EsquemaData canDataSet = new EsquemaData
            {
                IdEsquemaVista = esquemaVistaId,
                DataEsquemaJson = buildEsquemaDataSetJson(dataTable, row)
            };
            migration_cnt++;
            return _repositoryED.Create(canDataSet);
        }

        bool addEsquemaFullText(DataTable dataTable, int row, int esquemaDataId)
        {
            bool result = true;
            foreach (EsquemaVistaColumna currentField in currentFields)
            {
                if (currentField.ColumnaEsquemaIdH < 1)
                {
                    errors = errors.Append($"Error: Columna {currentField.ColumnaEsquema} no encontrada en la base de datos para ONA {currentONA.RazonSocial}").ToArray();
                    continue;
                }

                int currentFieldIndex = Array.FindIndex(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray(), c => c == currentField.ColumnaVista);
                if (currentFieldIndex == -1)
                {
                    errors = errors.Append($"Error: Columna {currentField.ColumnaVista} no encontrada en el archivo de migración para ONA {currentONA.RazonSocial}").ToArray();
                    continue;
                }

                string? currentValue = dataTable.Rows[row][currentFieldIndex].ToString();
                if (string.IsNullOrEmpty(currentValue))
                {
                    errors = errors.Append($"Error: Columna {currentField.ColumnaVista} no puede ser nula o vacía para ONA {currentONA.RazonSocial}").ToArray();
                    continue;
                }

                EsquemaFullText newCanFullText = new EsquemaFullText
                {
                    IdEsquemaData = esquemaDataId,
                    IdHomologacion = currentField.ColumnaEsquemaIdH,
                    FullTextData = currentValue,
                };

                result = _repositoryEFT.Create(newCanFullText) != null ? result : false;
            }
            return result;
        }

        string buildEsquemaDataSetJson(DataTable dataTable, int row)
        {
            JArray data = new JArray();
            foreach (EsquemaVistaColumna currentField in currentFields)
            {
                addLogDetail(currentField);
                if (currentField.ColumnaEsquemaIdH < 1)
                {
                    errors = errors.Append($"Error: Columna {currentField.ColumnaEsquema} no encontrada en la base de datos para ONA {currentONA.RazonSocial}").ToArray();
                    continue;
                }

                int currentFieldIndex = Array.FindIndex(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray(), c => c == currentField.ColumnaVista);
                if (currentFieldIndex == -1)
                {
                    errors = errors.Append($"Error: Columna {currentField.ColumnaVista} no encontrada en el archivo de migración para ONA {currentONA.RazonSocial}").ToArray();
                    continue;
                }

                // Verificar si la columna es orgUrlCertificado (sin sensibilidad a mayúsculas/minúsculas)
                //string urlCertificado = null;
                //string columnMatch = dataTable.Columns.Cast<DataColumn>()
                //                          .FirstOrDefault(c => string.Equals(c.ColumnName, "orgUrlCertificado", StringComparison.OrdinalIgnoreCase))
                //                          ?.ColumnName;

                //if (!string.IsNullOrEmpty(columnMatch))
                //{
                //    urlCertificado = dataTable.Rows[row][columnMatch].ToString();
                //}

                data.Add(new JObject
                {
                    ["IdHomologacion"] = currentField.ColumnaEsquemaIdH,
                    ["Data"] = dataTable.Rows[row][currentFieldIndex].ToString()
                    //["UrlCertificado"] = urlCertificado
                });
            }

            return data.ToString();
        }

        //bool deleteOldRecords(int idEsquemaVista)
        //{
        //    Console.WriteLine($"Deleting old records for {idEsquemaVista}");
        //    if (deleted)
        //    {
        //        Console.WriteLine("Already deleted");
        //        return true;
        //    }
        //    deleted = true;
        //    Console.WriteLine("Predelete");
        //    return _repositoryED.DeleteOldRecords(idEsquemaVista);
        //}
        bool DeleteDataAntigua(int idOna)
        {
            Console.WriteLine($"Deleting old records for {idOna}");
            if (deleted)
            {
                Console.WriteLine("Already deleted");
                return true;
            }
            deleted = true;
            Console.WriteLine("Predelete");
            return _repositoryED.DeleteDataAntigua(idOna);
        }

        //bool deleteOldRecord(string idVista, string idOrganizacion, int idHomologacionEsquema, int idConexion)
        //{
        //    return _repositoryED.DeleteOldRecord(idVista, idOrganizacion, idConexion, idHomologacionEsquema);
        //}

        bool addLogDetail(EsquemaVistaColumna field)
        {
            if (currentLogMigracion == null)
            {
                return false;
            }
            LogMigracionDetalle logMigracionDetalle = new LogMigracionDetalle(currentLogMigracion);
            logMigracionDetalle.IdEsquemaVista = field.IdEsquemaVista;
            logMigracionDetalle.ColumnaEsquemaIdH = field.ColumnaEsquemaIdH;
            logMigracionDetalle.ColumnaEsquema = field.ColumnaEsquema;
            logMigracionDetalle.ColumnaVista = field.ColumnaVista;
            logMigracionDetalle.ColumnaVistaPK = field.ColumnaVistaPK;
            currentLogMigracionDetalle = _repositoryLM.CreateDetalle(logMigracionDetalle);
            return currentLogMigracionDetalle != null;
        }
    }
}