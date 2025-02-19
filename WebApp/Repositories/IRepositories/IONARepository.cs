using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IONARepository
  {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza un registro de ONA en la base de datos.
         */
        bool Update(ONA data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de ONA en la base de datos.
         */
        bool Create(ONA data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de ONA en la base de datos por su identificador �nico.
         */
        ONA? FindById(int Id);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindBySiglas: Busca un registro de ONA en la base de datos por sus siglas.
         */
        ONA? FindBySiglas(string siglas);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de registros de ONA almacenados en la base de datos.
         */
        List<ONA> FindAll();

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAllPaises: Obtiene la lista de pa�ses relacionados con las ONAs.
         */
        List<VwPais> FindAllPaises();

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByIdAsync: Busca de forma as�ncrona un registro de ONA en la base de datos por su identificador �nico.
         */
        Task<ONA?> FindByIdAsync(int Id);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetListByONAsAsync: Obtiene de forma as�ncrona una lista de ONAs en base a un identificador espec�fico.
         */
        List<ONA> GetListByONAsAsync(int idOna);

    }
}
