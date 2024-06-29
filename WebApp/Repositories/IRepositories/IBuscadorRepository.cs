using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories {
    public interface IBuscadorRepository
    {
        object PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage);
        ICollection<EsquemaDto> FnHomologacionEsquemaTodo();
        HomologacionEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema);
        ICollection<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idHomologacionEsquema, int idDataLakeOrganizacion);
    }
}