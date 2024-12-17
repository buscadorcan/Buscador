using System.Security.Cryptography;
using WebApp.Service.IService;
using System.Text;

public class RandomPasswordGenerationStrategy : IPasswordGenerationStrategy
{
  /// <summary>
  /// Genera una contraseña aleatoria de longitud especificada.
  /// </summary>
  /// <param name="length">La longitud de la contraseña que se generará.</param>
  /// <returns>Una contraseña aleatoria generada.</returns>
  public string GeneratePassword(int length)
  {
    const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    StringBuilder res = new StringBuilder();
    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
      byte[] uintBuffer = new byte[sizeof(uint)];

      while (length-- > 0)
      {
        rng.GetBytes(uintBuffer);
        uint num = BitConverter.ToUInt32(uintBuffer, 0);
        res.Append(validChars[(int)(num % (uint)validChars.Length)]);
      }
    }

    return res.ToString();
  }
}
