
using ExcelDataReader;
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using WebApp.Repositories;
using Newtonsoft.Json.Linq;

namespace WebApp.Service.IService
{
  public class ExcelService(IDataLakeRepository dataLakeRepository, IOrganizacionDataRepository organizacionDataRepository, IOrganizacionFullTextRepository organizacionFullTextRepository, IHomologacionRepository homologacionRepository, HomologacionEsquemaRepository homologacionEsquemaRepository) : IExcelService
    {
      private IDataLakeRepository _repositoryDL = dataLakeRepository;
      private IOrganizacionDataRepository _repositoryDLO = organizacionDataRepository;
      private IOrganizacionFullTextRepository _repositoryOFT = organizacionFullTextRepository;
      private IHomologacionRepository _repositoryH = homologacionRepository;
      private IHomologacionEsquemaRepository _repositoryHE = homologacionEsquemaRepository;
      private int[] filters = [];
      private int executionIndex = 0;
      private int idVistaIndex = 2;
      private int idOrganizacionIndex = 3;
      private string currentIdVista = "";
      private string currentIdOrganizacion = "";
      private bool hasIdVista = false;
      private bool hasIdOrganizacion = false;
      private bool deleted = false;
      private JArray currentSchema = new JArray();
      private List<Homologacion> currentFields = new List<Homologacion>();

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
                HomologacionEsquema? homologacionEsquema = _repositoryHE.FindByViewName(sheetName);
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
                idOrganizacionIndex = Array.FindIndex(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray(), c => c == "IdOrganizacion");
                if (idVistaIndex == -1) {
                  hasIdOrganizacion = false;
                } else {
                  hasIdOrganizacion = true;
                }
                

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                  // Se borra los datos antiguos de todo el esquema que se está migrando y se los vuelve a cargar
                  deleteOldRecords(homologacionEsquema.IdHomologacionEsquema, int.Parse(dataTable.Rows[i][0].ToString())); 
                  OrganizacionData organizacionData = addOrganizacionData(dataTable, i);
                  addOrganizacionFullText(dataTable, i, organizacionData.IdOrganizacionData);
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

      OrganizacionData addOrganizacionData(DataTable dataTable, int row)
      {
        OrganizacionData organizacionData = new OrganizacionData
        {
          IdOrganizacionData = 0,
          IdHomologacionEsquema = int.Parse(dataTable.Rows[row][4].ToString() ?? ""),
          DataEsquemaJson = buildOrganizacionDataJson(dataTable, row),
          IdConexion = int.Parse(dataTable.Rows[row][0].ToString())
        };
        if (hasIdVista)
        {
          currentIdVista = dataTable.Rows[row][idVistaIndex].ToString();
          organizacionData.IdVista = currentIdVista;
        }
        if (hasIdOrganizacion)
        {
          currentIdOrganizacion = dataTable.Rows[row][idOrganizacionIndex].ToString();
          organizacionData.IdOrganizacion = currentIdOrganizacion;
        }
        return _repositoryDLO.Create(organizacionData);
      }

      bool addOrganizacionFullText(DataTable dataTable, int row, int organizacionDataId)
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

          OrganizacionFullText newOrganizacionFullText = new OrganizacionFullText
          {
            IdOrganizacionFullText = 0,
            IdOrganizacionData = organizacionDataId,
            IdHomologacion = currentField.IdHomologacion,
            FullTextOrganizacion = currentValue,
          };

          if (hasIdVista)
          {
            newOrganizacionFullText.IdVista = currentIdVista;
          }
          if (hasIdOrganizacion)
          {
            newOrganizacionFullText.IdOrganizacion = currentIdOrganizacion;
          }

          result = _repositoryOFT.Create(newOrganizacionFullText) != null ? result : false;
        }
        return result;
      }
      string buildOrganizacionDataJson(DataTable dataTable, int row)
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
        if (deleted)
        {
          return true;
        }
        deleted = true;
        return _repositoryDLO.DeleteOldRecords(idHomologacionEsquema, idConexion);
      }

      bool deleteOldRecord(string idVista, string idOrganizacion, int idHomologacionEsquema, int idConexion)
      {
        return _repositoryDLO.DeleteOldRecord(idVista, idOrganizacion, idConexion, idHomologacionEsquema);
      }
  }
}