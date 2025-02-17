using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IThesaurusService
    {
        Thesaurus ObtenerThesaurus();
        string AgregarExpansion(List<string> sinonimos);
        string AgregarSubAExpansion(string expansionExistente, string nuevoSub);
        string ActualizarExpansion(List<Expansion> expansions);
        string EjecutarArchivoBat();
    }
}
