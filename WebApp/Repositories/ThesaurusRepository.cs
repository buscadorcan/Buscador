using System.Diagnostics;
using System.ServiceProcess;
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
        private readonly string _nombreArchivoBat = configuration["Thesaurus:RutaArchivoBat"];
        private readonly string _nombreExec = configuration["Thesaurus:RutapsexecPath"];
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
            string mensaje = "";
            string serverName = "216.172.100.184:1097"; // Nombre o IP del servidor SQL Server
            string user = "administrator"; // Usuario con privilegios
            string password = "S!desoft@dm!n2025"; // Contraseña del usuario

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = _nombreExec,
                    Arguments = $@"\\{serverName} -u {user} -p {password} -s cmd /c {_nombreArchivoBat}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    mensaje = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        mensaje += "\nError: " + error;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error: {ex.Message}";
            }

            return mensaje;
        }
    }
}
