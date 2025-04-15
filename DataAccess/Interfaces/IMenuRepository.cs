using DataAccess.Models;

namespace DataAccess.Interfaces
{
    public interface IMenuRepository
    {
        /// <summary>
        /// Actualiza un registro de <see cref="MenuRol"/> en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="MenuRol"/> con los datos actualizados.</param>
        /// <returns>Devuelve <c>true</c> si la actualización fue exitosa, de lo contrario <c>false</c>.</returns>
        bool Update(MenuRol data);

        /// <summary>
        /// Crea un nuevo registro de <see cref="MenuRol"/> en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="MenuRol"/> con los datos a insertar.</param>
        /// <returns>Devuelve <c>true</c> si la creación fue exitosa, de lo contrario <c>false</c>.</returns>
        bool Create(MenuRol data);

        /// <summary>
        /// Busca los datos de un menú específico asociado a un rol en la base de datos.
        /// </summary>
        /// <param name="idHRol">Identificador del rol.</param>
        /// <param name="idHMenu">Identificador del menú.</param>
        /// <returns>Devuelve un objeto <see cref="Menus"/> si se encuentra, de lo contrario <c>null</c>.</returns>
        Menus? FindDataById(int idHRol, int idHMenu);

        /// <summary>
        /// Busca un registro de <see cref="MenuRol"/> en la base de datos por su identificador único.
        /// </summary>
        /// <param name="idHRol">Identificador del rol.</param>
        /// <param name="idHMenu">Identificador del menú.</param>
        /// <returns>Devuelve un objeto <see cref="MenuRol"/> si se encuentra, de lo contrario <c>null</c>.</returns>
        MenuRol? FindById(int idHRol, int idHMenu);

        /// <summary>
        /// Obtiene la lista completa de registros de <see cref="Menus"/> almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve una lista de objetos <see cref="Menus"/>.</returns>
        List<Menus> FindAll();

        /// <summary>
        /// Obtiene de forma asíncrona una lista de menús asociados a un rol específico.
        /// </summary>
        /// <param name="idHRol">Identificador del rol.</param>
        /// <param name="idHMenu">Identificador del menú.</param>
        /// <returns>Devuelve una lista de objetos <see cref="Menus"/> asociados al rol y menú especificados.</returns>
        List<Menus> GetListByMenusAsync(int idHRol, int idHMenu);
        List<MenuPagina> ObtenerMenusPendingConfig(int idHomologacionRol);

    }
}


