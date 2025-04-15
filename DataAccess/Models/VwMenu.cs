using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DataAccess.Models
{
    [Table("vwMenu")]
    public class VwMenu
    {
        //[Key]
        //public int IdHomologacionMenu { get; set; }
        public int MostrarWebOrden { get; set; }
        public string MostrarWeb { get; set; }
        public string TooltipWeb { get; set; }
        public string Icono { get; set; }
        public string href { get; set; }
        public string CodigoHomologacion { get; set; }
        public string CodigoHomologacionRol { get; set; }
    }
}
