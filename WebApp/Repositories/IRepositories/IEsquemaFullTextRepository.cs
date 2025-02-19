using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaFullTextRepository
  {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de EsquemaFullText en la base de datos.
         */
        EsquemaFullText Create(EsquemaFullText data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateAsync: Crea un nuevo registro de EsquemaFullText en la base de datos de forma asíncrona.
         */
        Task<EsquemaFullText> CreateAsync(EsquemaFullText data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de EsquemaFullText en la base de datos por su identificador único.
         */
        EsquemaFullText? FindById(int Id);

    }
}
