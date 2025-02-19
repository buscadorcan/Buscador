namespace WebApp.Service.IService
{
    public interface IImportador
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Importar: Importa datos desde los archivos especificados en las rutas proporcionadas.
         */
        Boolean Importar(string[] path);
    }
}