namespace WebApp.Service.IService
{
    public interface IRandomStringGeneratorService
    {
        /// <summary>
        /// Generates a temporary password using a random selection of characters.
        /// </summary>
        /// <param name="length">The length of the generated password.</param>
        /// <returns>A randomly generated temporary password.</returns>
        string GenerateTemporaryPassword(int length);

        /// <summary>
        /// Generates a temporary code using a random selection of digits.
        /// </summary>
        /// <param name="length">The length of the generated code.</param>
        /// <returns>A randomly generated temporary code.</returns>
        string GenerateTemporaryCode(int length);
    }
}