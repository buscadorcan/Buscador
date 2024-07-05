
using WebApp.Models;
using System.Data;
using WebApp.Repositories.IRepositories;
using Microsoft.Data.SqlClient;

namespace WebApp.Service.IService
{
  public class ImportadorService(IDataLakeRepository dataLakeRepository, IDataLakeOrganizacionRepository dataLakeOrganizacionRepository, IOrganizacionFullTextRepository organizacionFullTextRepository, IHomologacionRepository homologacionRepository) : IImportadorService
    {
      private IDataLakeRepository _repositoryDL = dataLakeRepository;
      private IDataLakeOrganizacionRepository _repositoryDLO = dataLakeOrganizacionRepository;
      private IOrganizacionFullTextRepository _repositoryOFT = organizacionFullTextRepository;
      private IHomologacionRepository _repositoryH = homologacionRepository;
      private string connectionString = "Server=localhost,1434;Initial Catalog=CAN_DB;User ID=sa;Password=pat_mic_DBKEY;TrustServerCertificate=True";
      private int? executionIndex = 0;
      private string[] views =  ["vwGrilla", "vwEsq01", "vwEsq02"];
      private int[] filters = [5, 6];
      private bool deleted = false;
      
      public Boolean Importar(string[] vistas) 
      {
        bool result = true;
        if (vistas != null && vistas.Length > 0){ views = vistas; }

        foreach (string view in views)
        {
          deleted = false;
          executionIndex = Array.IndexOf(views, view);
          Console.WriteLine("Execution Index: " + executionIndex + "View: " + view);
          result = result && Leer(view);
        }
        return result;
      }

      public bool Leer(string viewName)
      {
        
        string query = "SELECT * FROM " + viewName;
        // This shal be fixed to soft delete the old record, it´s commented for testing purposes
        // string updateQuery = $"UPDATE DataLakeOrganizacion SET Estado = 'X' where IdDataLakeOrganizacion <= {_repositoryDLO.getLastId()}";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          SqlCommand command = new SqlCommand(query, connection);
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
              dataLake = getDatalake(row, dataLake);
              if (dataLake == null) { return false; }
              
              deleteOldRecords(int.Parse(row[4].ToString() ?? ""));
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

      DataLake? getDatalake(DataRow row, DataLake? dataLake)
      {
        if (dataLake == null) {
          return buildDatalake(row);
        } else
        {
          if (row[0]?.ToString().Equals(dataLake?.DataTipo?.ToString()) == true &&
              row[1]?.ToString().Equals(dataLake?.DataSistemaOrigen?.ToString()) == true &&
              row[2]?.ToString().Equals(dataLake?.DataSistemaOrigenId?.ToString()) == true)
          {
            if (DateTime.Parse(row[3]?.ToString() ?? "01/01/1900") > dataLake.DataSistemaFecha)
            {
              dataLake.DataSistemaFecha = DateTime.Parse(row[3]?.ToString() ?? "01/01/1900");
              dataLake.Estado = "A";
              dataLake.DataFechaCarga = DateTime.Now;
              return _repositoryDL.Update(dataLake);
            }
            else if (DateTime.Parse(row[3]?.ToString() ?? "01/01/1900") == dataLake.DataSistemaFecha)
            {
              return null;
            }
            return dataLake;
          } else 
          {
            return buildDatalake(row);
          }
        }
      }

      DataLake buildDatalake(DataRow row)
      {
        DataLake tmpDataLake = new DataLake
        {
          DataTipo = row[0].ToString(),
          DataSistemaOrigen = row[1].ToString(),
          DataSistemaOrigenId = row[2].ToString()
        };

        var existingDataLake = _repositoryDL.FindBy(tmpDataLake);
        if (existingDataLake != null)
        {
          existingDataLake.DataSistemaFecha = DateTime.Parse(row[3]?.ToString() ?? "01/01/1900");
          _repositoryDL.Update(existingDataLake);
          return existingDataLake;
        }
        else
        {
          tmpDataLake.Estado = "A";
          tmpDataLake.DataSistemaFecha = DateTime.Parse(row[3]?.ToString() ?? "01/01/1900");
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
            IdHomologacionEsquema = int.Parse(row[4].ToString() ?? ""),
            DataEsquemaJson = buildDataLakeJson(row, columns),
            Estado = "A"
          });
      }
     
      bool addOrganizacionFullText(DataRow row, DataColumnCollection columns, int dataLakeOrganizacionId)
      {
        int columnsCount = columns.Count;
        if (columnsCount < 7)
        {
          return false;
        }
        Boolean result = true;
        if (executionIndex == 0)
        {
          foreach(int filter in filters)
          {
            Homologacion? homologacion = _repositoryH.FindByMostrarWeb(row[filter].ToString() ?? "");
            if (homologacion == null) { continue; }

            _repositoryOFT.Create(new OrganizacionFullText
            {
              IdOrganizacionFullText = 0,
              IdDataLakeOrganizacion = dataLakeOrganizacionId,
              IdHomologacion = homologacion.IdHomologacion,
              FullTextOrganizacion = row[filter].ToString()
            });
          }
        }

        for (int col = 7; col < columnsCount; col++)
        {
          result = _repositoryOFT.Create(new OrganizacionFullText
          {
            IdOrganizacionFullText = 0,
            IdDataLakeOrganizacion = dataLakeOrganizacionId,
            IdHomologacion = int.Parse(columns[col].ColumnName.Substring(1)),
            FullTextOrganizacion = row[col].ToString()
          }) != null ? result : false;
        }
        return result;
      }
      
      string buildDataLakeJson(DataRow row, DataColumnCollection columns)
      {
        if (columns.Count < 7)
        {
          return "[]";
        }
        string json = "[";
        for (int col = 7; col < columns.Count; col++)
        {
          json += "{ \"IdHomologacion\": \"" + columns[col].ColumnName.Substring(1) + "\", \"Data\": \"" + columns[col].ColumnName.Substring(1) + " " + row[col].ToString() + "\" },";
        }
        return json.TrimEnd(',') + "]";
      }
      bool deleteOldRecords(int IdHomologacionEsquema)
      {
        if (deleted) { return true; }
        deleted = true;
        return _repositoryDLO.DeleteOldRecords(IdHomologacionEsquema);
      }
  }
}