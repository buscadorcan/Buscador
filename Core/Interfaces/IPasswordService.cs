namespace Core.Interfaces
{
    public interface IPasswordService
    {
        /// <summary>
        /// Genera una contraseña temporal aleatoria con la longitud especificada.
        /// </summary>
        /// <param name="length">
        /// La longitud deseada de la contraseña temporal. Debe ser un número positivo mayor o igual a 6 para garantizar seguridad.
        /// </param>
        /// <returns>
        /// Una cadena de texto que representa la contraseña temporal generada aleatoriamente.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Se lanza si el parámetro <paramref name="length"/> es menor a 6.
        /// </exception>
        /// <remarks>
        /// La contraseña generada incluirá una combinación de letras mayúsculas, minúsculas, números y caracteres especiales 
        /// para mejorar la seguridad. Está diseñada para ser usada como una clave temporal que el usuario debe cambiar posteriormente.
        /// </remarks>
        string GenerateTemporaryPassword(int length);

    }
}