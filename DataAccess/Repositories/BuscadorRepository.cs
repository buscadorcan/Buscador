using System.Data;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedApp.Dtos;

namespace DataAccess.Repositories
{
    public class BuscadorRepository : BaseRepository, IBuscadorRepository
    {
        public BuscadorRepository(
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
        }
        public BuscadorDto PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage)
        {
            return ExecuteDbOperation(context =>
            {
                var rowsTotal = new SqlParameter
                {
                    ParameterName = "@RowsTotal",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                var panelONAjson = new SqlParameter
                {
                    ParameterName = "@vwPanelONAjson",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = -1, // -1 para valores grandes tipo NVARCHAR(MAX)
                    Direction = ParameterDirection.Output
                };


                var lstTem = context.Database.SqlQueryRaw<BuscadorResultadoData>(
                   "exec paBuscar2K25 @paramJSON, @PageNumber, @RowsPerPage, @RowsTotal OUT, @vwPanelONAjson OUT",
                   new SqlParameter("@paramJSON", paramJSON),
                   new SqlParameter("@PageNumber", PageNumber),
                   new SqlParameter("@RowsPerPage", RowsPerPage),
                   rowsTotal,
                   panelONAjson
                 ).AsNoTracking().ToList();

                var panelONAData = string.IsNullOrEmpty(panelONAjson.Value as string)
                        ? new List<vwPanelONA>()
                        : JsonConvert.DeserializeObject<List<vwPanelONA>>(panelONAjson.Value.ToString());

                var panelONADataDto = panelONAData.Select(o => new vwPanelONADto
                {
                    Sigla  = o.Sigla,
                    Pais = o.Pais,
                    Icono = o.Icono,
                    NroOrg = o.NroOrg
                }).ToList();


                return new BuscadorDto
                {
                    Data = lstTem.Select(c => new BuscadorResultadoDataDto()
                    {
                        IdONA = c.IdONA,
                        Siglas = c.Siglas,
                        Texto = c.Texto,
                        VistaPK = c.VistaPK,
                        VistaFK = c.VistaFK,
                        IdEsquema = c.IdEsquema,
                        IdEsquemaVista = c.IdEsquemaVista,
                        IdEsquemaData = c.IdEsquemaData,
                        DataEsquemaJson = JsonConvert.DeserializeObject<List<ColumnaEsquema>>(c.DataEsquemaJson ?? "[]")
                    }).ToList(),
                    TotalCount = (int)rowsTotal.Value,
                    PanelONA = panelONADataDto
                };
            });
        }
        public List<EsquemaDto> FnHomologacionEsquemaTodo(string VistaFK, int idOna)
        {
            return ExecuteDbOperation(context =>
            {
                return context.Database.SqlQuery<EsquemaDto>($"select * from fnEsquemaTodo({VistaFK},{idOna})").AsNoTracking().OrderBy(c => c.MostrarWebOrden).ToList();
            });
        }

        public FnEsquemaDto? FnHomologacionEsquema(int idEsquema)
        {
            return ExecuteDbOperation(context =>
            {
                return context.Database.SqlQuery<FnEsquemaDto>($"select * from fnEsquema({idEsquema})").AsNoTracking().FirstOrDefault();
            });
        }

        public fnEsquemaCabeceraDto? FnEsquemaCabecera(int IdEsquemadata)
        {
            return ExecuteDbOperation(context =>
            {
                return context.Database.SqlQuery<fnEsquemaCabeceraDto>($"select * from fnEsquemaCabecera({IdEsquemadata})").AsNoTracking().FirstOrDefault();
            });
        }
        public List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idEsquema, string VistaFK, int idOna)
        {
            return ExecuteDbOperation(context =>
            {
                var lstTem = context.Database.SqlQuery<FnHomologacionEsquemaData>($"select * from fnEsquemaDato({idEsquema},{VistaFK}, {idOna})")
                                              .AsNoTracking()
                                              .ToList();

                return lstTem.Select(c =>
                {
                    List<ColumnaEsquema> dataEsquema = new List<ColumnaEsquema>();

                    try
                    {
                        dataEsquema = JsonConvert.DeserializeObject<List<ColumnaEsquema>>(c.DataEsquemaJson ?? "[]");
                    }
                    catch (JsonReaderException ex)
                    {
                        Console.WriteLine($"Error al deserializar JSON: {ex.Message}");
                        Console.WriteLine($"JSON inválido: {c.DataEsquemaJson}");
                    }

                    return new FnHomologacionEsquemaDataDto()
                    {
                        IdEsquemaData = c.IdEsquemaData,
                        IdEsquema = c.IdEsquema,
                        DataEsquemaJson = dataEsquema
                    };
                }).ToList();
            });
        }

        public List<FnEsquemaDataBuscadoDto> FnEsquemaDatoBuscar(int IdEsquemadata, string TextoBuscar)
        {
            return ExecuteDbOperation(context =>
            {
                try
                {
                    Console.WriteLine($"Ejecutando SQL con parámetros: IdEsquemadata={IdEsquemadata}, TextoBuscar={TextoBuscar}");

                    // Definir los parámetros de la consulta
                    var paramIdEsquemadata = new SqlParameter("@IdEsquemadata", SqlDbType.Int) { Value = IdEsquemadata };
                    var paramTextoBuscar = new SqlParameter("@TextoBuscar", SqlDbType.NVarChar, 400) { Value = TextoBuscar ?? (object)DBNull.Value };

                    // Ejecutar la consulta con parámetros
                    var lstTem = context.Database.SqlQueryRaw<FnEsquemaDataBuscado>(
                        "SELECT * FROM fnEsquemaDatoBuscado(@IdEsquemadata, @TextoBuscar)",
                        paramIdEsquemadata, paramTextoBuscar
                    ).AsNoTracking().ToList();

                    Console.WriteLine($"Registros obtenidos de SQL: {lstTem.Count}");

                    // Transformar los datos en DTOs
                    var resultado = lstTem.Select(c =>
                    {
                        Console.WriteLine($"Procesando JSON para IdEsquemaData={c.IdEsquemaData}");

                        List<ColumnaEsquema> jsonData;
                        try
                        {
                            jsonData = JsonConvert.DeserializeObject<List<ColumnaEsquema>>(c.DataEsquemaJson ?? "[]");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al deserializar JSON: {ex.Message}");
                            jsonData = new List<ColumnaEsquema>();
                        }

                        return new FnEsquemaDataBuscadoDto()
                        {
                            IdEsquemaData = c.IdEsquemaData,
                            IdEsquema = c.IdEsquema,
                            DataEsquemaJson = jsonData
                        };
                    }).ToList();

                    Console.WriteLine($"Registros transformados a DTOs: {resultado.Count}");
                    return resultado;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en ExecuteDbOperation: {ex.Message}");
                    return new List<FnEsquemaDataBuscadoDto>(); // Retorna lista vacía en caso de error
                }
            });
        }


        public List<FnPredictWordsDto> FnPredictWords(string word)
        {
            return ExecuteDbOperation(context =>
            {
                return context.Database.SqlQuery<FnPredictWordsDto>($"select * from fnPredictWord({word})").AsNoTracking().OrderBy(c => c.Word).ToList();
            });
        }

        public bool ValidateWords(List<string> words)
        {
            if (words == null || words.Count == 0)
                return false;

            bool valor =  ExecuteDbOperation(context =>
            {
                return context.EsquemaFullText.Any(e => words.Contains(e.FullTextData));
            });

            return valor;
            
        }

        public int AddEventTracking(EventTrackingDto eventTracking)
        {
            _ = ExecuteDbOperation<int>(context =>
            {
                var codigoHomologacionRol = new SqlParameter("@CodigoHomologacionRol", SqlDbType.NVarChar, 25) { Value = eventTracking.CodigoHomologacionRol };
                var idUsuario = new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = eventTracking.idUsuario };
                var codigoHomologacionMenu = new SqlParameter("@CodigoHomologacionMenu", SqlDbType.NVarChar, 100) { Value = eventTracking.CodigoHomologacionMenu };
                var nombreControl = new SqlParameter("@NombreControl", SqlDbType.NVarChar, 100) { Value = eventTracking.NombreControl };
                var nombreAccion = new SqlParameter("@NombreAccion", SqlDbType.NVarChar, 100) { Value = eventTracking.NombreAccion };
                var ubicacionJson = new SqlParameter("@UbicacionJson", SqlDbType.NVarChar, -1) { Value = eventTracking.UbicacionJson };
                var parametroJson = new SqlParameter("@ParametroJson", SqlDbType.NVarChar, -1)
                {
                    Value = string.IsNullOrEmpty(eventTracking.ParametroJson) ? (object)DBNull.Value : eventTracking.ParametroJson
                };

                return context.Database.ExecuteSqlRaw(
                    "EXEC paAddEventTracking @IdUsuario, @CodigoHomologacionRol, @CodigoHomologacionMenu, @NombreControl, @NombreAccion, @UbicacionJson, @ParametroJson",
                    new[] { idUsuario, codigoHomologacionRol, codigoHomologacionMenu, nombreControl, nombreAccion, ubicacionJson, parametroJson }
                );
            });

            return 0;

        }
    }
}
