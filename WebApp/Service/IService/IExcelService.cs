
using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IExcelService
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ImportarExcel: Importa un archivo de Excel desde la ruta especificada y registra la migración en el sistema para un ONA determinado.
         */
        Task<Boolean> ImportarExcel(string path, LogMigracion migracion, int idOna);
    }
}