using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IUsuarioEndpointRepository
  {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de registros de UsuarioEndpoint almacenados en la base de datos.
         */
        ICollection<UsuarioEndpoint> FindAll();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByEndpointId: Busca un registro de UsuarioEndpoint en la base de datos por su identificador de endpoint.
         */
        UsuarioEndpoint? FindByEndpointId(int endpointId);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro de UsuarioEndpoint en la base de datos.
         */
        bool Create(UsuarioEndpoint UsuarioEndpoint);

    }
}
