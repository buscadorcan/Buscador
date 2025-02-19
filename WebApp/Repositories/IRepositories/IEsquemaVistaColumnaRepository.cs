using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IEsquemaVistaColumnaRepository
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza un registro de EsquemaVistaColumna en la base de datos.
         */
        bool Update(EsquemaVistaColumna data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de EsquemaVistaColumna en la base de datos.
         */
        bool Create(EsquemaVistaColumna data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de EsquemaVistaColumna en la base de datos por su identificador único.
         */
        EsquemaVistaColumna? FindById(int Id);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByIdEsquemaVista: Obtiene una lista de columnas de un esquema vista específico.
         */
        List<EsquemaVistaColumna> FindByIdEsquemaVista(int IdEsquemaVista);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByIdEsquemaVistaOna: Obtiene una lista de columnas de un esquema vista específico dentro de un ONA.
         */
        List<EsquemaVistaColumna> FindByIdEsquemaVistaOna(int IdEsquemaVista, int IdOna);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByIdEsquemaVistaOnaAsync: Obtiene de forma asíncrona una lista de columnas de un esquema vista específico dentro de un ONA.
         */
        Task<List<EsquemaVistaColumna>> FindByIdEsquemaVistaOnaAsync(int IdEsquemaVista, int IdOna);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de registros de EsquemaVistaColumna almacenados en la base de datos.
         */
        List<EsquemaVistaColumna> FindAll();

    }
}