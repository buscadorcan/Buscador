namespace WebApp.Repositories.IRepositories
{
    public interface IpaActualizarFiltroRepository
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ActualizarFiltroAsync: Actualiza de forma asíncrona los filtros almacenados en la base de datos.
         */
        Task<bool> ActualizarFiltroAsync();
    }
}
