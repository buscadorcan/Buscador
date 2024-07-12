
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
      private readonly string defaultConnectionString = "Server=localhost,1434;Initial Catalog=CAN_DB;User ID=sa;Password=pat_mic_DBKEY;TrustServerCertificate=True";
      private Conexion? currentConexion = null;
      private int executionIndex = 0;
      private string[] views =  [];
      private string[] schemas =  [];
      private int[] hids = [];
      private int[] heids = [];
      private int[] filters = [];
      private bool deleted = false;
      
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
          if (DBViews.Count > 0) { views = DBViews.ToArray(); }
          if (DBSchemas.Count > 0) { schemas = DBSchemas.ToArray(); }
          if (HEIds.Count > 0) { heids = HEIds.ToArray(); }
          Console.WriteLine("Vistas: " + string.Join(", ", DBViews));
          Console.WriteLine("Esquemas: " + string.Join(", ", DBSchemas));
          Console.WriteLine("Ids: " + string.Join(", ", HEIds));
          
          foreach (Conexion conexion in conexiones)
          {
            currentConexion = conexion;
            Console.WriteLine("Conexión: " + conexion.BaseDatos);
            filters = JArray.Parse(conexion.Filtros).ToObject<int[]>();
            Console.WriteLine("Filtros: " + string.Join(", ", filters));
            connectionString = conectionStringBuilderService.BuildConnectionString(conexion);
            ImportarSistema(views, connectionString);
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
          Console.WriteLine("Execution Index: " + executionIndex + " - View: " + view);
          result = result && Leer(view);
        }
        return result;
      }

      public bool Leer(string viewName)
      {
        Console.WriteLine("ViewName: " + viewName);
        string currentSchema = schemas[executionIndex];
        Console.WriteLine("Schema: " + currentSchema);
        JArray schemaArray = JArray.Parse(currentSchema);
        // Console.WriteLine("SchemaArray: " + schemaArray);
        int[] homologacionIds = Array.Empty<int>();

        foreach (JObject item in schemaArray)
        {
          int idHomologacion = item.Value<int>("IdHomologacion");
          homologacionIds = homologacionIds.Append(idHomologacion).ToArray();
          Console.WriteLine("IdHomologacion: " + idHomologacion);
        }
        Console.WriteLine("HomologacionIds: " + string.Join(", ", homologacionIds));

        List<Homologacion> homologaciones = _repositoryH.FindByIds(homologacionIds);
        string selectFields = string.Join(", ", homologaciones.Select(h => h.NombreHomologado));
        string selectQuery = $"SELECT {selectFields} FROM " + viewName;
        Console.WriteLine("SelectFields: " + selectFields);
        Console.WriteLine("SelectQuery: " + selectQuery);
        int[] newHomologacionIds = homologaciones.Select(h => h.IdHomologacion).ToArray();
        hids = newHomologacionIds;

        foreach (Homologacion homologacion in homologaciones)
        {
          Console.WriteLine("MostrarWeb: " + homologacion.MostrarWeb);

        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
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
            Console.WriteLine("Columns: " + string.Join(", ", columns.Cast<DataColumn>().Select(c => c.ColumnName)));

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
              dataLake = getDatalake(dataLake);
              if (dataLake == null) { return false; }
              
              Console.WriteLine("DataLake: " + dataLake.IdDataLake);
              Console.WriteLine("Esquema ID: " + heids[executionIndex]);
              deleteOldRecords(heids[executionIndex]);

              DataLakeOrganizacion dataLakeOrganizacion = addDataLakeOrganizacion(row, dataLake, columns);
              if (dataLakeOrganizacion == null) { return false; }

              addOrganizacionFullText(row, columns, dataLakeOrganizacion.IdDataLakeOrganizacion);
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
        return _repositoryDLO.Create(new DataLakeOrganizacion
          {
            IdDataLakeOrganizacion = 0,
            IdDataLake = dataLake.IdDataLake,
            IdHomologacionEsquema = heids[executionIndex],
            DataEsquemaJson = buildDataLakeJson(row, columns),
            Estado = "A"
          });
      }

      bool addOrganizacionFullText(DataRow row, DataColumnCollection columns, int dataLakeOrganizacionId)
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
              IdDataLakeOrganizacion = dataLakeOrganizacionId,
              IdHomologacion = filter,
              FullTextOrganizacion = homologacion.NombreHomologado + " " + homologacion.MostrarWeb
            });
          }
        }

        for (int col = 0; col < columns.Count; col++)
        {
          try {
            result = _repositoryOFT.Create(new OrganizacionFullText
            {
              IdOrganizacionFullText = 0,
              IdDataLakeOrganizacion = dataLakeOrganizacionId,
              IdHomologacion = hids[col],
              FullTextOrganizacion = columns[col].ColumnName + " " + row[col].ToString()
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
        JArray dataLakeJson = [];
        for (int col = 0; col < columns.Count; col++)
        {
          dataLakeJson.Add(new JObject
          {
            { "IdHomologacion", hids[col] },
            { "Data", columns[col].ColumnName + " " + row[col].ToString() }
          });
        }

        Console.WriteLine("DataLakeJson: " + dataLakeJson.ToString());
        return dataLakeJson.ToString();
      }
      
      bool deleteOldRecords(int IdHomologacionEsquema)
      {
        if (deleted) { return true; }
        deleted = true;
        return _repositoryDLO.DeleteOldRecords(IdHomologacionEsquema);
      }
  }
}