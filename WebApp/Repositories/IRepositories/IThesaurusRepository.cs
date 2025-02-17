using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IThesaurusRepository
    {
        Thesaurus ObtenerThesaurus();
        void GuardarThesaurus(Thesaurus thesaurus);
        void EjecutarArchivoBat();
    }
}
