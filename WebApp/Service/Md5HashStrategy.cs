using System.Security.Cryptography;
using System.Text;
using WebApp.Service.IService;

public class Md5HashStrategy : IHashStrategy
{
  /// <summary>
  /// Calcula el hash MD5 de la entrada proporcionada.
  /// </summary>
  /// <param name="input">La cadena de texto de la cual se generará el hash MD5.</param>
  /// <returns>El hash MD5 de la entrada como una cadena en formato hexadecimal.</returns>
  public string ComputeHash(string? input)
  {
    using (MD5 md5 = MD5.Create()) // Se crea una instancia de MD5 para calcular el hash.
    {
      // Convierte la entrada en un arreglo de bytes con codificación UTF-8. Si la entrada es nula, se convierte a una cadena vacía.
      byte[] data = Encoding.UTF8.GetBytes(input ?? "");
            
      // Calcula el hash MD5 de los bytes de entrada.
      byte[] hash = md5.ComputeHash(data);
                
      // Se construye una cadena hexadecimal que representa el hash calculado.
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < hash.Length; i++)
      {
        sb.Append(hash[i].ToString("x2")); // Convierte cada byte en un formato hexadecimal de 2 dígitos.
      }

      return sb.ToString(); // Devuelve el hash en formato hexadecimal.
    }
  }
}
