using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaDataRepository
  {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza un registro en la base de datos con los datos proporcionados.
         */
        bool Update(EsquemaData data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro en la base de datos.
         */
        EsquemaData? Create(EsquemaData data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateAsync: Crea un nuevo registro en la base de datos de forma as�ncrona.
         */
        Task<EsquemaData?> CreateAsync(EsquemaData data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro en la base de datos por su identificador �nico.
         */
        EsquemaData? FindById(int Id);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la colecci�n completa de registros almacenados en la base de datos.
         */
        ICollection<EsquemaData> FindAll();

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetLastId: Obtiene el �ltimo identificador registrado en la base de datos.
         */
        int GetLastId();

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/DeleteOldRecords: Elimina registros antiguos asociados a un ONA espec�fico.
         */
        bool DeleteOldRecords(int idONA);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/DeleteOldRecordsAsync: Elimina registros antiguos de un esquema de vista de manera as�ncrona.
         */
        Task<bool> DeleteOldRecordsAsync(int IdEsquemaVista);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/DeleteOldRecord: Elimina un registro antiguo espec�fico utilizando identificadores de vista, ente, conexi�n y esquema de homologaci�n.
         */
        bool DeleteOldRecord(string idVista, string idEnte, int idConexion, int idHomologacionEsquema);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/DeleteByExcludingVistaIds: Elimina registros excluyendo aquellos con los identificadores de vista proporcionados.
         */
        bool DeleteByExcludingVistaIds(List<string> idsVista, string idEnte, int idConexion, int idEsquemaData);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/DeleteDataAntigua: Elimina datos antiguos de un ONA espec�fico.
         */
        bool DeleteDataAntigua(int idONA);

    }
}
