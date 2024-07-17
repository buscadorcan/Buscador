
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace WebApp.Service.IService
{
  public class ImportadorService(IDataLakeRepository dataLakeRepository, IDataLakeOrganizacionRepository dataLakeOrganizacionRepository, IOrganizacionFullTextRepository organizacionFullTextRepository, IHomologacionRepository homologacionRepository, IHomologacionEsquemaRepository homologacionEsquemaRepository, IConexionRepository conexionRepository) : IImportadorService
    {
      private IDataLakeRepository _repositoryDL = dataLakeRepository;
      private IDataLakeOrganizacionRepository _repositoryDLO = dataLakeOrganizacionRepository;
      private IOrganizacionFullTextRepository _repositoryOFT = organizacionFullTextRepository;
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
      private bool saveIdOrganizacion = false;
      
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

          SqlCommand command = new SqlCommand(selectQuery, connection);
          SqlDataAdapter adapter = new SqlDataAdapter(command);
          DataSet dataSet = new DataSet();

          try
          {
            connection.Open();
            adapter.Fill(dataSet);
            DataLake? dataLake = null;
            if (dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
            {
              Console.WriteLine("No tables found");
              return false;
            }
            DataColumnCollection columns = dataSet.Tables[0].Columns;

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
              dataLake = getDatalake(dataLake);
              if (dataLake == null) { return false; }
              
              deleteOldRecords(heids[executionIndex]);

              DataLakeOrganizacion dataLakeOrganizacion = addDataLakeOrganizacion(row, dataLake, columns);
              if (dataLakeOrganizacion == null) { return false; }

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
          if ("ORGANIZQCION"?.ToString().Equals(dataLake?.DataTipo?.ToString()) == true &&
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
        if (saveIdOrganizacion) { newDataLakeOrganizacion.IdOrganizacion = row[columns.Count - 2].ToString(); }

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
       
        if (fieldExists(connection, viewName, "IdOrganizacion")) {
            newSelectFieldsStr += ", IdOrganizacion";
            saveIdOrganizacion = true;
        }
        else {
            Console.WriteLine("Field IdOrganizacion does not exist in view " + viewName);
            saveIdOrganizacion = false;
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
        return _repositoryDLO.DeleteOldRecords(IdHomologacionEsquema);
      }
  }
}