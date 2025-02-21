using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Service
{
    public class ThesaurusService(IThesaurusRepository thesaurusRepository) : IThesaurusService
    {
        private readonly IThesaurusRepository _thesaurusRepository = thesaurusRepository;


        ///<summary>
        ///ObtenerThesaurus: Obtiene el objeto del archivo thesaurus
        ///</summary>
        public Thesaurus ObtenerThesaurus()
        {
            return _thesaurusRepository.ObtenerThesaurus();
        }

        ///<summary>
        ///AgregarExpansion: Agrega una nueva expansion al objeto
        ///</summary>
        public string AgregarExpansion(List<string> sinonimos)
        {
            try
            {
                var thesaurus = _thesaurusRepository.ObtenerThesaurus();
                thesaurus.Expansions.Add(new Expansion { Substitutes = sinonimos });
                _thesaurusRepository.GuardarThesaurus(thesaurus);
                return "ok";

            }
            catch 
            {
                throw;
            } 
            
        }

        ///<summary>
        ///AgregarSubAExpansion: Agrega un nuevo sinonimo
        ///</summary>
        public string AgregarSubAExpansion(string expansionExistente, string nuevoSub)
        {
            try
            {
                var thesaurus = _thesaurusRepository.ObtenerThesaurus();

                var expansion = thesaurus.Expansions.FirstOrDefault(e => e.Substitutes.Contains(expansionExistente));
                if (expansion != null)
                {
                    expansion.Substitutes.Add(nuevoSub);
                    _thesaurusRepository.GuardarThesaurus(thesaurus);
                    return "ok";
                }

                return "No se encontró la expansión especificada.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar sub a la expansión: {ex.Message}");
            }
        }

        ///<summary>
        ///ActualizarExpansion: Actualiza la lista de expansiones del thesaurus
        ///</summary>
        public string ActualizarExpansion(List<Expansion> expansions)
        {
            try
            {
                var thesaurus = _thesaurusRepository.ObtenerThesaurus();
                thesaurus.Expansions.Clear();
                foreach (var expansion in expansions) {
                    thesaurus.Expansions.Add(new Expansion { Substitutes = expansion.Substitutes });
                }
               
                _thesaurusRepository.GuardarThesaurus(thesaurus);
                return "ok";

            }
            catch
            {
                throw;
            }

        }

        ///<summary>
        ///EjecutarArchivoBat: realiza el reemplazo del archivo thesaurus en la carpeta de sqlserver
        ///</summary>
        public string EjecutarArchivoBat() {
            try
            {
                _thesaurusRepository.EjecutarArchivoBat();
                return "ok";

            }
            catch
            {
                throw;
            }

        }

        ///<summary>
        ///ResetSQLServer: reinicia el servicio de sqlserver
        ///</summary>
        public string ResetSQLServer()
        {
            try
            {
                var mensaje = _thesaurusRepository.ResetSQLServer();
                return mensaje;

            }
            catch
            {
                throw;
            }

        }

    }
}
