using Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Core.Service
{
    public class RandomStringGeneratorService : IRandomStringGeneratorService
    {
        // Define valid characters and digits for password and code generation
        private const string ValidChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string ValidDigits = "0123456789";

        /// <inheritdoc />
        public string GenerateTemporaryPassword(int length)
        {
            return Generate(length, ValidChars);
        }

        /// <inheritdoc />
        public string GenerateTemporaryCode(int length)
        {
            return Generate(length, ValidDigits);
        }

        /// <summary>
        /// Generates a random string using a specified set of valid characters.
        /// </summary>
        /// <param name="length">The length of the generated string.</param>
        /// <param name="validChars">A string of valid characters from which to generate the string.</param>
        /// <returns>A randomly generated string.</returns>
        private string Generate(int length, string validChars)
        {
            // Use StringBuilder for efficient string concatenation
            StringBuilder result = new StringBuilder();

            // Initialize a cryptographically secure random number generator
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    // Generate a random number and convert it to an index within validChars
                    rng.GetBytes(uintBuffer);
                    uint randomNum = BitConverter.ToUInt32(uintBuffer, 0);
                    result.Append(validChars[(int)(randomNum % (uint)validChars.Length)]);
                }
            }

            // Return the generated string
            return result.ToString();
        }
    }
}
