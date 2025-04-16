using SharedApp.Dtos;

namespace Infractruture.Interfaces {
    public interface IDynamicService
    {
        Task<List<PropiedadesTablaDto>> GetProperties(string codigoHomologacion, string viewName);
        Task<List<PropiedadesTablaDto>> GetValueColumna(int idONA, string valueColumn, string viewName);
        Task<List<string>> GetViewNames(string codigoHomologacion);
        //Task<List<EsquemaVistaDto>> GetListaValidacionEsquema(int idOna, int idEsquemaVista);
        Task<List<EsquemaVistaDto>> GetListaValidacionEsquema(int idOna, int idEsquema);
        Task<bool> TestConnectionAsync(int idOna);
        Task<bool> MigrarConexionAsync(int idOna);

    }
}   