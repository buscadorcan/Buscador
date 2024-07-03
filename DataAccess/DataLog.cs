using System.Diagnostics.CodeAnalysis;

namespace DataAccess
{
  [ExcludeFromCodeCoverage]
    public class DataLog
    {
        public void CreateDataBaseLog()
        {
            DateTime currentDate = DateTime.Now;

            string fileNameFormat = "yyyyMMdd_HHmmss";

            string fileName = currentDate.ToString(fileNameFormat) + ".txt";

            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            string content = "Hola, este archivo fue creado el " + currentDate.ToString();

            File.WriteAllText(filePath, content);
            Console.WriteLine("Archivo creado con éxito en: " + filePath);
        }
    }
}