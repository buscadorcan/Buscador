using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    /// <summary>
    /// Implementa el repositorio para la lógica de búsqueda en la base de datos.
    /// Esta clase proporciona métodos para realizar búsquedas, obtener esquemas y homologaciones de datos.
    /// </summary>
    public class BuscadorRepository : BaseRepository, IBuscadorRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="BuscadorRepository"/>.
        /// </summary>
        /// <param name="logger">El logger utilizado para registrar eventos.</param>
        /// <param name="sqlServerDbContextFactory">La fábrica de contexto para la base de datos SQL Server.</param>
        public BuscadorRepository(
            ILogger<UsuarioRepository> logger,
            ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
        }

        /// <summary>
        /// Realiza una búsqueda de palabra utilizando parámetros JSON, número de página y filas por página.
        /// Ejecuta el procedimiento almacenado <c>paBuscarPalabra</c> para obtener los resultados.
        /// </summary>
        /// <param name="paramJSON">El parámetro JSON utilizado para filtrar los resultados.</param>
        /// <param name="PageNumber">El número de la página actual para paginación.</param>
        /// <param name="RowsPerPage">El número de filas por página.</param>
        /// <returns>Un objeto <see cref="ResultPaBuscarPalabraDto"/> con los resultados de la búsqueda.</returns>
        public ResultPaBuscarPalabraDto BuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage)
        {
            return ExecuteDbOperation(context => {
                var rowsTotal = new SqlParameter {
                    ParameterName = "@RowsTotal",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                var lstTem = context.Database.SqlQueryRaw<PaBuscarPalabra>(
                    "exec paBuscarPalabra @paramJSON, @PageNumber, @RowsPerPage, @RowsTotal OUT",
                    new SqlParameter("@paramJSON", paramJSON),
                    new SqlParameter("@PageNumber", PageNumber),
                    new SqlParameter("@RowsPerPage", RowsPerPage),
                    rowsTotal
                ).AsNoTracking().ToList();

                return new ResultPaBuscarPalabraDto{
                    Data = lstTem.Select(c => new ResultDataPaBuscarPalabraDto() {
                        IdEsquema = c.IdEsquema,
                        VistaPK = c.VistaPK,
                        IdEsquemaData = c.IdEsquemaData,
                        DataEsquemaJson = JsonConvert.DeserializeObject<List<ColumnaEsquemaDto>>(c.DataEsquemaJson ?? "[]")
                    }).ToList(),
                    TotalCount = (int) rowsTotal.Value
                };
            });
        }

        /// <summary>
        /// Obtiene todos los esquemas homologados disponibles para un ente específico.
        /// Ejecuta la función <c>fnHomologacionEsquemaTodo</c> para obtener los esquemas de homologación.
        /// </summary>
        /// <param name="idEnte">El identificador del ente para el cual se obtendrán los esquemas.</param>
        /// <returns>Una lista de objetos <see cref="EsquemaDto"/> que representan los esquemas homologados.</returns>
        public List<EsquemaDto> FnHomologacionEsquemaTodo(string idEnte)
        {
            return ExecuteDbOperation(context => {
                return context.Database.SqlQuery<EsquemaDto>($"select * from fnHomologacionEsquemaTodo({idEnte})").AsNoTracking().OrderBy(c => c.MostrarWebOrden).ToList();
            });
        }

        /// <summary>
        /// Obtiene los detalles de un esquema de homologación específico.
        /// Ejecuta la función <c>fnHomologacionEsquema</c> para obtener la información del esquema.
        /// </summary>
        /// <param name="idHomologacionEsquema">El identificador del esquema de homologación a obtener.</param>
        /// <returns>Un objeto <see cref="FnHomologacionEsquemaDto"/> que contiene los detalles del esquema de homologación.</returns>
        public FnHomologacionEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema)
        {
            return ExecuteDbOperation(context => {
                return context.Database.SqlQuery<FnHomologacionEsquemaDto>($"select * from fnHomologacionEsquema({idHomologacionEsquema})").AsNoTracking().FirstOrDefault();
            });
        }

        /// <summary>
        /// Obtiene los datos asociados a un esquema de homologación específico para un ente dado.
        /// Ejecuta la función <c>fnHomologacionEsquemaDato</c> para obtener los datos de homologación.
        /// </summary>
        /// <param name="idHomologacionEsquema">El identificador del esquema de homologación.</param>
        /// <param name="idEnte">El identificador del ente para el cual se obtendrán los datos.</param>
        /// <returns>Una lista de objetos <see cref="FnHomologacionEsquemaDataDto"/> que contienen los datos de homologación.</returns>
        public List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idHomologacionEsquema, string idEnte)
        {
            return ExecuteDbOperation(context => {
                var lstTem = context.Database.SqlQuery<FnHomologacionEsquemaData>($"select * from fnHomologacionEsquemaDato({idHomologacionEsquema}, {idEnte})").AsNoTracking().ToList();

                return lstTem.Select(c => new FnHomologacionEsquemaDataDto() {
                    IdCanDataSet = c.IdCanDataSet,
                    IdHomologacionEsquema = c.IdHomologacionEsquema,
                    DataEsquemaJson = JsonConvert.DeserializeObject<List<ColumnaEsquemaDto>>(c.DataEsquemaJson ?? "[]")
                }).ToList();
            });
        }

        /// <summary>
        /// Predice las palabras asociadas a un término de búsqueda dado.
        /// Ejecuta la función <c>fnPredictWord</c> para obtener las predicciones de palabras.
        /// </summary>
        /// <param name="word">La palabra utilizada para predecir palabras asociadas.</param>
        /// <returns>Una lista de objetos <see cref="FnPredictWordsDto"/> que representan las palabras predichas.</returns>
        public List<FnPredictWordsDto> FnPredictWords(string word)
        {
            return ExecuteDbOperation(context => {
                return context.Database.SqlQuery<FnPredictWordsDto>($"select * from fnPredictWord({word})").AsNoTracking().OrderBy(c => c.Word).ToList();
            });
        }
    }
}
