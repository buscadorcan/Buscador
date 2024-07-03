using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories {
    public interface IBuscadorRepository
    {
        BuscadorDto PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage);
        List<EsquemaDto> FnHomologacionEsquemaTodo();
        HomologacionEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema);
        List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idHomologacionEsquema, int idDataLakeOrganizacion);
    }
}