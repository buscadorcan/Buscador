using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vwPais")]
    public class VwPais
    {
        [Key]
        public int IdHomologacionPais { get; set; }
        public string? Pais { get; set; }
    }
}
