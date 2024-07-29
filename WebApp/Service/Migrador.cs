
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace WebApp.Service.IService
{
  public class Migrador(IDataLakeRepository dataLakeRepository, IDataLakeOrganizacionRepository dataLakeOrganizacionRepository, IOrganizacionFullTextRepository organizacionFullTextRepository, IHomologacionRepository homologacionRepository, IHomologacionEsquemaRepository homologacionEsquemaRepository, IConexionRepository conexionRepository) : IMigrador
    {
      private IDataLakeRepository _repositoryDL = dataLakeRepository;
      private IDataLakeOrganizacionRepository _repositoryDLO = dataLakeOrganizacionRepository;
      private IOrganizacionFullTextRepository _repositoryOFT = organizacionFullTextRepository;
      private IHomologacionRepository _repositoryH = homologacionRepository;
      private IHomologacionEsquemaRepository _repositoryHE = homologacionEsquemaRepository;
      private IConexionRepository _repositoryC = conexionRepository;
      private string connectionString = "Server=localhost,1434;Initial Catalog=CAN_DB;User ID=sa;Password=pat_mic_DBKEY;TrustServerCertificate=True";
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
      private bool saveIdOrganizacion = false;
      
      public Boolean Migrar(Conexion conexion) 
      {
        if (conexion == null) { return false; }

        try
        {
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
          
          if (!conexion.Migrar.Equals("S")) { return true; }
          currentConexion = conexion;
          filters = JArray.Parse(conexion.Filtros).ToObject<int[]>();
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
          if (selectQuery == "") { return false; }

          SqlCommand command = new SqlCommand(selectQuery, connection);
          SqlDataAdapter adapter = new SqlDataAdapter(command);
          DataSet dataSet = new DataSet();

          try
          {
            connection.Open();
            adapter.Fill(dataSet);
            DataLake? dataLake = null;
            List<int> dataLakeIds = [];
            if (dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
            {
              Console.WriteLine("No tables found");
              return false;
            }
            DataColumnCollection columns = dataSet.Tables[0].Columns;
            List<string> vistaIds = getExistingIdsFromVista(connection, viewName, vids[executionIndex]);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
              dataLake = getDatalake(dataLake);
              if (dataLake == null) { return false; }

              DataLakeOrganizacion dataLakeOrganizacion = addDataLakeOrganizacion(row, dataLake, columns);
              if (dataLakeOrganizacion == null) { return false; }

              // Se obtienen los registros de la organización que ya existian en el DataLake
              dataLakeIds = _repositoryDL.FindByDataSistemaOrigenId(dataLake.DataSistemaOrigenId, dataLake.IdDataLake);

              // Si se encuentar al menos un registro se procede a borrar los registros anteriores
              if (dataLakeIds.Count > 0)
              {
                // Se borra las versiones anteriores de los registros migrados
                deleteOldRecord(dataLakeOrganizacion.IdVista, dataLakeOrganizacion.IdOrganizacion, dataLakeIds);
           
                if (vistaIds.Count > 0) {
                  // Se borra los registros que ya no existan en las vistas exepto los que se acaban de insertar
                  _repositoryDLO.DeleteByExcludingVistaIds(vistaIds, dataLakeOrganizacion.IdOrganizacion, dataLakeIds, dataLakeOrganizacion.IdDataLakeOrganizacion);
                }
              }

              addOrganizacionFullText(row, columns, dataLakeOrganizacion);
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

      DataLake? getDatalake(DataLake? dataLake)
      {
        Homologacion? homologacionOrg = _repositoryH.FindById(14);
        if (dataLake == null) {
          return buildDatalake(homologacionOrg);
        } else
        {
          if ("ORGANIZACION"?.ToString().Equals(dataLake?.DataTipo?.ToString()) == true &&
              homologacionOrg?.MostrarWeb?.Equals(dataLake?.DataSistemaOrigen?.ToString()) == true &&
              homologacionOrg?.IdHomologacion.ToString().Equals(dataLake?.DataSistemaOrigenId?.ToString()) == true)
          {
            if (currentConexion?.FechaConexion > dataLake.DataSistemaFecha)
            {
              dataLake.DataSistemaFecha = currentConexion?.FechaConexion ?? DateTime.Now;;
              dataLake.Estado = "A";
              dataLake.DataFechaCarga = DateTime.Now;
              return _repositoryDL.Update(dataLake);
            }
            else if (currentConexion?.FechaConexion == dataLake.DataSistemaFecha)
            {
              return null;
            }
            return dataLake;
          } else 
          {
            return buildDatalake(homologacionOrg);
          }
        }
      }

      DataLake buildDatalake(Homologacion? homologacionOrg)
      {
        DataLake tmpDataLake = new DataLake
        {
          DataTipo = "ORGANIZACION",
          DataSistemaOrigen = homologacionOrg?.MostrarWeb,
          DataSistemaOrigenId = homologacionOrg?.IdHomologacion.ToString()
        };

        var existingDataLake = _repositoryDL.FindBy(tmpDataLake);
        if (existingDataLake != null)
        {
          existingDataLake.DataSistemaFecha = currentConexion?.FechaConexion ?? DateTime.Now;
          _repositoryDL.Update(existingDataLake);
          return existingDataLake;
        }
        else
        {
          tmpDataLake.Estado = "A";
          tmpDataLake.DataSistemaFecha = currentConexion?.FechaConexion ?? DateTime.Now;
          tmpDataLake.DataFechaCarga = DateTime.Now;
          return _repositoryDL.Create(tmpDataLake);
        }
      }

      DataLakeOrganizacion addDataLakeOrganizacion(DataRow row, DataLake dataLake, DataColumnCollection columns)
      {
        DataLakeOrganizacion newDataLakeOrganizacion = new DataLakeOrganizacion
        {
          IdDataLakeOrganizacion = 0,
          IdDataLake = dataLake.IdDataLake,
          IdHomologacionEsquema = heids[executionIndex],
          DataEsquemaJson = buildDataLakeJson(row, columns),
          Estado = "A"
        };
        if (saveIdVista) { newDataLakeOrganizacion.IdVista = row[columns.Count - 1].ToString(); }
        if (saveIdOrganizacion) {
          if(saveIdVista) {
            newDataLakeOrganizacion.IdOrganizacion = row[columns.Count - 2].ToString(); 
          } else {
            newDataLakeOrganizacion.IdOrganizacion = row[columns.Count - 1].ToString();
          }
        }

        return _repositoryDLO.Create(newDataLakeOrganizacion);
      }

      bool addOrganizacionFullText(DataRow row, DataColumnCollection columns, DataLakeOrganizacion dataLakeOrganizacion)
      {
        Boolean result = true;
        if (executionIndex == 0)
        {
          foreach(int filter in filters)
          {
            Homologacion? homologacion = _repositoryH.FindById(filter);
            if (homologacion == null) { continue; }

            _repositoryOFT.Create(new OrganizacionFullText
            {
              IdOrganizacionFullText = 0,
              IdDataLakeOrganizacion = dataLakeOrganizacion.IdDataLakeOrganizacion,
              IdHomologacion = filter,
              IdOrganizacion = dataLakeOrganizacion.IdOrganizacion,
              IdVista = dataLakeOrganizacion.IdVista,
              FullTextOrganizacion = homologacion.MostrarWeb.ToLower().Trim()
            });
          }
        }

        for (int col = 0; col < columns.Count; col++)
        {
          try {
            result = _repositoryOFT.Create(new OrganizacionFullText
            {
              IdOrganizacionFullText = 0,
              IdDataLakeOrganizacion = dataLakeOrganizacion.IdDataLakeOrganizacion,
              IdHomologacion = hids[col],
              IdOrganizacion = dataLakeOrganizacion.IdOrganizacion,
              IdVista = dataLakeOrganizacion.IdVista,
              FullTextOrganizacion = row[col].ToString().ToLower().Trim()
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
        if (saveIdOrganizacion) { subtractFields++; }

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
          Console.WriteLine($"View {viewName} does not exist");
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
        if (fieldExists(connection, viewName, "IdOrganizacion")) {
          newSelectFieldsStr += ", IdOrganizacion";
          saveIdOrganizacion = true;
        } else {
          Console.WriteLine("Field IdOrganizacion does not exist in view " + viewName);
          saveIdOrganizacion = false;
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
          string onlyNewWhere = currentConexion?.FechaConexion != null ? $" WHERE OrgFechaActualizacion >= '{currentConexion?.FechaConexion}'" : "";
          selectString = $"{selectString} {onlyNewWhere}";
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
        string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE IN ('BASE TABLE', 'VIEW') AND TABLE_NAME = '{viewName}'";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataAdapter adapter = new SqlDataAdapter(command);
        DataSet dataSet = new DataSet();
        adapter.Fill(dataSet);
        return dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0;
      }

      bool deleteOldRecord(string idVista, string idOrganizacion, List<int> dataLakeIds)
      {
        return _repositoryDLO.DeleteOldRecord(idVista, idOrganizacion, dataLakeIds);
      }
  
      List<string> getExistingIdsFromVista(SqlConnection connection, string viewName, string idVista) {
        string query = $"SELECT '{idVista}' FROM '{viewName}'";
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