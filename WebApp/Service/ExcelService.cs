
using ExcelDataReader;
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using WebApp.Repositories;
using Newtonsoft.Json.Linq;

namespace WebApp.Service.IService
{
  public class ExcelService(ICanDataSetRepository canDataSetRepository, ICanFullTextRepository canFullTextRepository, IHomologacionRepository homologacionRepository, IHomologacionEsquemaRepository homologacionEsquemaRepository) : IExcelService
    {
      private ICanDataSetRepository _repositoryDLO = canDataSetRepository;
      private ICanFullTextRepository _repositoryOFT = canFullTextRepository;
      private IHomologacionRepository _repositoryH = homologacionRepository;
      private IHomologacionEsquemaRepository _repositoryHE = homologacionEsquemaRepository;
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
      private string idEnteName = " IdOrganizacion";

      public Boolean ImportarExcel(string path) 
      {
        return Leer(path);;
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
              foreach (DataTable dataTable in DataSet.Tables)
              {
                string sheetName = dataTable.TableName;
                homologacionEsquema = _repositoryHE.FindByViewName(sheetName);
                if (homologacionEsquema == null) { continue; }

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
                  Console.WriteLine($"Deleting old records for {homologacionEsquema.IdHomologacionEsquema} and {int.Parse(dataTable.Rows[i][0].ToString())}");
                  deleteOldRecords(homologacionEsquema.IdHomologacionEsquema, int.Parse(dataTable.Rows[i][0].ToString())); 
                  CanDataSet canDataSet = addCanDataSet(dataTable, i);
                  addCanFullText(dataTable, i, canDataSet.IdCanDataSet);
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
          DataEsquemaJson = buildCanDataSetJson(dataTable, row),
          IdConexion = int.Parse(dataTable.Rows[row][0].ToString())
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
  }
}