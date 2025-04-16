namespace Core.Services
{
    public interface IHashStrategy
    {
        /// <summary>
        /// Calcula un hash seguro basado en la cadena de entrada proporcionada.
        /// </summary>
        /// <param name="input">La cadena de texto de la cual se calculará el hash. Puede ser nula.</param>
        /// <returns>
        /// Un string que representa el hash generado.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si el parámetro <paramref name="input"/> es nulo y la implementación no admite valores nulos.
        /// </exception>
        /// <remarks>
        /// Este método utiliza un algoritmo criptográfico seguro, como SHA-256 o SHA-512,
        /// para generar un hash único de la entrada.
        /// </remarks>
        string ComputeHash(string? input);

    }
}
