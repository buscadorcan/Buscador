
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace WebApp.Service.IService
{
  public class Migrador(IEsquemaDataRepository esquemaDataRepository, IEsquemaFullTextRepository esquemaFullTextRepository, IHomologacionRepository homologacionRepository, IEsquemaRepository esquemaRepository, IONAConexionRepository conexionRepository) : IMigrador
    {
      private IEsquemaDataRepository _repositoryDLO = esquemaDataRepository;
      private IEsquemaFullTextRepository _repositoryOFT = esquemaFullTextRepository;
      private IHomologacionRepository _repositoryH = homologacionRepository;
      private IEsquemaRepository _repositoryHE = esquemaRepository;
      private IONAConexionRepository _repositoryC = conexionRepository;
      private string connectionString = "Server=localhost,1434;Initial Catalog=CAN_DB;User ID=sa;Password=pat_mic_DBKEY;TrustServerCertificate=True";
      private ONAConexion? currentConexion = null;
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
      
      public Boolean Migrar(ONAConexion conexion) 
      {
        if (conexion == null) { return false; }

        try
        {
          List<Esquema> homologacionEsquemas = _repositoryHE.FindAllWithViews();
          HashSet<string> DBViews = homologacionEsquemas.Select(he => he.EsquemaVista).Where(v => v != null).Select(v => v!).ToHashSet();
          HashSet<string> DBSchemas = homologacionEsquemas.Select(he => he.EsquemaJson).Where(v => v != null).Select(v => v!).ToHashSet();
          // HashSet<int> HEIds = homologacionEsquemas.Select(he => he.IdHomologacionEsquema).Select(v => v!).ToHashSet();
          // HashSet<string> ViewIds = homologacionEsquemas.Select(he => he.IdVistaNombre).Select(v => v!).ToHashSet();
          if (DBViews.Count > 0) { views = DBViews.ToArray(); }
          if (DBSchemas.Count > 0) { schemas = DBSchemas.ToArray(); }
          // if (HEIds.Count > 0) { heids = HEIds.ToArray(); }
          // if (ViewIds.Count > 0) { vids = ViewIds.ToArray(); }
          // Console.WriteLine("Vistas: " + string.Join(", ", DBViews));
          // Console.WriteLine("Esquemas: " + string.Join(", ", DBSchemas));
          // Console.WriteLine("Ids: " + string.Join(", ", HEIds));
          // Console.WriteLine("Vista Ids: " + string.Join(", ", ViewIds));
          
          if (!conexion.Migrar.Equals("S")) { return true; }
          currentConexion = conexion;
          // filters = JArray.Parse(conexion.Filtros).ToObject<int[]>();
          ConectionStringBuilderService conectionStringBuilderService = new ConectionStringBuilderService();
          connectionString = conectionStringBuilderService.BuildConnectionString(conexion);
          ImportarSistema(views, connectionString);

          return true;
        } catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return false;
        }
      }

      public bool ImportarSistema(string[] vistas, string connectionString) 
      {
        if (connectionString == null)
        {
          return false;
        }
        else
        { 
          this.connectionString = connectionString;
        }
        
        bool result = true;
        if (vistas != null && vistas.Length > 0){ views = vistas; }

        foreach (string view in views)
        {
          deleted = false;
          executionIndex = Array.IndexOf(views, view);
          result = result && Procesar(view);
        }
        return result;
      }

      public bool Procesar(string viewName)
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
          Console.WriteLine("Select Query: " + selectQuery);
          if (selectQuery == "") { return false; }

          SqlCommand command = new SqlCommand(selectQuery, connection);
          SqlDataAdapter adapter = new SqlDataAdapter(command);
          DataSet dataSet = new DataSet();

          try
          {
            connection.Open();
            adapter.Fill(dataSet);
            List<int> dataLakeIds = [];
            if (dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
            {
              Console.WriteLine("No hay nuevos registros en la vista " + viewName);
              return false;
            }
            DataColumnCollection columns = dataSet.Tables[0].Columns;
            List<string> vistaIds = getExistingIdsFromVista(connection, viewName, vids[executionIndex]);
            Console.WriteLine("VistaIds: " + string.Join(", ", vistaIds));

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
              // CanDataSet canDataSet = addCanDataSet(row, columns);
              // if (canDataSet == null) { return false; }

              // Se borra las versiones anteriores de los registros migrados
              // deleteOldRecord(canDataSet.IdVista, canDataSet.IdEnte);
          
              if (vistaIds.Count > 0) {
                // Se borra los registros que ya no existan en las vistas exepto los que se acaban de insertar
                // _repositoryDLO.DeleteByExcludingVistaIds(vistaIds, canDataSet.IdEnte, currentConexion.IdConexion, canDataSet.IdCanDataSet);
              }

              // addCanFullText(row, columns, canDataSet);
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

      // CanDataSet addCanDataSet(DataRow row, DataColumnCollection columns)
      // {
      //   CanDataSet newCanDataSet = new CanDataSet
      //   {
      //     IdCanDataSet = 0,
      //     IdConexion = currentConexion?.IdConexion ?? 0,
      //     IdHomologacionEsquema = heids[executionIndex],
      //     DataEsquemaJson = buildCanDataSetJson(row, columns)
      //   };
      //   if (saveIdVista) { newCanDataSet.IdVista = row[columns.Count - 1].ToString(); }
      //   if (saveIdEnte) {
      //     if(saveIdVista) {
      //       newCanDataSet.IdEnte = row[columns.Count - 2].ToString(); 
      //     } else {
      //       newCanDataSet.IdEnte = row[columns.Count - 1].ToString();
      //     }
      //   }

      //   return _repositoryDLO.Create(newCanDataSet);
      // }

      bool addCanFullText(DataRow row, DataColumnCollection columns, EsquemaData canDataSet)
      {
        Boolean result = true;
        if (executionIndex == 0)
        {
          foreach(int filter in filters)
          {
            Homologacion? homologacion = _repositoryH.FindById(filter);
            if (homologacion == null) { continue; }

            // _repositoryOFT.Create(new CanFullText
            // {
            //   IdCanFullText = 0,
            //   IdCanDataSet = canDataSet.IdCanDataSet,
            //   IdHomologacion = filter,
            //   IdEnte = canDataSet.IdEnte,
            //   IdVista = canDataSet.IdVista,
            //   FullTextData = homologacion.MostrarWeb.ToLower().Trim()
            // });
          }
        }

        for (int col = 0; col < columns.Count; col++)
        {
          try {
            // result = _repositoryOFT.Create(new CanFullText
            // {
            //   IdCanFullText = 0,
            //   IdCanDataSet = canDataSet.IdCanDataSet,
            //   IdHomologacion = hids[col],
            //   IdEnte = canDataSet.IdEnte,
            //   IdVista = canDataSet.IdVista,
            //   FullTextData = row[col].ToString().ToLower().Trim()
            // }) != null ? result : false;
          } catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
            return false;
          }
        }
        return result;
      }

      string buildCanDataSetJson(DataRow row, DataColumnCollection columns)
      {
        int subtractFields = 0;
        if (saveIdVista) { subtractFields++; }
        if (saveIdEnte) { subtractFields++; }

        int fieldsCount = columns.Count - subtractFields;
        JArray dataLakeJson = [];
        for (int col = 0; col < fieldsCount; col++)
        {
          dataLakeJson.Add(new JObject
          {
            { "IdHomologacion", hids[col] },
            { "Data", row[col].ToString() }
          });
        }

        // Console.WriteLine("DataLakeJson: " + dataLakeJson.ToString());
        return dataLakeJson.ToString();
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
       
        // Agregamos el Id de la organización en caso de existir
        if (fieldExists(connection, viewName, "IdEnte")) {
          newSelectFieldsStr += ", IdEnte";
          saveIdEnte = true;
        } else {
          Console.WriteLine("Field IdEnte does not exist in view " + viewName);
          saveIdEnte = false;
        }
        // Agregamos el Id de la vista en caso de existir
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
        string selectString = $"SELECT {newSelectFieldsStr} FROM {viewName}";
        // Seleccionamos solo los nuevos registros en caso de existir la fecha deconexión
        if (fieldExists(connection, viewName, "OrgFechaActualizacion")) {
            // string onlyNewWhere = currentConexion?.FechaConexion != null ? $" WHERE CAST(OrgFechaActualizacion AS DATE) >= CAST('{currentConexion?.FechaConexion?.ToString("yyyy-MM-dd")}' AS DATE)" : "";
          // selectString = $"{selectString} {onlyNewWhere}";
        } 

        return selectString;
      }

      bool fieldExists(SqlConnection connection, string viewName, string fieldName)
      {
        // This shall be validated through the conexion data base, this only works for SQL SERVER, it shall work for all the supported data bases
        string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{viewName}' AND COLUMN_NAME = '{fieldName}'";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataAdapter adapter = new SqlDataAdapter(command);
        DataSet dataSet = new DataSet();
        adapter.Fill(dataSet);
        return dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0;
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

      bool deleteOldRecord(string idVista, string idOrganizacion)
      {
        // return _repositoryDLO.DeleteOldRecord(idVista, idOrganizacion, currentConexion?.IdConexion ?? 0, heids[executionIndex]);
        return false;
      }
  
      List<string> getExistingIdsFromVista(SqlConnection connection, string viewName, string idVista) {
        string query = $"SELECT {idVista} FROM {viewName}";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataAdapter adapter = new SqlDataAdapter(command);
        DataSet dataSet = new DataSet();
        adapter.Fill(dataSet);
        List<string> ids = new List<string>();
        foreach (DataRow row in dataSet.Tables[0].Rows)
        {
          ids.Add(row[0].ToString());
        }
        return ids;
      }
  }
}