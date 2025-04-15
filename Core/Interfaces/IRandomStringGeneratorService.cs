namespace Core.Interfaces
{
    public interface IRandomStringGeneratorService
    {
        /// <summary>
        /// Genera una contraseña temporal aleatoria utilizando una combinación de caracteres alfanuméricos.
        /// </summary>
        /// <param name="length">Longitud de la contraseña generada.</param>
        /// <returns>Devuelve una cadena de caracteres que representa la contraseña generada.</returns>
        string GenerateTemporaryPassword(int length);

        /// <summary>
        /// Genera un código temporal aleatorio compuesto únicamente de dígitos.
        /// </summary>
        /// <param name="length">Longitud del código generado.</param>
        /// <returns>Devuelve una cadena de caracteres numéricos que representa el código generado.</returns>
        string GenerateTemporaryCode(int length);

    }
}