using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
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

                var lstTem = context.Database.SqlQueryRaw<BuscadorResultadoData>(
                  "exec paBuscar2K25 @paramJSON, @PageNumber, @RowsPerPage, @RowsTotal OUT",
                  new SqlParameter("@paramJSON", paramJSON),
                  new SqlParameter("@PageNumber", PageNumber),
                  new SqlParameter("@RowsPerPage", RowsPerPage),
                  rowsTotal
                ).AsNoTracking().ToList();

                return new BuscadorDto
                {
                    Data = lstTem.Select(c => new BuscadorResultadoDataDto()
                    {
                        IdONA = c.IdONA,
                        Siglas = c.Siglas,
                        TextOverView = c.TextOverView,
                        VistaPK = c.VistaPK,
                        VistaFK = c.VistaFK,
                        IdEsquema = c.IdEsquema,
                        IdEsquemaVista = c.IdEsquemaVista,
                        IdEsquemaData = c.IdEsquemaData,             
                        DataEsquemaJson = JsonConvert.DeserializeObject<List<ColumnaEsquema>>(c.DataEsquemaJson ?? "[]")
                    }).ToList(),
                    TotalCount = (int)rowsTotal.Value
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
        public List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idEsquema, string VistaFK, int idOna)
        {
            return ExecuteDbOperation(context =>
            {
                var lstTem = context.Database.SqlQuery<FnHomologacionEsquemaData>($"select * from fnEsquemaDato({idEsquema},{VistaFK}, {idOna})").AsNoTracking().ToList();

                return lstTem.Select(c => new FnHomologacionEsquemaDataDto()
                {
                    IdEsquemaData = c.IdEsquemaData,
                    IdEsquema = c.IdEsquema,
                    DataEsquemaJson = JsonConvert.DeserializeObject<List<ColumnaEsquema>>(c.DataEsquemaJson ?? "[]")
                }).ToList();
            });
        }
        public List<FnPredictWordsDto> FnPredictWords(string word)
        {
            return ExecuteDbOperation(context =>
            {
                return context.Database.SqlQuery<FnPredictWordsDto>($"select * from fnPredictWord({word})").AsNoTracking().OrderBy(c => c.Word).ToList();
            });
        }
    }
}
