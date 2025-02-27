using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Tls;
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
        private readonly string _IpSqlServer = configuration["Thesaurus:IpServidorSqlServer"];
        private readonly string _rutaComando = configuration["Thesaurus:RutaArchivoComando"];
        //nombreServicioSqlServer
        private readonly IWebHostEnvironment _env = env;

        ///<summary>
        ///ObtenerThesaurus: Obtiene la información completa del thesaurus almacenado en la base de datos.
        ///</summary>
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

        ///<summary>
        ///GuardarThesaurus: Guarda o actualiza el thesaurus en la base de datos.
        ///</summary>
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

        ///<summary>
        ///EjecutarArchivoBat: Ejecuta un archivo .bat en el servidor para automatizar procesos relacionados con el thesaurus.
        ///</summary>
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

        ///<summary>
        ///ResetSQLServer: actualiza el servidor de sqlserver
        ///</summary>
        public string ResetSQLServer()
        {
            string rutaComando = _rutaComando;

            try
            {
                if (!File.Exists(rutaComando))
                {
                    return "No existe el archivo de reinicio de servidor de sqlserver";

                }
                using (FileStream fs = new FileStream(_rutaComando, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine("reiniciar");
                }
                return "Se realizó el proceso de reinicio (tiempo de espera 3 segundos)";
            }
            catch (Exception ex)
            {
                return $"Excepción: {ex.Message}";
            }
        }
    }
}
