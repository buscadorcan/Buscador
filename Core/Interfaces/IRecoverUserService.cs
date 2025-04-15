using SharedApp.Dtos;

namespace Core.Interfaces
{
    public interface IRecoverUserService
    {
        /// <summary>
        /// Recupera la contraseña de un usuario generando una nueva contraseña temporal 
        /// y enviándola al correo electrónico asociado.
        /// </summary>
        /// <param name="usuarioRecuperacionDto">Objeto que contiene la información del usuario 
        /// para la recuperación de contraseña.</param>
        /// <returns>Devuelve un objeto <see cref="Result{bool}"/> indicando si la operación fue exitosa o no.</returns>
        Result<bool> RecoverPassword(UsuarioRecuperacionDto usuarioRecuperacionDto);
    }
}
