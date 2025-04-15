using SharedApp.Dtos;
using DataAccess.Models;

namespace DataAccess.Interfaces
{
    /// <summary>
    /// Interfaz que define las operaciones necesarias para el seguimiento de eventos en la aplicación.
    /// </summary>
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
        /// <param name="CodigoHomologacionRol">Tipo de usuario asociado.</param>
        /// <param name="CodigoHomologacionMenu">Nombre de la página de acceso.</param>
        /// <returns>Devuelve una cadena de texto con el código correspondiente al usuario.</returns>
        string GetCodeByUser(string nombreUsuario, string CodigoHomologacionRol, string CodigoHomologacionMenu);

        /// <summary>
        /// Busca los datos de un menú específico asociado a un rol en la base de datos.
        /// </summary>
        /// <param name="idHRol">Identificador del rol.</param>
        /// <param name="idHMenu">Identificador del menú.</param>
        /// <returns>Devuelve un objeto <see cref="Menus"/> si se encuentra, de lo contrario <c>null</c>.</returns>
        Menus? FindDataById(int idHRol, int idHMenu);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<VwEventUserAll> GetEventUserAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <param name="fini"></param>
        /// <param name="ffin"></param>
        /// <returns></returns>
        List<EventUser> GetEventAll(string report, DateOnly fini, DateOnly ffin);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fini"></param>
        /// <param name="ffin"></param>
        /// <returns></returns>
        bool DeleteEventAll(DateOnly fini, DateOnly ffin);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteEventById(int id);

        /// <summary>
        /// Consulta los inicios de sesion 
        /// </summary>
        /// <returns></returns>
        List<VwEventTrackingSessionDto> GetEventSession();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<PaginasMasVisitadaDto> GetEventPagMasVisit();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<FiltrosMasUsadoDto> GetEventFiltroMasUsa();
    }
}
