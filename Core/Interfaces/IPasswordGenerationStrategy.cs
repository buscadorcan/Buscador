namespace Core.Interfaces
{
    public interface IPasswordGenerationStrategy
    {
        /// <summary>
        /// Genera una contraseña aleatoria con la longitud especificada.
        /// </summary>
        /// <param name="length">
        /// La longitud deseada de la contraseña. Debe ser un número positivo mayor a 4 para garantizar seguridad.
        /// </param>
        /// <returns>
        /// Una cadena que representa la contraseña generada aleatoriamente.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Se lanza si el parámetro <paramref name="length"/> es menor a 4.
        /// </exception>
        /// <remarks>
        /// La contraseña generada incluirá una combinación de letras mayúsculas, minúsculas, números y caracteres especiales.
        /// </remarks>
        string GeneratePassword(int length);

    }
}