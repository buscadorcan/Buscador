using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    public interface IEventTrackingRepository
    {
        /// <summary>
        /// Registra un nuevo evento de seguimiento en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="paAddEventTrackingDto"/> con la información del evento a registrar.</param>
        /// <returns>Devuelve un valor booleano indicando si el registro fue exitoso.</returns>
        bool Create(paAddEventTrackingDto data);

        /// <summary>
        /// Obtiene un código asociado a un usuario en función de su nombre, tipo y página de acceso.
        /// </summary>
        /// <param name="nombreUsuario">Nombre del usuario para el cual se busca el código.</param>
        /// <param name="tipoUsuario">Tipo de usuario asociado.</param>
        /// <param name="nombrePagina">Nombre de la página de acceso.</param>
        /// <returns>Devuelve una cadena de texto con el código correspondiente al usuario.</returns>
        string GetCodeByUser(string nombreUsuario, string tipoUsuario, string nombrePagina);

    }
}
