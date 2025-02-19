using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IHomologacionRepository
    {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza un registro de Homologaci�n en la base de datos.
         */
        bool Update(Homologacion data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de Homologaci�n en la base de datos.
         */
        bool Create(Homologacion data);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de Homologaci�n en la base de datos por su identificador �nico.
         */
        Homologacion? FindById(int id);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByParent: Obtiene una colecci�n de registros de homologaci�n asociados a un elemento superior.
         */
        ICollection<Homologacion> FindByParent();

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByIds: Busca una lista de registros de Homologaci�n en la base de datos utilizando m�ltiples identificadores.
         */
        List<Homologacion> FindByIds(int[] ids);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwHomologacionPorCodigo: Obtiene la homologaci�n en base a un c�digo de homologaci�n espec�fico.
         */
        List<VwHomologacion> ObtenerVwHomologacionPorCodigo(string codigoHomologacion);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByAll: Obtiene la lista completa de registros de Homologaci�n almacenados en la base de datos.
         */
        List<Homologacion> FindByAll();

    }
}
