using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaVistaRepository
  {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza un registro de EsquemaVista en la base de datos.
         */
        bool Update(EsquemaVista data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de EsquemaVista en la base de datos.
         */
        bool Create(EsquemaVista data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de EsquemaVista en la base de datos por su identificador único.
         */
        EsquemaVista? FindById(int Id);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByIdEsquema: Busca un esquema vista en la base de datos por su identificador de esquema.
         */
        EsquemaVista? FindByIdEsquema(int IdEsquema);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/_FindByIdEsquema: Busca un esquema vista en la base de datos utilizando el identificador de esquema y el ONA.
         */
        EsquemaVista? _FindByIdEsquema(int IdEsquema, int idOna);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/_FindByIdEsquemaAsync: Busca de forma asíncrona un esquema vista en la base de datos utilizando el identificador de esquema y el ONA.
         */
        Task<EsquemaVista?> _FindByIdEsquemaAsync(int IdEsquema, int idOna);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de registros de EsquemaVista almacenados en la base de datos.
         */
        List<EsquemaVista> FindAll();

    }
}