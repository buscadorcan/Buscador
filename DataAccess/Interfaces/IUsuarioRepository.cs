using SharedApp.Dtos;
using DataAccess.Models;

namespace DataAccess.Interfaces
{
  public interface IUsuarioRepository
  {
        /// <summary>
        /// Busca un usuario en la base de datos por su identificador único.
        /// </summary>
        /// <param name="idUsuario">El identificador único del usuario.</param>
        /// <returns>Devuelve un objeto <see cref="Usuario"/> si se encuentra, de lo contrario, devuelve null.</returns>
        Usuario? FindById(int idUsuario);

        /// <summary>
        /// Busca un usuario en la base de datos por su dirección de correo electrónico.
        /// </summary>
        /// <param name="email">La dirección de correo electrónico del usuario.</param>
        /// <returns>Devuelve un objeto <see cref="Usuario"/> si se encuentra, de lo contrario, devuelve null.</returns>
        Usuario? FindByEmail(string email);

        /// <summary>
        /// Crea un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="usuario">Objeto de tipo <see cref="Usuario"/> que contiene los datos a almacenar.</param>
        /// <returns>Devuelve un valor booleano que indica si la operación fue exitosa.</returns>
        bool Create(Usuario usuario);

        /// <summary>
        /// Actualiza la información de un usuario en la base de datos.
        /// </summary>
        /// <param name="usuario">Objeto de tipo <see cref="Usuario"/> con los datos actualizados.</param>
        /// <returns>Devuelve un valor booleano que indica si la operación fue exitosa.</returns>
        bool Update(Usuario usuario);

        /// <summary>
        /// Verifica si un nombre de usuario es único en la base de datos.
        /// </summary>
        /// <param name="usuario">El nombre de usuario a verificar.</param>
        /// <returns>Devuelve true si el usuario es único, de lo contrario, false.</returns>
        bool IsUniqueUser(string usuario);

        /// <summary>
        /// Obtiene la lista completa de usuarios almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve una colección de objetos <see cref="UsuarioDto"/>.</returns>
        ICollection<UsuarioDto> FindAll();


        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ChangePasswd: Cambia de clave de acceso del usuario.
         */
        Result<bool> ChangePasswd(string clave, string claveNueva, int idUsuario, string nueva);

    }
}
