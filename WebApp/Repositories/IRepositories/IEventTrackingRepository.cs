using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    public interface IEventTrackingRepository
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Registra un nuevo evento de seguimiento en la base de datos.
         */
        bool Create(paAddEventTrackingDto data);
    }
}
