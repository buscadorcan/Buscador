using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaRepository
  {
        /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/Update: Actualiza un registro de Esquema en la base de datos.
     */
        bool Update(Esquema data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de Esquema en la base de datos.
         */
        bool Create(Esquema data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro de Esquema en la base de datos por su identificador único.
         */
        Esquema? FindById(int Id);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByViewName: Busca un esquema en la base de datos utilizando el nombre de la vista.
         */
        Esquema? FindByViewName(string esquemaVista);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByViewNameAsync: Busca un esquema en la base de datos de forma asíncrona utilizando el nombre de la vista.
         */
        Task<Esquema?> FindByViewNameAsync(string esquemaVista);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de esquemas almacenados en la base de datos.
         */
        List<Esquema> FindAll();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAllWithViews: Obtiene la lista completa de esquemas junto con sus vistas asociadas.
         */
        List<Esquema> FindAllWithViews();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetListaEsquemaByOna: Obtiene la lista de esquemas asociados a un ONA específico.
         */
        List<Esquema> GetListaEsquemaByOna(int idONA);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/UpdateEsquemaValidacion: Actualiza la validación de un esquema en la base de datos.
         */
        bool UpdateEsquemaValidacion(EsquemaVista data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateEsquemaValidacion: Crea una nueva validación de esquema en la base de datos.
         */
        bool CreateEsquemaValidacion(EsquemaVista data);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/EliminarEsquemaVistaColumnaByIdEquemaVistaAsync: Elimina de manera asíncrona una columna de un esquema vista por su identificador.
         */
        bool EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(int id);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetEsquemaVistaColumnaByIdEquemaVistaAsync: Obtiene una columna de un esquema vista de manera asíncrona en base al ONA y esquema.
         */
        EsquemaVistaColumna? GetEsquemaVistaColumnaByIdEquemaVistaAsync(int idOna, int idEsquema);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GuardarListaEsquemaVistaColumna: Guarda una lista de columnas para un esquema vista específico.
         */
        bool GuardarListaEsquemaVistaColumna(List<EsquemaVistaColumna> listaEsquemaVistaColumna, int? idOna, int? intidEsquema);


    }
}