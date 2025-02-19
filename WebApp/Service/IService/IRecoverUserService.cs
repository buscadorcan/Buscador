using SharedApp.Models.Dtos;

namespace WebApp.Service.IService
{
    public interface IRecoverUserService
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/RecoverPassword: Recupera la contraseña de un usuario generando una contraseña temporal y enviándola por correo electrónico.
         */
        Task<Result<bool>> RecoverPassword(UsuarioRecuperacionDto usuarioRecuperacionDto);
    }
}
