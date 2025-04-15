using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SharedApp.Dtos
{

    public class ThesaurusDto
    {
        public int DiacriticsSensitive { get; set; }
        public List<ExpansionDto> Expansions { get; set; } = new();


        public List<ReplacementDto> Replacements { get; set; } = new();
    }

    public class ExpansionDto
    {
        public List<string> Substitutes { get; set; } = new();
        public bool sectionValidate { get; set; } = true;
    }

    public class ReplacementDto
    {
        public List<string> Patterns { get; set; } = new();
        public List<string> Substitutes { get; set; } = new();
    }

}
