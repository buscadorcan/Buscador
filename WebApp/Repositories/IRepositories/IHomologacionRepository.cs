using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IHomologacionRepository
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza un registro de Homologación en la base de datos.
         */
        bool Update(Homologacion data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de Homologación en la base de datos.
         */
        bool Create(Homologacion data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de Homologación en la base de datos por su identificador único.
         */
        Homologacion? FindById(int id);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByParent: Obtiene una colección de registros de homologación asociados a un elemento superior.
         */
        ICollection<Homologacion> FindByParent();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByIds: Busca una lista de registros de Homologación en la base de datos utilizando múltiples identificadores.
         */
        List<Homologacion> FindByIds(int[] ids);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwHomologacionPorCodigo: Obtiene la homologación en base a un código de homologación específico.
         */
        List<VwHomologacion> ObtenerVwHomologacionPorCodigo(string codigoHomologacion);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByAll: Obtiene la lista completa de registros de Homologación almacenados en la base de datos.
         */
        List<Homologacion> FindByAll();

    }
}
