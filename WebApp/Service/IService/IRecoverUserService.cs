using SharedApp.Models.Dtos;

namespace WebApp.Service.IService
{
    public interface IRecoverUserService
    {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/RecoverPassword: Recupera la contrase�a de un usuario generando una contrase�a temporal y envi�ndola por correo electr�nico.
         */
        Result<bool> RecoverPassword(UsuarioRecuperacionDto usuarioRecuperacionDto);
    }
}
