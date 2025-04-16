namespace Core.Services
{
    public interface IHashService
    {
        /// <summary>
        /// Genera un hash único basado en la cadena de entrada proporcionada.
        /// </summary>
        /// <param name="input">La cadena de texto de la cual se generará el hash. Puede ser nula.</param>
        /// <returns>
        /// Un string con el hash generado. Si la entrada es nula o vacía, se devolverá un hash predeterminado.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si el parámetro <paramref name="input"/> es nulo y la implementación no admite valores nulos.
        /// </exception>
        /// <remarks>
        /// Este método utiliza un algoritmo de hashing seguro como SHA-256 o SHA-512.
        /// El resultado puede variar dependiendo de la implementación.
        /// </remarks>
        string GenerateHash(string? input);

    }
}
