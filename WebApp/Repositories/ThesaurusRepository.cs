using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class ThesaurusRepository(IConfiguration configuration, IWebHostEnvironment env) : IThesaurusRepository
    {
        private readonly string _rutaArchivo = configuration["Thesaurus:RutaGuardado"];
        private readonly string _rutaArchivoBat = configuration["Thesaurus:RutaArchivoBat"];
        private readonly IWebHostEnvironment _env = env;
        public Thesaurus ObtenerThesaurus()
        {
            var rutaProyecto = Directory.GetCurrentDirectory();
            string rutaArchivo = Path.Combine(rutaProyecto, "Files", _rutaArchivo);
            if (!File.Exists(rutaArchivo))
            {
                return new Thesaurus();
            }

            try
            {
                string xmlContent = File.ReadAllText(rutaArchivo);

                // Extraemos solo el contenido de <thesaurus>...</thesaurus>: luis ricopa
                string pattern = @"<thesaurus[\s\S]*?</thesaurus>";
                Match match = Regex.Match(xmlContent, pattern, RegexOptions.IgnoreCase);

                if (!match.Success)
                {
                    throw new Exception("No se encontró el nodo <thesaurus> en el archivo XML.");
                }

                string xmlLimpio = match.Value; // Solo la parte relevante del XML: luis ricopa

                // Deserializar el XML limpio: Luis ricoa
                XmlSerializer serializer = new XmlSerializer(typeof(Thesaurus));
                using StringReader reader = new StringReader(xmlLimpio);
                return (Thesaurus)serializer.Deserialize(reader) ?? new Thesaurus();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al procesar el archivo XML: {ex.Message}");
            }
        }

        public void GuardarThesaurus(Thesaurus thesaurus)
        {
            try
            {
                var rutaProyecto = Directory.GetCurrentDirectory();
                string rutaArchivo = Path.Combine(rutaProyecto, "Files", _rutaArchivo);
                if (!File.Exists(rutaArchivo))
                {
                    throw new Exception("El archivo XML no existe.");
                }

                string xmlContent = File.ReadAllText(rutaArchivo);
                XmlSerializer serializer = new XmlSerializer(typeof(Thesaurus));

                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "x-schema:tsSchema.xml"); 

                using StringWriter writer = new StringWriter();
                using XmlWriter xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });

                serializer.Serialize(xmlWriter, thesaurus, namespaces);
                string nuevoThesaurusXML = writer.ToString();

                string pattern = @"<thesaurus[\s\S]*?</thesaurus>";

                if (!Regex.IsMatch(xmlContent, pattern))
                {
                    throw new Exception("No se encontró el nodo <thesaurus> en el archivo XML.");
                }

                string nuevoXML = Regex.Replace(xmlContent, pattern, nuevoThesaurusXML);

                File.WriteAllText(rutaArchivo, nuevoXML);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar el archivo XML: {ex.Message}");
            }
        }

        public void EjecutarArchivoBat() {

            try
            {
                var rutaProyecto = Directory.GetCurrentDirectory();
                string rutaArchivo = Path.Combine(rutaProyecto, "Files", _rutaArchivoBat);
                if (string.IsNullOrEmpty(rutaArchivo))
                {
                    throw new Exception("La ruta del archivo .bat no está configurada en appsettings.json");
                }

                var proceso = new ProcessStartInfo
                {
                    FileName = rutaArchivo,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = proceso })
                {
                    process.Start();
                    string resultado = process.StandardOutput.ReadToEnd();
                    string errores = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(errores))
                    {
                        throw new Exception($"Error al ejecutar el archivo BAT: {errores}");
                    }

                    Console.WriteLine("Resultado de la ejecución:");
                    Console.WriteLine(resultado);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ejecutando el archivo .bat: {ex.Message}");
            }
        }
    }
}
