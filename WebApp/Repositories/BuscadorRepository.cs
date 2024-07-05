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
            IDbContextFactory dbContextFactory
        ) : base(dbContextFactory, logger)
        {
            
        }
        public BuscadorDto PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage)
        {
            return ExecuteDbOperation(context => {
                var rowsTotal = new SqlParameter
                {
                    ParameterName = "@RowsTotal",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                var lstTem = context.Database.SqlQueryRaw<FnHomologacionEsquemaData>(
                    "exec psBuscarPalabra @paramJSON, @PageNumber, @RowsPerPage, @RowsTotal OUT",
                    new SqlParameter("@paramJSON", paramJSON),
                    new SqlParameter("@PageNumber", PageNumber),
                    new SqlParameter("@RowsPerPage", RowsPerPage),
                    rowsTotal
                ).AsNoTracking().ToList();

                return new BuscadorDto{
                    Data = lstTem.Select(c => new FnHomologacionEsquemaDataDto()
                    {
                        IdDataLakeOrganizacion = c.IdDataLakeOrganizacion,
                        DataEsquemaJson = JsonConvert.DeserializeObject<List<ColumnaEsquema>>(c.DataEsquemaJson ?? "[]")
                    })
                    .ToList(),
                    TotalCount = (int) rowsTotal.Value
                };
            });
        }
        public List<EsquemaDto> FnHomologacionEsquemaTodo()
        {
            return ExecuteDbOperation(context => {
                return context.Database.SqlQuery<EsquemaDto>($"select * from fnHomologacionEsquemaTodo()").AsNoTracking().OrderBy(c => c.MostrarWebOrden).ToList();
            });
        }
        public HomologacionEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema)
        {
            return ExecuteDbOperation(context => {
                return context.Database.SqlQuery<HomologacionEsquemaDto>($"select * from fnHomologacionEsquema({idHomologacionEsquema})").AsNoTracking().FirstOrDefault();
            });
        }
        public List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idHomologacionEsquema, int idDataLakeOrganizacion)
        {
            return ExecuteDbOperation(context => {
                var lstTem = context.Database.SqlQuery<FnHomologacionEsquemaData>($"select * from fnHomologacionEsquemaDato({idHomologacionEsquema}, {idDataLakeOrganizacion})").AsNoTracking().ToList();

                return lstTem.Select(c => new FnHomologacionEsquemaDataDto()
                {
                    IdDataLakeOrganizacion = c.IdDataLakeOrganizacion,
                    IdHomologacionEsquema = c.IdHomologacionEsquema,
                    DataEsquemaJson = JsonConvert.DeserializeObject<List<ColumnaEsquema>>(c.DataEsquemaJson ?? "[]")
                })
                .ToList();
            });
        }
    }
}