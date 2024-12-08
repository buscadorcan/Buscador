
using ExcelDataReader;
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using WebApp.Repositories;
using Newtonsoft.Json.Linq;
using MySqlX.XDevAPI.Common;
using ExcelDataReader.Log;

namespace WebApp.Service.IService
{
  public class ExcelService(
    ICanDataSetRepository canDataSetRepository,
    ICanFullTextRepository canFullTextRepository,
    IHomologacionRepository homologacionRepository,
    IHomologacionEsquemaRepository homologacionEsquemaRepository,
    IMigracionExcelRepository migracionExcelRepository,
    ILogMigracionRepository logMigracionRepository,
    IConexionRepository conexionRepository
    ) : IExcelService
    {
      private ICanDataSetRepository _repositoryDLO = canDataSetRepository;
      private ICanFullTextRepository _repositoryOFT = canFullTextRepository;
      private IHomologacionRepository _repositoryH = homologacionRepository;
      private IHomologacionEsquemaRepository _repositoryHE = homologacionEsquemaRepository;
      private IMigracionExcelRepository _repositoryME = migracionExcelRepository;
      private ILogMigracionRepository _repositoryLM = logMigracionRepository;
      private IConexionRepository _repositoryC = conexionRepository;
      private int[] filters = [];
      private int executionIndex = 0;
      private int idVistaIndex = -1;
      private int idOrganizacionIndex = -1;
      private string currentIdVista = "";
      private string currentIdEnte = "";
      private bool hasIdVista = false;
      private bool hasIdEnte = false;
      private bool deleted = false;
      private JArray currentSchema = new JArray();
      private List<Homologacion> currentFields = new List<Homologacion>();
      HomologacionEsquema? homologacionEsquema = null;
      private LogMigracion? currentLogMigracion = null;
      private LogMigracionDetalle? currentLogMigracionDetalle = null;
      private Conexion? currentConexion = null;
      private string idEnteName = " IdOrganizacion";

      public Boolean ImportarExcel(string path, MigracionExcel migracion) 
      {
        try {
          if (migracion == null) {
            migracion = new MigracionExcel();
            migracion.MigracionEstado = "PROCESSING";
            migracion.ExcelFileName = path.Split("/").Last();
            migracion = _repositoryME.Create(migracion);
          } else {
            migracion.MigracionEstado = "PROCESSING";
            // var result = true;
            _repositoryME.Update(migracion);
          }
          var result = Leer(path);
          if(result) {
            migracion.MigracionEstado = "SUCCESS";
          } else {
            migracion.MigracionEstado = "ERROR";
            migracion.MensageError = "Algo sslió mal en la migración";
          }
          _repositoryME.Update(migracion);

          return result;
        } catch (Exception e) {
          Console.WriteLine(e);
          migracion.MigracionEstado = "ERROR";
          migracion.MensageError = e.Message;
          _repositoryME.Update(migracion);
          if (currentLogMigracion != null) {
            currentLogMigracion.Final = DateTime.Now;
            currentLogMigracion.Estado = "ERROR";
            currentLogMigracion.Observacion = e.Message;
            _repositoryLM.Update(currentLogMigracion);
          }
          return false;
        }
      }

      public Boolean Leer(string fileSrc)
      {
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
              int idConexion;
              var migrationValue = DataSet.Tables[1].Rows[0][0].ToString();
              Console.WriteLine("Migration Value: " + migrationValue);
              try {
                idConexion = int.Parse(migrationValue);
              } catch (Exception e) {
                currentConexion = _repositoryC.FindBySiglas(migrationValue);
              }
              if (currentConexion == null) {
                throw new Exception("Error: Conexion no encontrada en la base de datos");
              }
              foreach (DataTable dataTable in DataSet.Tables)
              {
                LogMigracion logMigracion = new LogMigracion();
                string sheetName = dataTable.TableName;
                homologacionEsquema = _repositoryHE.FindByViewName(sheetName);
                if (homologacionEsquema == null) { continue; }

                logMigracion.OrigenVista = sheetName;
                logMigracion.OrigenFilas = dataTable.Rows.Count;
                logMigracion.OrigenSistema = currentConexion.Siglas;
                logMigracion.EsquemaFilas = dataTable.Rows.Count;
                logMigracion.EsquemaId = homologacionEsquema.IdHomologacionEsquema;
                logMigracion.EsquemaVista = homologacionEsquema.VistaNombre;
                currentLogMigracion = _repositoryLM.Create(logMigracion);

                currentSchema = JArray.Parse(homologacionEsquema.EsquemaJson);
                int[] homologacionIds = Array.Empty<int>();

                foreach (JObject item in currentSchema)
                {
                  int idHomologacion = item.Value<int>("IdHomologacion");
                  homologacionIds = homologacionIds.Append(idHomologacion).ToArray();
                }

                currentFields = _repositoryH.FindByIds(homologacionIds);

                executionIndex = DataSet.Tables.IndexOf(dataTable);
                Console.WriteLine("Execution Index: " + executionIndex + " Sheet: " + dataTable.TableName);
                if (string.IsNullOrEmpty(homologacionEsquema.IdVistaNombre))
                {
                  hasIdVista = false;
                } else {
                  idVistaIndex = Array.FindIndex(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray(), c => c == homologacionEsquema.IdVistaNombre);
                  if (idVistaIndex == -1) {
                    hasIdVista = false;
                  } else {
                    hasIdVista = true;
                  }
                }
                idOrganizacionIndex = Array.FindIndex(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray(), c => c == idEnteName);
                if (idOrganizacionIndex == -1) {
                  hasIdEnte = false;
                } else {
                  hasIdEnte = true;
                }
                

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                  // Se borra los datos antiguos de todo el esquema que se está migrando y se los vuelve a cargar
                  Console.WriteLine($"Deleting old records for {homologacionEsquema.IdHomologacionEsquema}");
                  
                  deleteOldRecords(homologacionEsquema.IdHomologacionEsquema, currentConexion.IdConexion); 
                  CanDataSet canDataSet = addCanDataSet(dataTable, i);
                  addCanFullText(dataTable, i, canDataSet.IdCanDataSet);
                  currentLogMigracion.Final = DateTime.Now;
                  currentLogMigracion.Estado = "OK";
                  _repositoryLM.Update(currentLogMigracion);
                }
              }
              return true;
            } else {
              Console.WriteLine("No tables found");
              return false;
            }
          }
        }
      }

      CanDataSet addCanDataSet(DataTable dataTable, int row)
      {
        CanDataSet canDataSet = new CanDataSet
        {
          IdCanDataSet = 0,
          IdHomologacionEsquema = homologacionEsquema.IdHomologacionEsquema,
          DataEsquemaJson = buildCanDataSetJson(dataTable, row)
        };
        if (hasIdVista)
        {
          currentIdVista = dataTable.Rows[row][idVistaIndex].ToString();
          canDataSet.IdVista = currentIdVista;
        }
        if (hasIdEnte)
        {
          currentIdEnte = dataTable.Rows[row][idOrganizacionIndex].ToString();
          canDataSet.IdEnte = currentIdEnte;
        }
        return _repositoryDLO.Create(canDataSet);
      }

      bool addCanFullText(DataTable dataTable, int row, int canDataSetId)
      {
        bool result = true;
        foreach (Homologacion currentField in currentFields)
        {
          if (string.IsNullOrEmpty(currentField.NombreHomologado))
          {
            continue;
          }

          int currentFieldIndex = Array.FindIndex(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray(), c => c == currentField.NombreHomologado);
          if (currentFieldIndex == -1)
          {
            continue;
          }

          string currentValue = dataTable.Rows[row][currentFieldIndex].ToString();
          if (string.IsNullOrEmpty(currentValue))
          {
            continue;
          }

          CanFullText newCanFullText = new CanFullText
          {
            IdCanFullText = 0,
            IdCanDataSet = canDataSetId,
            IdHomologacion = currentField.IdHomologacion,
            FullTextData = currentValue,
          };

          if (hasIdVista)
          {
            newCanFullText.IdVista = currentIdVista;
          }
          if (hasIdEnte)
          {
            newCanFullText.IdEnte = currentIdEnte;
          }

          result = _repositoryOFT.Create(newCanFullText) != null ? result : false;
        }
        return result;
      }

      string buildCanDataSetJson(DataTable dataTable, int row)
      {
        JArray data = new JArray();
        foreach (Homologacion currentField in currentFields)
        {
          addLogDetail(currentField);
          if (string.IsNullOrEmpty(currentField.NombreHomologado))
          {
            continue;
          }

          int currentFieldIndex = Array.FindIndex(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray(), c => c == currentField.NombreHomologado);
          if (currentFieldIndex == -1)
          {
            continue;
          }
          data.Add(new JObject
          {
            ["IdHomologacion"] = currentField.IdHomologacion,
            ["Data"] = dataTable.Rows[row][currentFieldIndex].ToString()
          });
        }

        return data.ToString();
      }

      bool deleteOldRecords(int idHomologacionEsquema, int idConexion)
      {
        Console.WriteLine($"Deleting old records for {idHomologacionEsquema} and {idConexion}");
        if (deleted)
        {
          Console.WriteLine("Already deleted");
          return true;
        }
        deleted = true;
        Console.WriteLine("Predelete");
        return _repositoryDLO.DeleteOldRecords(idHomologacionEsquema, idConexion);
      }
 
      bool deleteOldRecord(string idVista, string idOrganizacion, int idHomologacionEsquema, int idConexion)
      {
        return _repositoryDLO.DeleteOldRecord(idVista, idOrganizacion, idConexion, idHomologacionEsquema);
      }
  
      bool addLogDetail(Homologacion homologacion) {
        if (currentLogMigracion == null) {
          return false;
        }
        LogMigracionDetalle logMigracionDetalle = new LogMigracionDetalle(currentLogMigracion);
        logMigracionDetalle.IdHomologacion = homologacion.IdHomologacion;
        logMigracionDetalle.NombreHomologacion = homologacion.MostrarWeb;
        logMigracionDetalle.OrigenVistaColumna = homologacion.NombreHomologado;
        logMigracionDetalle.EsquemaIdHomologacion = homologacionEsquema.IdHomologacionEsquema.ToString();
        currentLogMigracionDetalle = _repositoryLM.CreateDetalle(logMigracionDetalle);
        return currentLogMigracionDetalle != null;
      }
  }
}