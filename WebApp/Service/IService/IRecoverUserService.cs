using SharedApp.Models.Dtos;

namespace WebApp.Service.IService
{
    public interface IRecoverUserService
    {
        /// <summary>
        /// Recovers a user's password by generating a temporary password and sending it via email.
        /// </summary>
        /// <param name="usuarioRecuperacionDto">The data transfer object containing the user's email for password recovery.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> object containing a boolean value indicating success or failure.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if an error occurs during the password recovery process.
        /// </exception>
        Task<Result<bool>> RecoverPassword(UsuarioRecuperacionDto usuarioRecuperacionDto);
    }
}
