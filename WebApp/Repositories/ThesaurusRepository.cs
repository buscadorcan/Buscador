using System.Diagnostics;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Org.BouncyCastle.Asn1.X509;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebApp.Repositories
{
    public class ThesaurusRepository(IConfiguration configuration, IWebHostEnvironment env) : IThesaurusRepository
    {
        private readonly string _rutaArchivo = configuration["Thesaurus:RutaGuardado"];
        private readonly string _rutaArchivoDestino = configuration["Thesaurus:RutaFdata"];
        private readonly IWebHostEnvironment _env = env;


        public Thesaurus ObtenerThesaurus()
        {
            var rutaProyecto = Directory.GetCurrentDirectory();
            string rutaArchivo = Path.Combine(rutaProyecto, _rutaArchivo);
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

                var archivo = (Thesaurus)serializer.Deserialize(reader);
               

                return archivo ?? new Thesaurus();

               
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al procesar el archivo XML: {ex.Message}");
            }
        }

        public void GuardarThesaurus(Thesaurus thesaurus)
        {
            string rutaArchivo = "";
            try
            {
                var rutaProyecto = Directory.GetCurrentDirectory();
                rutaArchivo = Path.Combine(rutaProyecto, _rutaArchivo);
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
                throw new Exception($"Error al guardar el archivo XML: {ex.Message} ruta:"+ rutaArchivo);
            }
        }

        public void EjecutarArchivoBat() {

            try
            {
                var rutaProyecto = Directory.GetCurrentDirectory();
                string rutaArchivo = Path.Combine(rutaProyecto, _rutaArchivo);
                string rutaArchiDestino = _rutaArchivoDestino;


                File.Copy(rutaArchivo, rutaArchiDestino, true);
                ResetSQLServer();


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ejecutando el archivo .bat: {ex.Message}");
            }
        }

        public void ResetSQLServer()
        {
            string serviceName = "MSSQLSERVER"; // Nombre del servicio SQL Server
            ServiceController service = new ServiceController(serviceName);

            try
            {
                Console.WriteLine($"Estado actual del servicio: {service.Status}");

                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("Deteniendo el servicio...");
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(1));
                    Console.WriteLine("Servicio detenido.");
                }

                Console.WriteLine("Iniciando el servicio...");
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1));
                Console.WriteLine("Servicio iniciado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
