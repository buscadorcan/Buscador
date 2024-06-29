using System.Security.Cryptography;
using System.Text;
using WebApp.Service.IService;

public class Md5HashStrategy : IHashStrategy
{
    public string ComputeHash(string? input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] data = Encoding.UTF8.GetBytes(input ?? "");
            byte[] hash = md5.ComputeHash(data);
                
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
