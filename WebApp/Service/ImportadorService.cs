
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace WebApp.Service.IService
{
  public class ImportadorService(ICanDataSetRepository dataLakeOrganizacionRepository, ICanFullTextRepository canFullTextRepository, IHomologacionRepository homologacionRepository, IHomologacionEsquemaRepository homologacionEsquemaRepository, IConexionRepository conexionRepository) : IImportador
    {
      private ICanDataSetRepository _repositoryDLO = dataLakeOrganizacionRepository;
      private ICanFullTextRepository _repositoryOFT = canFullTextRepository;
      private IHomologacionRepository _repositoryH = homologacionRepository;
      private IHomologacionEsquemaRepository _repositoryHE = homologacionEsquemaRepository;
      private IConexionRepository _repositoryC = conexionRepository;
      private string connectionString = "Server=localhost,1434;Initial Catalog=CAN_DB;User ID=sa;Password=pat_mic_DBKEY;TrustServerCertificate=True";
      // private readonly string defaultConnectionString = "Server=localhost,1434;Initial Catalog=CAN_DB;User ID=sa;Password=pat_mic_DBKEY;TrustServerCertificate=True";
      private Conexion? currentConexion = null;
      private int executionIndex = 0;
      private string[] views =  [];
      private string[] schemas =  [];
      private int[] hids = [];
      private int[] heids = [];
      private string[] vids = [];
      private int[] filters = [];
      private bool deleted = false;
      private bool saveIdVista = false;
      private bool saveIdEnte = false;
      private string idEnteName = "IdOrganizacion";
      
      public Boolean Importar(string[] vistas) 
      {
        try
        {
          // Agregar obtensión de vistas de base de datos
          List<Conexion> conexiones = _repositoryC.FindAll();
          ConectionStringBuilderService conectionStringBuilderService = new ConectionStringBuilderService();
          List<HomologacionEsquema> homologacionEsquemas = _repositoryHE.FindAllWithViews();
          HashSet<string> DBViews = homologacionEsquemas.Select(he => he.VistaNombre).Where(v => v != null).Select(v => v!).ToHashSet();
          HashSet<string> DBSchemas = homologacionEsquemas.Select(he => he.EsquemaJson).Where(v => v != null).Select(v => v!).ToHashSet();
          HashSet<int> HEIds = homologacionEsquemas.Select(he => he.IdHomologacionEsquema).Select(v => v!).ToHashSet();
          HashSet<string> ViewIds = homologacionEsquemas.Select(he => he.IdVistaNombre).Select(v => v!).ToHashSet();
          if (DBViews.Count > 0) { views = DBViews.ToArray(); }
          if (DBSchemas.Count > 0) { schemas = DBSchemas.ToArray(); }
          if (HEIds.Count > 0) { heids = HEIds.ToArray(); }
          if (ViewIds.Count > 0) { vids = ViewIds.ToArray(); }
          // Console.WriteLine("Vistas: " + string.Join(", ", DBViews));
          // Console.WriteLine("Esquemas: " + string.Join(", ", DBSchemas));
          // Console.WriteLine("Ids: " + string.Join(", ", HEIds));
          // Console.WriteLine("Vista Ids: " + string.Join(", ", ViewIds));
          
          foreach (Conexion conexion in conexiones)
          {
            
            if (!conexion.Migrar.Equals("S")) { continue; }
            currentConexion = conexion;
            filters = JArray.Parse(conexion.Filtros).ToObject<int[]>();
            connectionString = conectionStringBuilderService.BuildConnectionString(conexion);
            ImportarSistema(views, connectionString);
            // Environment.Exit(0);
            conexion.FechaConexion = DateTime.Now;
            conexion.Migrar = "N";
            _repositoryC.Update(conexion);
          }

          return true;
        } catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return false;
        }
      }

      public bool ImportarSistema(string[] vistas, string connectionString) 
      {
        if (connectionString != null){ this.connectionString = connectionString; }
        bool result = true;
        if (vistas != null && vistas.Length > 0){ views = vistas; }

        foreach (string view in views)
        {
          deleted = false;
          executionIndex = Array.IndexOf(views, view);
          result = result && Leer(view);
        }
        return result;
      }

      public bool Leer(string viewName)
      {
        string currentSchema = schemas[executionIndex];
        JArray schemaArray = JArray.Parse(currentSchema);
        int[] homologacionIds = Array.Empty<int>();

        foreach (JObject item in schemaArray)
        {
          int idHomologacion = item.Value<int>("IdHomologacion");
          homologacionIds = homologacionIds.Append(idHomologacion).ToArray();
        }

        List<Homologacion> homologaciones = _repositoryH.FindByIds(homologacionIds);

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          int[] newHomologacionIds = homologaciones.Select(h => h.IdHomologacion).ToArray();
          string[] newSelectFields = homologaciones.Select(h => h.NombreHomologado).ToArray();
          string selectQuery = buildSelectViewQuery(connection, viewName, newSelectFields, newHomologacionIds);
          if (selectQuery == "") { return false; }

          SqlCommand command = new SqlCommand(selectQuery, connection);
          SqlDataAdapter adapter = new SqlDataAdapter(command);
          DataSet dataSet = new DataSet();

          try
          {
            connection.Open();
            adapter.Fill(dataSet);
            if (dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
            {
              Console.WriteLine("No tables found");
              return false;
            }
            DataColumnCollection columns = dataSet.Tables[0].Columns;

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
              
              deleteOldRecords(heids[executionIndex]);

              CanDataSet dataLakeOrganizacion = addCanDataSet(row, columns);
              if (dataLakeOrganizacion == null) { return false; }

              addCanFullText(row, columns, dataLakeOrganizacion);
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
            return false;
          }
          finally
          {
            connection.Close();
          }

          return true;
        }
      }

      CanDataSet addCanDataSet(DataRow row, DataColumnCollection columns)
      {
        CanDataSet newCanDataSet = new CanDataSet
        {
          IdCanDataSet = 0,
          IdHomologacionEsquema = heids[executionIndex],
          IdConexion = currentConexion.IdConexion,
          DataEsquemaJson = buildDataLakeJson(row, columns),
        };
        if (saveIdVista) { newCanDataSet.IdVista = row[columns.Count - 1].ToString(); }
        if (saveIdEnte) { newCanDataSet.IdEnte = row[columns.Count - 2].ToString(); }

        return _repositoryDLO.Create(newCanDataSet);
      }

      bool addCanFullText(DataRow row, DataColumnCollection columns, CanDataSet dataLakeOrganizacion)
      {
        Boolean result = true;
        if (executionIndex == 0)
        {
          foreach(int filter in filters)
          {
            Homologacion? homologacion = _repositoryH.FindById(filter);
            if (homologacion == null) { continue; }

            _repositoryOFT.Create(new CanFullText
            {
              IdCanFullText = 0,
              IdCanDataSet = dataLakeOrganizacion.IdCanDataSet,
              IdHomologacion = filter,
              IdEnte = dataLakeOrganizacion.IdEnte,
              IdVista = dataLakeOrganizacion.IdVista,
              FullTextData = homologacion.MostrarWeb.ToLower().Trim()
            });
          }
        }

        for (int col = 0; col < columns.Count; col++)
        {
          try {
            string indexValue = row[col]?.ToString()?.ToLower()?.Trim();

            if (string.IsNullOrEmpty(indexValue)) { continue; }

            result = _repositoryOFT.Create(new CanFullText
            {
              IdCanFullText = 0,
              IdCanDataSet = dataLakeOrganizacion.IdCanDataSet,
              IdHomologacion = hids[col],
              IdEnte = dataLakeOrganizacion.IdEnte,
              IdVista = dataLakeOrganizacion.IdVista,
              FullTextData = row[col].ToString().ToLower().Trim()
            }) != null ? result : false;
          } catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
            return false;
          }
        }
        return result;
      }

      string buildDataLakeJson(DataRow row, DataColumnCollection columns)
      {
        int subtractFields = 0;
        if (saveIdVista) { subtractFields++; }
        if (saveIdEnte) { subtractFields++; }

        int fieldsCount = columns.Count - subtractFields;
        JArray canDataSetJson = [];
        for (int col = 0; col < fieldsCount; col++)
        {
          canDataSetJson.Add(new JObject
          {
            { "IdHomologacion", hids[col] },
            { "Data", row[col].ToString() }
          });
        }

        return canDataSetJson.ToString();
      }

      string buildSelectViewQuery(SqlConnection connection, string viewName, string[] selectFields, int[] homologacionIds)
      {
        if(!viewExists(connection, viewName))
        {
          Console.WriteLine($"Vista {viewName} no existe");
          return "";
        }

        List<int> newHomologacionIds = new List<int>();
        List<string> newSelectFields = new List<string>();

        foreach (string field in selectFields)
        {
          if (fieldExists(connection, viewName, field))
          {
            int homologacionId = homologacionIds[Array.IndexOf(selectFields, field)];
            newHomologacionIds.Add(homologacionId);
            newSelectFields.Add(field);
          }
          else
          {
            Console.WriteLine($"Field {field} does not exist in view {viewName}");
            continue;
          }
        }
        string newSelectFieldsStr = string.Join(", ", newSelectFields);
       
        if (fieldExists(connection, viewName, idEnteName)) {
            newSelectFieldsStr += ", IdEnte";
            saveIdEnte = true;
        }
        else {
            Console.WriteLine($"Field {idEnteName} does not exist in view {viewName)}");
            saveIdEnte = false;
        }
        if (fieldExists(connection, viewName, vids[executionIndex])) {
          newSelectFieldsStr += ", " + vids[executionIndex];
          saveIdVista = true;
        }
        else
        {
          Console.WriteLine("Field " + vids[executionIndex] + " does not exist in view " + viewName);
          saveIdVista = false;
        }
        hids = newHomologacionIds.ToArray();

        return $"SELECT {newSelectFieldsStr} FROM {viewName}";
      }

      bool viewExists(SqlConnection connection, string viewName)
      {
        // This shall be validated through the conexion data base, this only works for SQL SERVER, it shall work for all the supported data bases
        string query = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE IN ('BASE TABLE', 'VIEW') AND TABLE_NAME = '{viewName}'";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataAdapter adapter = new SqlDataAdapter(command);
        DataSet dataSet = new DataSet();
        adapter.Fill(dataSet);
        return dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0;
      }

      bool fieldExists(SqlConnection connection, string viewName, string fieldName)
      {
        // This shall be validated through the conexion data base, tihi only works for SQL SERVER, it shall work for all the supported data bases
        string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{viewName}' AND COLUMN_NAME = '{fieldName}'";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataAdapter adapter = new SqlDataAdapter(command);
        DataSet dataSet = new DataSet();
        adapter.Fill(dataSet);
        return dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0;
      }

      bool deleteOldRecords(int IdHomologacionEsquema)
      {
        if (deleted) { return true; }
        deleted = true;
        return _repositoryDLO.DeleteOldRecords(IdHomologacionEsquema, currentConexion.IdConexion);
      }
  }
}