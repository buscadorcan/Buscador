using System.Xml.Serialization;

namespace DataAccess.Models
{
    [XmlRoot("thesaurus", Namespace = "x-schema:tsSchema.xml")]
    public class Thesaurus
    {
        [XmlElement("diacritics_sensitive")]
        public int DiacriticsSensitive { get; set; }

        [XmlElement("expansion")]
        public List<Expansion> Expansions { get; set; } = new();

        [XmlElement("replacement")]
        public List<Replacement> Replacements { get; set; } = new();
    }

    public class Expansion
    {
        [XmlElement("sub")]
        public List<string> Substitutes { get; set; } = new();
    }

    public class Replacement
    {
        [XmlElement("pat")]
        public List<string> Patterns { get; set; } = new();

        [XmlElement("sub")]
        public List<string> Substitutes { get; set; } = new();
    }
}
