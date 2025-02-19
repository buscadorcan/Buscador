using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IUsuarioRepository
  {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un usuario en la base de datos por su identificador único.
         */
        Usuario? FindById(int idUsuario);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByEmail: Busca un usuario en la base de datos por su dirección de correo electrónico.
         */
        Usuario? FindByEmail(string email);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo usuario en la base de datos.
         */
        bool Create(Usuario usuario);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza la información de un usuario en la base de datos.
         */
        bool Update(Usuario usuario);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/IsUniqueUser: Verifica si un nombre de usuario es único en la base de datos.
         */
        bool IsUniqueUser(string usuario);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene la lista completa de usuarios almacenados en la base de datos.
         */
        ICollection<UsuarioDto> FindAll();

    }
}
