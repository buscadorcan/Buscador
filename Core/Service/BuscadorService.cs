

using Core.Interfaces;
using DataAccess.Interfaces;
using SharedApp.Dtos;

namespace Core.Service
{
    public class BuscadorService : IBuscadorService
    {
        private readonly IBuscadorRepository _buscadorRepository;

        public BuscadorService(IBuscadorRepository buscadorRepository)
        {
            this._buscadorRepository = buscadorRepository;
        }

        public int AddEventTracking(EventTrackingDto eventTracking)
        {
            return _buscadorRepository.AddEventTracking(eventTracking);
        }

        public fnEsquemaCabeceraDto? FnEsquemaCabecera(int IdEsquemadata)
        {
            return _buscadorRepository.FnEsquemaCabecera(IdEsquemadata);
        }

        public List<FnEsquemaDataBuscadoDto> FnEsquemaDatoBuscar(int IdEsquemaData, string TextoBuscar)
        {
            return _buscadorRepository.FnEsquemaDatoBuscar(IdEsquemaData, TextoBuscar);
        }

        public FnEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema)
        {
            return _buscadorRepository.FnHomologacionEsquema(idHomologacionEsquema);
        }

        public List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idEsquema, string VistaFK, int idOna)
        {
            return _buscadorRepository.FnHomologacionEsquemaDato(idEsquema, VistaFK, idOna);
        }

        public List<EsquemaDto> FnHomologacionEsquemaTodo(string VistaFK, int idOna)
        {
            return _buscadorRepository.FnHomologacionEsquemaTodo(VistaFK, idOna);
        }

        public List<FnPredictWordsDto> FnPredictWords(string word)
        {
            return _buscadorRepository.FnPredictWords(word);
        }

        public BuscadorDto PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage)
        {
            return _buscadorRepository.PsBuscarPalabra(paramJSON, PageNumber, RowsPerPage);
        }

        public bool ValidateWords(List<string> words)
        {
            return _buscadorRepository.ValidateWords(words);
        }
    }
}
